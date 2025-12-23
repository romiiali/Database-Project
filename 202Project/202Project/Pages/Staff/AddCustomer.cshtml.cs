using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _202Project.models;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class AddCustomerModel : PageModel
    {
        [BindProperty]
        public Customer CustomerForm { get; set; } = new Customer();
        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";
        public void OnGet()
        {
        }


        public IActionResult OnPostAdd()

        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                int newCustomerId;
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(CustomerId), 1) + 1 FROM Customer", con))
                {
                    newCustomerId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                string customerquery = @"INSERT INTO Customer 
                    (CustomerId,Fname, Lname, Phone, Email) 
                    VALUES (@id,@fname, @lname, @phone, @email)";

                // First insert medicine
                using (var cmd = new SqlCommand(customerquery, con))
                {
                    cmd.Parameters.AddWithValue("@id", newCustomerId);
                    cmd.Parameters.AddWithValue("@fname", CustomerForm.FName);
                    cmd.Parameters.AddWithValue("@lname", CustomerForm.LName);
                    cmd.Parameters.AddWithValue("@phone", CustomerForm.Phone);
                    cmd.Parameters.AddWithValue("@email", CustomerForm.Email);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Staff/Customers"); // Redirect to medicines list
        }
    }
}
