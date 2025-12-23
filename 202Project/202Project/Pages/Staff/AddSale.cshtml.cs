using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace _202Project.Pages.Staff
{
    public class AddSaleModel : PageModel
    {
        // Model for individual medicine items
        public class MedicineItem
        {
            public int BatchId { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
            public int Quantity { get; set; } = 1;
        }

        // Simple model for sale form
        public class SaleForm
        {
            [Required(ErrorMessage = "Please select a customer")]
            public int CustomerId { get; set; }

            [Range(0, double.MaxValue, ErrorMessage = "Discount cannot be negative")]
            public int? Discount { get; set; }
            public int SaleId { get; set; }
        }

        // For dropdowns
        public List<SelectListItem> Medicines { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();

        // For form binding
        [BindProperty]
        public SaleForm Sale { get; set; } = new SaleForm();

        // List of medicine items (bound as collection)
        [BindProperty]
        public List<MedicineItem> MedicineItems { get; set; } = new List<MedicineItem>();

        // Hidden field for item count
        [BindProperty]
        public int ItemCount { get; set; }

        // For add/remove actions
        [BindProperty]
        public string Action { get; set; }

        [BindProperty]
        public int? RemoveIndex { get; set; }

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;Trust Server Certificate=True";

        public void OnGet()
        {
            LoadMedicines();
            LoadCustomers();

            // Initialize with one empty medicine item
            if (MedicineItems.Count == 0)
            {
                MedicineItems.Add(new MedicineItem());
            }
        }

        public IActionResult OnPost()
        {
            LoadMedicines();
            LoadCustomers();

            // Handle Add button click
            if (Action == "add")
            {
                MedicineItems.Add(new MedicineItem());
                return Page();
            }

            // Handle Remove button click
            if (RemoveIndex.HasValue && RemoveIndex >= 0 && RemoveIndex < MedicineItems.Count)
            {
                MedicineItems.RemoveAt(RemoveIndex.Value);
                return Page();
            }

            // Handle form submission (Complete Sale button)
            if (Action == "submit")
            {
                // Validate at least one medicine item
                if (MedicineItems.Count == 0 || MedicineItems.All(m => m.BatchId == 0))
                {
                    ModelState.AddModelError("", "Please select at least one medicine.");
                    return Page();
                }

                // Validate each medicine item
                bool hasValidItems = false;
                for (int i = 0; i < MedicineItems.Count; i++)
                {
                    var item = MedicineItems[i];
                    if (item.BatchId > 0 && item.Quantity > 0)
                    {
                        hasValidItems = true;
                    }
                }

                if (!hasValidItems)
                {
                    ModelState.AddModelError("", "Please select at least one medicine with quantity greater than 0.");
                    return Page();
                }

                if (!ModelState.IsValid)
                    return Page();

                // Process the sale
                return ProcessSale();
            }

            return Page();
        }

        private IActionResult ProcessSale()
        {
            using SqlConnection con = new SqlConnection(connectionString);
            con.Open();
            using SqlTransaction transaction = con.BeginTransaction();

            try
            {
                decimal totalAmount = 0;
                const int staffId = 3; // TODO: Replace with logged-in staff ID
                int saleId;
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(SaleId), 1) + 1 FROM SalesTransaction", con, transaction))
                {
                    saleId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // 1. Insert SaleTransaction
                string insertSaleQuery = @"
            INSERT INTO SalesTransaction
            (SaleId,SaleStatus, SaleTime, Discount, TotalAmount, CustomerId, StaffId)
            VALUES (@saleId,'Completed', GETDATE(), @Discount, 0, @CustomerId, @StaffId);";

                using (SqlCommand saleCmd = new SqlCommand(insertSaleQuery, con, transaction))
                {
                    saleCmd.Parameters.AddWithValue("@saleId", saleId);
                    saleCmd.Parameters.AddWithValue("@Discount", Sale.Discount ?? 0);
                    saleCmd.Parameters.AddWithValue("@CustomerId", Sale.CustomerId);
                    saleCmd.Parameters.AddWithValue("@StaffId", staffId);
                    saleCmd.ExecuteNonQuery();

                }

                // 2. Process each medicine item
                foreach (var item in MedicineItems.Where(m => m.BatchId > 0 && m.Quantity > 0))
                {
                    // Get fresh stock & price
                    string checkQuery = @"
                        SELECT m.Price, b.QuantityRecieved, m.CommercialName
                        FROM Batch b
                        JOIN Medicine m ON b.MedicineId = m.MedicineId
                        WHERE b.BatchId = @BatchId";

                    decimal price;
                    int availableQty;
                    string medName;

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, con, transaction))
                    {
                        checkCmd.Parameters.AddWithValue("@BatchId", item.BatchId);
                        using SqlDataReader r = checkCmd.ExecuteReader();

                        if (!r.Read())
                            throw new Exception("Selected medicine batch not found.");

                        price = r.GetDecimal(0);
                        availableQty = r.GetInt32(1);
                        medName = r.GetString(2);
                    }

                    if (availableQty < item.Quantity)
                        throw new Exception($"Insufficient stock for {medName}. Available: {availableQty}");

                    // Insert SaleItem
                    string insertItemQuery = @"
                        INSERT INTO SaleItem (SaleId, BatchId, Price, QuantitySold)
                        VALUES (@SaleId, @BatchId, @Price, @Quantity)";

                    using (SqlCommand itemCmd = new SqlCommand(insertItemQuery, con, transaction))
                    {
                        itemCmd.Parameters.AddWithValue("@SaleId", saleId);
                        itemCmd.Parameters.AddWithValue("@BatchId", item.BatchId);
                        itemCmd.Parameters.AddWithValue("@Price", price);
                        itemCmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        itemCmd.ExecuteNonQuery();
                    }

                    // Update Batch quantity
                    string updateBatchQuery = @"
                        UPDATE Batch
                        SET QuantityRecieved = QuantityRecieved - @Quantity
                        WHERE BatchId = @BatchId
                          AND QuantityRecieved >= @Quantity";

                    using (SqlCommand updateCmd = new SqlCommand(updateBatchQuery, con, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@BatchId", item.BatchId);
                        updateCmd.Parameters.AddWithValue("@Quantity", item.Quantity);

                        if (updateCmd.ExecuteNonQuery() == 0)
                            throw new Exception($"Stock conflict for {medName}");
                    }

                    totalAmount += price * item.Quantity;
                }

                // 3. Apply discount
                if (Sale.Discount.HasValue && Sale.Discount > 0)
                {
                    totalAmount -= Sale.Discount.Value;
                    if (totalAmount < 0)
                        totalAmount = 0;
                }

                // 4. Update total amount
                string updateSaleTotalQuery = @"
                    UPDATE SalesTransaction
                    SET TotalAmount = @TotalAmount
                    WHERE SaleId = @SaleId";

                using (SqlCommand updateSaleCmd = new SqlCommand(updateSaleTotalQuery, con, transaction))
                {
                    updateSaleCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                    updateSaleCmd.Parameters.AddWithValue("@SaleId", saleId);
                    updateSaleCmd.ExecuteNonQuery();
                }

                transaction.Commit();

                TempData["SuccessMessage"] =
                    $"Sale #{saleId} completed successfully. Total: ${totalAmount:F2}";

                return RedirectToPage("/Staff/Sale");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }

        private void LoadMedicines()
        {
            Medicines.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"
                    SELECT b.BatchId, m.CommercialName, m.Price, b.QuantityRecieved
                    FROM Batch b 
                    INNER JOIN Medicine m ON b.MedicineId = m.MedicineId 
                    WHERE b.QuantityRecieved > 0 
                    AND b.ExpiryDate > GETDATE() 
                    ORDER BY m.CommercialName";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Medicines.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = $"{reader.GetString(1)} - ${reader.GetInt32(2)} (Available: {reader.GetInt32(3)})"
                        });
                    }
                }
            }
        }

        private void LoadCustomers()
        {
            Customers.Clear();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT CustomerId, CONCAT(Fname, ' ', Lname) as CustomerName FROM Customer ORDER BY Fname";

                using (SqlCommand cmd = new SqlCommand(query, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Customers.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(),
                            Text = reader.GetString(1)
                        });
                    }
                }
            }
        }
    }
}