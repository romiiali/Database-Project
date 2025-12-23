using _202Project.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class CustomersModel : PageModel
    {
         
        public List<Customer> Customers { get; set; } = new List<Customer>();

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
                var customer = new Customer
                {
                    Id = reader.GetInt32(0),
                    FName = reader.GetString(1),
                    LName = reader.GetString(2),
                    Phone = reader.GetInt32(3),
                    Email = reader.GetString(4)
                };
                Customers.Add(customer);
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
