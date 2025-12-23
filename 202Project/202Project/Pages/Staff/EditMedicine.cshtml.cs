using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class EditMedicineModel : PageModel
    {
        public class MedicineForm
        {
            public int MedicineId { get; set; }
            public string? CommercialName { get; set; }
            public string? ScientificName { get; set; }
            public int? Price { get; set; }
            public int? Dosage { get; set; }
            public int? CategoryId { get; set; }
            public int? ReorderQuantity { get; set; }  // Fixed typo: Reoder → Reorder
        }

        public class BatchForm
        {
            public int BatchId { get; set; }
            public int? BatchNumber { get; set; }
            public int? SupplierId { get; set; }
            public int? QuantityRecieved { get; set; }
            public DateTime? ArrivalDate { get; set; }
            public DateTime? ExpiryDate { get; set; }
        }
        [BindProperty]
        public MedicineForm Medicine { get; set; } = new MedicineForm();

        [BindProperty]  // Add this for Batch
        public BatchForm Batch { get; set; } = new BatchForm();
        public List<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";

        public IActionResult OnGet(int id)
        {
            LoadSuppliers();
            LoadCategories();
            LoadMedicineData(id);
            return Page();
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

        private void LoadMedicineData(int medicineId)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                // Load medicine details - FIXED COLUMN NAME
                string medicineQuery = @"
            SELECT MedicineId, CommercialName, ScientificName, Price, 
                   Dosage, CategoryId, ReorderQuantity
            FROM Medicine 
            WHERE MedicineId = @id";

                using (var cmd = new SqlCommand(medicineQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", medicineId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Medicine.MedicineId = reader.GetInt32(0);
                            Medicine.CommercialName = reader.GetString(1);
                            Medicine.ScientificName = reader.IsDBNull(2) ? null : reader.GetString(2);
                            Medicine.Price = reader.GetInt32(3);
                            Medicine.Dosage =reader.GetInt32(4);
                            Medicine.CategoryId = reader.GetInt32(5);

                            // Safe handling for ReorderQuantity column
                            if (!reader.IsDBNull(6))
                            {
                                var column6Value = reader[6];

                                // Try different parsing approaches
                                if (column6Value is int)
                                {
                                    Medicine.ReorderQuantity = (int)column6Value;
                                }
                                else if (column6Value is decimal)
                                {
                                    Medicine.ReorderQuantity = Convert.ToInt32((decimal)column6Value);
                                }
                                else if (int.TryParse(column6Value.ToString(), out int parsedValue))
                                {
                                    Medicine.ReorderQuantity = parsedValue;
                                }
                                else
                                {
                                    // If parsing fails, set to default
                                    Medicine.ReorderQuantity = 0;
                                }
                            }
                            else
                            {
                                // Default when NULL
                                Medicine.ReorderQuantity = 0;
                            }
                        }
                        else
                        {
                            // Medicine not found
                            TempData["ErrorMessage"] = "Medicine not found!";
                            return; // Exit early since no medicine found
                        }
                    }
                }

                // Load latest batch for this medicine
                string batchQuery = @"
            SELECT TOP 1 BatchId, BatchNumber, SupplierId, 
                   QuantityRecieved, ArrivalDate, ExpiryDate
            FROM Batch 
            WHERE MedicineId = @medicineId
            ORDER BY BatchId DESC";

                using (var cmd = new SqlCommand(batchQuery, con))
                {
                    cmd.Parameters.AddWithValue("@medicineId", medicineId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Batch.BatchId = reader.GetInt32(0);
                            Batch.BatchNumber = reader.GetInt32(1);
                            Batch.SupplierId = reader.GetInt32(2);
                            Batch.QuantityRecieved = reader.GetInt32(3);
                            Batch.ArrivalDate = reader.GetDateTime(4);
                            Batch.ExpiryDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
                        }
                        // If no batch found, Batch will remain with default values
                    }
                }
            }
        }
        public IActionResult OnPostEdit()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var medicinequery = new List<string>();
                var medicineParams = new SqlCommand();

                if (!string.IsNullOrEmpty(Medicine.CommercialName))
                {
                    medicinequery.Add("CommercialName = @name");
                    medicineParams.Parameters.AddWithValue("@name", Medicine.CommercialName);
                }
                if (!string.IsNullOrEmpty(Medicine.ScientificName))
                {
                    medicinequery.Add("ScientificName = @scientific");
                    medicineParams.Parameters.AddWithValue("@scientific", Medicine.ScientificName);
                }
                if (Medicine.Price.HasValue)
                {
                    medicinequery.Add("Price = @price");
                    medicineParams.Parameters.AddWithValue("@price", Medicine.Price.Value);
                }
                if (Medicine.Dosage.HasValue)
                {
                    medicinequery.Add("Dosage = @dosage");
                    medicineParams.Parameters.AddWithValue("@dosage", Medicine.Dosage.Value);
                }
                if (Medicine.CategoryId.HasValue)
                {
                    medicinequery.Add("CategoryId = @category");
                    medicineParams.Parameters.AddWithValue("@category", Medicine.CategoryId.Value);
                }
                if (Medicine.ReorderQuantity.HasValue)
                {
                    medicinequery.Add("ReorderQuantity = @stock");
                    medicineParams.Parameters.AddWithValue("@stock", Medicine.ReorderQuantity.Value);
                }

                if (medicinequery.Count > 0)
                {
                    string medicineQuery = $@"
                        UPDATE Medicine SET 
                        {string.Join(", ", medicinequery)}
                        WHERE MedicineId = @id";

                    using (var cmd = new SqlCommand(medicineQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", Medicine.MedicineId);

                        // Copy parameters from medicineParams
                        foreach (SqlParameter param in medicineParams.Parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value));
                        }

                        cmd.ExecuteNonQuery();
                    }
                }


                var batchUpdates = new List<string>();
                var batchParams = new SqlCommand();

                if (Batch.BatchNumber.HasValue)
                {
                    batchUpdates.Add("BatchNumber = @batchNumber");
                    batchParams.Parameters.AddWithValue("@batchNumber", Batch.BatchNumber);
                }
                if (Batch.SupplierId.HasValue)
                {
                    batchUpdates.Add("SupplierId = @supplierId");
                    batchParams.Parameters.AddWithValue("@supplierId", Batch.SupplierId.Value);
                }
                if (Batch.QuantityRecieved.HasValue)
                {
                    batchUpdates.Add("QuantityReceived = @quantity");
                    batchParams.Parameters.AddWithValue("@quantity", Batch.QuantityRecieved.Value);
                }
                if (Batch.ArrivalDate.HasValue)
                {
                    batchUpdates.Add("ArrivalDate = @arrival");
                    batchParams.Parameters.AddWithValue("@arrival", Batch.ArrivalDate.Value);
                }
                if (Batch.ExpiryDate.HasValue)
                {
                    batchUpdates.Add("ExpiryDate = @expiry");
                    batchParams.Parameters.AddWithValue("@expiry", Batch.ExpiryDate.Value);
                }

                // Only update batch if it exists AND has changes
                if (batchUpdates.Count > 0 && Batch.BatchId > 0)
                {
                    string batchQuery = $@"
                        UPDATE Batch SET 
                        {string.Join(", ", batchUpdates)}
                        WHERE BatchId = @batchId AND MedicineId = @medicineId";

                    using (var cmd = new SqlCommand(batchQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@batchId", Batch.BatchId);
                        cmd.Parameters.AddWithValue("@medicineId", Medicine.MedicineId);

                        // Copy parameters from batchParams
                        foreach (SqlParameter param in batchParams.Parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value));
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToPage("/Staff/Medicines");
        }
    }
}
