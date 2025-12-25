// Pages/Staff/Supplier/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff.Supplier
{
    public class CreateModel : PageModel
    {
        static string connectionString = "Data Source=localhost; Initial Catalog=PharmacyDatabase; Integrated Security=True; TrustServerCertificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        [BindProperty]
        public SupplierInputModel Supplier { get; set; } = new SupplierInputModel();

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                con.Open();
                string query = @"
                    INSERT INTO Suppliers (SupplierName, Email, Phone, Address) 
                    VALUES (@name, @email, @phone, @address)";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", Supplier.SupplierName);
                cmd.Parameters.AddWithValue("@email", Supplier.Email);
                cmd.Parameters.AddWithValue("@phone", Supplier.Phone);
                cmd.Parameters.AddWithValue("@address", Supplier.Address ?? (object)DBNull.Value);

                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving supplier: {ex.Message}");
                return Page();
            }
        }

        public class SupplierInputModel
        {
            public string SupplierName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string? Address { get; set; }
        }
    }
}