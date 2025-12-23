using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff
{
    public class EditSupplierModel : PageModel
    {
        [BindProperty]
        public SupplierForm Supplier { get; set; } = new SupplierForm();

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";

        public class SupplierForm
        {
            public int SupplierId { get; set; }
            public string? Name { get; set; }
            public int? Phone { get; set; } 
            public string? Email { get; set; }
            public string? Address { get; set; }
        }

        public IActionResult OnGet(int id)
        {
            LoadSupplierData(id);
            return Page();
        }

        private void LoadSupplierData(int supplierId)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = @"SELECT SupplierId, SupplierName, Phone, Email, Address 
                                 FROM Supplier 
                                 WHERE SupplierId = @id";

                using (var cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@id", supplierId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Supplier.SupplierId = reader.GetInt32(0);
                            Supplier.Name = reader.GetString(1);
                            Supplier.Phone = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
                            Supplier.Email = reader.IsDBNull(3) ? null : reader.GetString(3);
                            Supplier.Address = reader.IsDBNull(4) ? null : reader.GetString(4);
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Supplier not found!";
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

                var updates = new List<string>();
                var updateParams = new SqlCommand();

                if (!string.IsNullOrEmpty(Supplier.Name))
                {
                    updates.Add("SupplierName = @name");
                    updateParams.Parameters.AddWithValue("@name", Supplier.Name);
                }
                // Phone is not nullable, so always update it
                updates.Add("Phone = @phone");
                updateParams.Parameters.AddWithValue("@phone", Supplier.Phone);

                if (!string.IsNullOrEmpty(Supplier.Email))
                {
                    updates.Add("Email = @email");
                    updateParams.Parameters.AddWithValue("@email", Supplier.Email);
                }
                if (!string.IsNullOrEmpty(Supplier.Address))
                {
                    updates.Add("Address = @address");
                    updateParams.Parameters.AddWithValue("@address", Supplier.Address);
                }

                if (updates.Count > 0)
                {
                    string query = $@"UPDATE Supplier SET 
                                    {string.Join(", ", updates)}
                                    WHERE SupplierId = @id";

                    using (var cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@id", Supplier.SupplierId);

                        foreach (SqlParameter param in updateParams.Parameters)
                        {
                            cmd.Parameters.Add(new SqlParameter(param.ParameterName, param.Value));
                        }

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return RedirectToPage("/Staff/Supplier");
        }
    }
}