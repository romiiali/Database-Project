// Pages/Staff/Batch/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _202Project.Pages.Staff.Batch
{
    public class IndexModel : PageModel
    {
        static string connectionString = "Data Source=localhost; Initial Catalog=PharmacyDatabase; Integrated Security=True; TrustServerCertificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        public List<BatchItem> BatchList { get; set; } = new List<BatchItem>();

        public void OnGet()
        {
            con.Open();
            string query = @"
                SELECT b.BatchNumber, m.CommercialName, s.SupplierName, 
                       b.QuantityRecieved, b.ArrivalDate, b.ExpiryDate
                FROM Batch b
                LEFT JOIN Medicine m ON b.MedicineID = m.MedicineID
                LEFT JOIN Suppliers s ON b.SupplierID = s.SupplierID
                ORDER BY b.ExpiryDate ASC";

            var cmd = new SqlCommand(query, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    BatchList.Add(new BatchItem
                    {
                        BatchNumber = reader.GetString(0),
                        MedicineName = reader.IsDBNull(1) ? "Unknown" : reader.GetString(1),
                        SupplierName = reader.IsDBNull(2) ? "Unknown" : reader.GetString(2),
                        QuantityRecieved = reader.GetInt32(3),
                        ArrivalDate = reader.GetDateTime(4),
                        ExpiryDate = reader.GetDateTime(5)
                    });
                }
            }
            con.Close();
        }

        public class BatchItem
        {
            public string BatchNumber { get; set; }
            public string MedicineName { get; set; }
            public string SupplierName { get; set; }
            public int QuantityRecieved { get; set; }
            public DateTime ArrivalDate { get; set; }
            public DateTime ExpiryDate { get; set; }
        }
    }
}