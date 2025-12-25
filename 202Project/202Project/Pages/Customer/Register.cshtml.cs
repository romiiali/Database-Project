// Pages/Customer/Register.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Customer // Fixed namespace
{
    public class RegisterModel : PageModel
    {
        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;Trust Server Certificate=True";

        [BindProperty]
        public CustomerInputModel CustomerInput { get; set; } = new CustomerInputModel();

        [TempData]
        public string SuccessMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    con.Open();
                    string query = @"
                        INSERT INTO Customers (FName, LName, Phone, Email) 
                        VALUES (@fname, @lname, @phone, @email);
                        SELECT SCOPE_IDENTITY();";

                    var cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@fname", CustomerInput.FName);
                    cmd.Parameters.AddWithValue("@lname", CustomerInput.LName);
                    cmd.Parameters.AddWithValue("@phone", CustomerInput.Phone);
                    cmd.Parameters.AddWithValue("@email",
                        string.IsNullOrEmpty(CustomerInput.Email) ? (object)DBNull.Value : CustomerInput.Email);

                    var customerId = Convert.ToInt32(cmd.ExecuteScalar());

                    SuccessMessage = $"Registration successful! Your customer ID is: {customerId}";
                    return RedirectToPage("/Customer/Dashboard");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred during registration: {ex.Message}";
                return Page();
            }
        }

        public class CustomerInputModel
        {
            public string FName { get; set; } = string.Empty;
            public string LName { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Email { get; set; }
        }
    }
}