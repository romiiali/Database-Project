// Pages/Staff/Supplier/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _202Project.Pages.Staff.Supplier
{
    public class IndexModel : PageModel
    {
        static string connectionString = "Data Source=localhost; Initial Catalog=PharmacyDatabase; Integrated Security=True; TrustServerCertificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        public List<SupplierItem> SupplierList { get; set; } = new List<SupplierItem>();

        public void OnGet()
        {
            con.Open();
            string query = "SELECT SupplierID, SupplierName, Email, Phone, Address FROM Suppliers";
            var cmd = new SqlCommand(query, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    SupplierList.Add(new SupplierItem
                    {
                        SupplierID = reader.GetInt32(0),
                        SupplierName = reader.GetString(1),
                        Email = reader.GetString(2),
                        Phone = reader.GetString(3),
                        Address = reader.IsDBNull(4) ? null : reader.GetString(4)
                    });
                }
            }
            con.Close();
        }

        public class SupplierItem
        {
            public int SupplierID { get; set; }
            public string SupplierName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string? Address { get; set; }
        }
    }
}