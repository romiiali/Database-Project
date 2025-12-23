using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class AddMedicineModel : PageModel
    {
        [BindProperty]
        public MedicineForm Medicine { get; set; } = new MedicineForm();

        [BindProperty]  
        public BatchForm Batch { get; set; } = new BatchForm();

        public List<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";

        public class MedicineForm
        {
            public int newMedicineId { get; set; }
            public string CommercialName { get; set; }
            public string ScientificName { get; set; }
            public float Price { get; set; }
            public float Dosage { get; set; }
            public int CategoryId { get; set; }
            public int ReorderQuantity { get; set; }  // Fixed typo: Reoder → Reorder
        }

        public class BatchForm
        {
            public int newBatchId { get; set; }
            public string BatchNumber { get; set; }
            public int SupplierId { get; set; }
            public int QuantityRecieved { get; set; }
            public DateTime ArrivalDate { get; set; }
            public DateTime ExpiryDate { get; set; }
        }

        public void OnGet()
        {
            LoadSuppliers();
            LoadCategories();
        }

        private void LoadSuppliers()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT SupplierId, SupplierName FROM Supplier ORDER BY SupplierName";

                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Suppliers.Add(new SelectListItem
                        {
                            Value = reader["SupplierId"].ToString(),
                            Text = reader["SupplierName"].ToString()
                        });
                    }
                }
            }
        }

        private void LoadCategories()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT CategoryId, CategoryName FROM Category ORDER BY CategoryName";

                using (var cmd = new SqlCommand(query, con))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Categories.Add(new SelectListItem
                        {
                            Value = reader["CategoryId"].ToString(),
                            Text = reader["CategoryName"].ToString()
                        });
                    }
                }
            }
        }

        public IActionResult OnPostAdd() 
        
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                int newMedicineId;
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(MedicineId), 1) + 1 FROM Medicine", con))
                {
                    newMedicineId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                int newBatchId;
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(BatchId), 1) + 1 FROM Batch", con))
                {
                    newBatchId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                string medicineQuery = @"INSERT INTO Medicine 
                    (MedicineId,CommercialName, ScientificName, Price, Dosage, CategoryId, ReorderQuantity) 
                    VALUES (@id,@name, @scientific, @price, @dosage, @category, @stock)";

                string batchQuery = @"INSERT INTO Batch 
                    (BatchId,MedicineId, BatchNumber, SupplierId, QuantityRecieved, ArrivalDate,ExpiryDate) 
                    VALUES (@id,@medicineId, @batchNumber, @supplierId, @quantity, @arrival,@expiry)";

                // First insert medicine
                using (var cmd = new SqlCommand(medicineQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", newMedicineId);
                    cmd.Parameters.AddWithValue("@name", Medicine.CommercialName);
                    cmd.Parameters.AddWithValue("@scientific", Medicine.ScientificName);
                    cmd.Parameters.AddWithValue("@price", Medicine.Price);
                    cmd.Parameters.AddWithValue("@dosage", Medicine.Dosage);
                    cmd.Parameters.AddWithValue("@category", Medicine.CategoryId);
                    cmd.Parameters.AddWithValue("@stock", Medicine.ReorderQuantity);
                    cmd.ExecuteNonQuery();
                }


                // Then insert batch
                using (var cmd = new SqlCommand(batchQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", newBatchId);
                    cmd.Parameters.AddWithValue("@medicineId", newMedicineId);
                    cmd.Parameters.AddWithValue("@batchNumber", Batch.BatchNumber);
                    cmd.Parameters.AddWithValue("@supplierId", Batch.SupplierId);
                    cmd.Parameters.AddWithValue("@quantity", Batch.QuantityRecieved);
                    cmd.Parameters.AddWithValue("@arrival", Batch.ArrivalDate);
                    cmd.Parameters.AddWithValue("@expiry", Batch.ExpiryDate);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Staff/Medicines"); // Redirect to medicines list
        }
    }
}