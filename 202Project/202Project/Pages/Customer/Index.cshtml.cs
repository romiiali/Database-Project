// Pages/Customer/Index.cshtml.cs
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Data;

namespace _202Project.Pages.Customer // Fixed namespace
{
    public class IndexModel : PageModel
    {
        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;Trust Server Certificate=True";


        public List<CustomerItem> CustomerList { get; set; } = new List<CustomerItem>();

        public void OnGet()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT CustomerID, FName, LName, Phone, Email FROM Customers";
                var cmd = new SqlCommand(query, con);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CustomerList.Add(new CustomerItem
                        {
                            CustomerID = reader.GetInt32(0),
                            FName = reader.GetString(1),
                            LName = reader.GetString(2),
                            Phone = reader.GetString(3),
                            Email = reader.IsDBNull(4) ? null : reader.GetString(4)
                        });
                    }
                }
            }
        }

        public class CustomerItem
        {
            public int CustomerID { get; set; }
            public string FName { get; set; }
            public string LName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
        }
    }
}