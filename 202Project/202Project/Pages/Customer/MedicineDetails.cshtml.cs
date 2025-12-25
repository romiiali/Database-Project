// Pages/Customer/MedicineDetails.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _202Project.Pages.Customer // Fixed namespace
{
    public class MedicineDetailsModel : PageModel
    {
        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;Trust Server Certificate=True";

        public MedicineItem Medicine { get; set; }
        public List<BatchItem> AvailableBatches { get; set; } = new List<BatchItem>();
        public List<MedicineItem> RelatedMedicines { get; set; } = new List<MedicineItem>();

        public IActionResult OnGet(int id)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                // Get medicine
                string medicineQuery = @"
                    SELECT m.MedicineID, m.CommercialName, m.ScientificName, 
                           m.Price, c.CategoryName, m.Dosage, m.Description, m.RecorderQuantity
                    FROM Medicine m
                    LEFT JOIN Category c ON m.CategoryID = c.CategoryID
                    WHERE m.MedicineID = @id";

                var cmd = new SqlCommand(medicineQuery, con);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Medicine = new MedicineItem
                        {
                            MedicineID = reader.GetInt32(0),
                            CommercialName = reader.GetString(1),
                            ScientificName = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            CategoryName = reader.GetString(4),
                            Dosage = reader.IsDBNull(5) ? null : reader.GetString(5),
                            Description = reader.IsDBNull(6) ? null : reader.GetString(6),
                            RecorderQuantity = reader.GetInt32(7)
                        };
                    }
                    else
                    {
                        return NotFound();
                    }
                }

                // Get batches
                string batchQuery = @"
                    SELECT BatchNumber, QuantityRecieved, ArrivalDate, ExpiryDate
                    FROM Batch 
                    WHERE MedicineID = @id 
                    AND QuantityRecieved > 0 
                    AND ExpiryDate > GETDATE()";

                cmd = new SqlCommand(batchQuery, con);
                cmd.Parameters.AddWithValue("@id", id);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        AvailableBatches.Add(new BatchItem
                        {
                            BatchNumber = reader.GetString(0),
                            QuantityReceived = reader.GetInt32(1),
                            ArrivalDate = reader.GetDateTime(2),
                            ExpiryDate = reader.GetDateTime(3)
                        });
                    }
                }

                // Get related medicines
                if (!string.IsNullOrEmpty(Medicine.CategoryName))
                {
                    string relatedQuery = @"
                        SELECT TOP 4 m.MedicineID, m.CommercialName, m.ScientificName, 
                               m.Price, c.CategoryName
                        FROM Medicine m
                        LEFT JOIN Category c ON m.CategoryID = c.CategoryID
                        WHERE c.CategoryName = @category 
                        AND m.MedicineID != @id";

                    cmd = new SqlCommand(relatedQuery, con);
                    cmd.Parameters.AddWithValue("@category", Medicine.CategoryName);
                    cmd.Parameters.AddWithValue("@id", id);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RelatedMedicines.Add(new MedicineItem
                            {
                                MedicineID = reader.GetInt32(0),
                                CommercialName = reader.GetString(1),
                                ScientificName = reader.GetString(2),
                                Price = reader.GetDecimal(3),
                                CategoryName = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return Page();
        }

        public class MedicineItem
        {
            public int MedicineID { get; set; }
            public string CommercialName { get; set; }
            public string ScientificName { get; set; }
            public decimal Price { get; set; }
            public string CategoryName { get; set; }
            public string Dosage { get; set; }
            public string Description { get; set; }
            public int RecorderQuantity { get; set; }
        }

        public class BatchItem
        {
            public string BatchNumber { get; set; }
            public int QuantityReceived { get; set; }
            public DateTime ArrivalDate { get; set; }
            public DateTime ExpiryDate { get; set; }
        }
    }
}