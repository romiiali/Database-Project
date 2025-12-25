using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using _202Project.models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace _202Project.Pages.Staff
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
    public class EditCustomerModel : PageModel
    {
        [BindProperty]
        public Customers CustomerForm { get; set; }
        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";
        public void OnGet(int id)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                string medicineQuery = @"SELECT CustomerID, Fname,Lname,Phone,Email FROM Customer WHERE CustomerId = @id";

                using (var cmd = new SqlCommand(medicineQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            CustomerForm = new Customers 
                            {
                                CustomerId = reader.GetInt32(0),
                                FName = reader.GetString(1),
                                LName = reader.GetString(2),
                                Phone = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3),
                                Email = reader.IsDBNull(4) ? null : reader.GetString(4)
                            };
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Customer not found!";
                            return;
                        }
                    }
                }
            }
        }

        public IActionResult OnPostEdit()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                var customerquery = new List<string>();
                var customerparams = new SqlCommand();

                if (!string.IsNullOrEmpty(CustomerForm.FName))
                {
                    customerquery.Add("Fname = @fname");
                    customerparams.Parameters.AddWithValue("@fname", CustomerForm.FName);
                }
                if (!string.IsNullOrEmpty(CustomerForm.LName))
                {
                    customerquery.Add("Lname = @lname");
                    customerparams.Parameters.AddWithValue("@lname", CustomerForm.LName);
                }
                if (CustomerForm.Phone.HasValue)
                {
                    customerquery.Add("Phone = @phone");
                    customerparams.Parameters.AddWithValue("@phone", CustomerForm.Phone.Value);
                }
                if (!string.IsNullOrEmpty(CustomerForm.Email))
                {
                    customerquery.Add("Email = @email");
                    customerparams.Parameters.AddWithValue("@email", CustomerForm.Email);
                }

                if (customerquery.Count > 0)
                {
                    string customerQuery = $@"
                        UPDATE Customer SET 
                        {string.Join(", ", customerquery)}
                        WHERE CustomerId = @id";

                    using (var cmd = new SqlCommand(customerQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@id", CustomerForm.CustomerId);

                        foreach (SqlParameter param in customerparams.Parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value));
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
                return RedirectToPage("/Staff/Customers");
            }
        }

    }
}
