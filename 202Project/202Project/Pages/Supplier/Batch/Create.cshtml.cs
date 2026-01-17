// Pages/Staff/Batch/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _202Project.Pages.Staff.Batch
{
    public class CreateModel : PageModel
    {
        static string connectionString = "Data Source=localhost; Initial Catalog=PharmacyDatabase; Integrated Security=True; TrustServerCertificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        [BindProperty]
        public BatchInputModel Batch { get; set; } = new BatchInputModel();

        public List<MedicineItem> Medicines { get; set; } = new List<MedicineItem>();
        public List<SupplierItem> Suppliers { get; set; } = new List<SupplierItem>();

        public IActionResult OnGet()
        {
            LoadDropdownData();
            return Page();
        }

        private void LoadDropdownData()
        {
            con.Open();

            // Load medicines
            string medicineQuery = "SELECT MedicineID, CommercialName FROM Medicine ORDER BY CommercialName";
            var cmd = new SqlCommand(medicineQuery, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Medicines.Add(new MedicineItem
                    {
                        MedicineID = reader.GetInt32(0),
                        CommercialName = reader.GetString(1)
                    });
                }
            }

            // Load suppliers
            string supplierQuery = "SELECT SupplierID, SupplierName FROM Suppliers ORDER BY SupplierName";
            cmd = new SqlCommand(supplierQuery, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Suppliers.Add(new SupplierItem
                    {
                        SupplierID = reader.GetInt32(0),
                        SupplierName = reader.GetString(1)
                    });
                }
            }

            con.Close();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                LoadDropdownData();
                return Page();
            }

            if (Batch.ExpiryDate <= Batch.ArrivalDate)
            {
                ModelState.AddModelError("Batch.ExpiryDate", "Error: The Expiry Date must be after the Arrival Date.");
                LoadDropdownData();
                return Page();
            }

            try
            {
                con.Open();
                string query = @"
                    INSERT INTO Batch (MedicineID, SupplierID, BatchNumber, QuantityRecieved, ArrivalDate, ExpiryDate) 
                    VALUES (@medicineId, @supplierId, @batchNumber, @quantity, @arrivalDate, @expiryDate)";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@medicineId", Batch.MedicineID);
                cmd.Parameters.AddWithValue("@supplierId", Batch.SupplierID);
                cmd.Parameters.AddWithValue("@batchNumber", Batch.BatchNumber);
                cmd.Parameters.AddWithValue("@quantity", Batch.QuantityRecieved);
                cmd.Parameters.AddWithValue("@arrivalDate", Batch.ArrivalDate);
                cmd.Parameters.AddWithValue("@expiryDate", Batch.ExpiryDate);

                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving batch: {ex.Message}");
                LoadDropdownData();
                return Page();
            }
        }

        public class BatchInputModel
        {
            public int MedicineID { get; set; }
            public int SupplierID { get; set; }
            public string BatchNumber { get; set; } = string.Empty;
            public int QuantityRecieved { get; set; }
            public DateTime ArrivalDate { get; set; }
            public DateTime ExpiryDate { get; set; }
        }

        public class MedicineItem
        {
            public int MedicineID { get; set; }
            public string CommercialName { get; set; }
        }

        public class SupplierItem
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
        }
    }
}