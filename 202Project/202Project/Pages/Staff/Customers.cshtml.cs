using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class CustomersModel : PageModel
    {
        public class Customers
        {
            public int CustomerId { get; set; }
            public string? Email { get; set; } = string.Empty;
            public string? Password { get; set; } = string.Empty;
            public string? FName { get; set; } = string.Empty;
            public string? LName { get; set; } = string.Empty;
            public int? Phone { get; set; }

        }

        public List<Customers> Customer { get; set; } = new List<Customers>();

        static string connectionString = "Data Source = localhost; Initial Catalog = PharmacyDatabase; Integrated Security = True; Trust Server Certificate=True";
        SqlConnection con = new SqlConnection(connectionString);
        public void OnGet()
        {
            string queryString = "SELECT * from Customer";
            con.Open();
            SqlCommand cmd = new SqlCommand(queryString, con);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var customer = new Customers
                {
                    CustomerId = reader.GetInt32(0),
                    FName = reader.GetString(1),
                    LName = reader.GetString(2),
                    Phone = reader.GetInt32(3),
                    Email = reader.GetString(4)
                };
                Customer.Add(customer);
            }
            ;
            reader.Close();

            con.Close();
        }

        public IActionResult OnPostDelete(int id)
        {
            con.Open();
            string querystring = "Update SalesTransaction set CustomerId= NULL where CustomerId=@id delete from Customer where CustomerId=@id ";
            var cmd = new SqlCommand(querystring, con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
            return RedirectToPage();

        }
    }
}
