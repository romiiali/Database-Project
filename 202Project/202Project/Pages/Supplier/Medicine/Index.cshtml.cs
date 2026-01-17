// Pages/Staff/Medicine/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _202Project.Pages.Staff.Medicine
{
    public class IndexModel : PageModel
    {
        static string connectionString = "Data Source=localhost; Initial Catalog=PharmacyDatabase; Integrated Security=True; TrustServerCertificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        public List<MedicineItem> MedicineList { get; set; } = new List<MedicineItem>();

        public void OnGet()
        {
            con.Open();
            string query = @"
                SELECT m.MedicineID, m.CommercialName, m.ScientificName, 
                       m.Price, m.Dosage, c.CategoryName
                FROM Medicine m
                LEFT JOIN Category c ON m.CategoryID = c.CategoryID
                ORDER BY m.CommercialName";

            var cmd = new SqlCommand(query, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    MedicineList.Add(new MedicineItem
                    {
                        MedicineID = reader.GetInt32(0),
                        CommercialName = reader.GetString(1),
                        ScientificName = reader.GetString(2),
                        Price = reader.GetDecimal(3),
                        Dosage = reader.IsDBNull(4) ? null : reader.GetString(4),
                        CategoryName = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5)
                    });
                }
            }
            con.Close();
        }

        public class MedicineItem
        {
            public int MedicineID { get; set; }
            public string CommercialName { get; set; }
            public string ScientificName { get; set; }
            public decimal Price { get; set; }
            public string? Dosage { get; set; }
            public string CategoryName { get; set; }
        }
    }
}