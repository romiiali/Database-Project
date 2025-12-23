using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class AddSupplierModel : PageModel
    {
        [BindProperty]
        public SupplierForm Supplier { get; set; } = new SupplierForm();

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";

        public class SupplierForm
        {
            public string Name { get; set; }
            public int Phone { get; set; } // Phone as int
            public string Email { get; set; }
            public string Address { get; set; }
        }

        public void OnGet()
        {
        }

        public IActionResult OnPostAdd()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                int newSupplierId = 0;
                using (var cmd = new SqlCommand("SELECT ISNULL(MAX(SupplierId), 0) + 1 FROM Supplier", con))
                {
                    newSupplierId = Convert.ToInt32(cmd.ExecuteScalar());
                }
                string supplierQuery = @"INSERT INTO Supplier 
                    (SupplierId, SupplierName, Phone, Email, Address) 
                    VALUES (@id, @name, @phone, @email, @address)";

                using (var cmd = new SqlCommand(supplierQuery, con))
                {
                    cmd.Parameters.AddWithValue("@id", newSupplierId);
                    cmd.Parameters.AddWithValue("@name", Supplier.Name);
                    cmd.Parameters.AddWithValue("@phone", Supplier.Phone);
                    cmd.Parameters.AddWithValue("@email", Supplier.Email);
                    cmd.Parameters.AddWithValue("@address", Supplier.Address);
                    cmd.ExecuteNonQuery();
                }
            }

            return RedirectToPage("/Staff/Supplier");
        }
    }
}