using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace _202Project.Pages.Staff
{
    public class SupplierModel : PageModel
    {
        public class SupplierItem
        {
            public int SupplierId { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Phone { get; set; }
            public string Email { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
        }

        public List<SupplierItem> Suppliers { get; set; } = new List<SupplierItem>();

        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;TrustServerCertificate=True";

        public void OnGet()
        {
            using (var con = new SqlConnection(connectionString))
            {
                string queryString = "SELECT SupplierId, SupplierName, Phone, Email, Address FROM Supplier ORDER BY SupplierName";
                con.Open();
                SqlCommand cmd = new SqlCommand(queryString, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var supplier = new SupplierItem
                    {
                        SupplierId = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Phone = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        Email = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                        Address = reader.IsDBNull(4) ? string.Empty : reader.GetString(4)
                    };
                    Suppliers.Add(supplier);
                }
                reader.Close();
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                string querystring = @"
                    BEGIN TRANSACTION;
                    BEGIN TRY
                        UPDATE Batch SET SupplierId = NULL WHERE SupplierId = @id;
                        UPDATE PurchaseOrder SET SupplierId = NULL WHERE SupplierId = @id;
                        DELETE FROM Supplier WHERE SupplierId = @id;
                        COMMIT TRANSACTION;
                    END TRY
                    BEGIN CATCH
                        ROLLBACK TRANSACTION;
                        THROW;
                    END CATCH";

                using (var cmd = new SqlCommand(querystring, con))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToPage();
        }
    }
}