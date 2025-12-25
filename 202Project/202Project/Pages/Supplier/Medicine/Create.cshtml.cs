// Pages/Staff/Medicine/Create.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;

namespace _202Project.Pages.Staff.Medicine
{
    public class CreateModel : PageModel
    {
        static string connectionString = "Data Source=localhost; Initial Catalog=PharmacyDatabase; Integrated Security=True; TrustServerCertificate=True";
        SqlConnection con = new SqlConnection(connectionString);

        [BindProperty]
        public MedicineInputModel Medicine { get; set; } = new MedicineInputModel();

        public List<CategoryItem> Categories { get; set; } = new List<CategoryItem>();

        public IActionResult OnGet()
        {
            // Load categories for dropdown if needed
            con.Open();
            string query = "SELECT CategoryID, CategoryName FROM Category";
            var cmd = new SqlCommand(query, con);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Categories.Add(new CategoryItem
                    {
                        CategoryID = reader.GetInt32(0),
                        CategoryName = reader.GetString(1)
                    });
                }
            }
            con.Close();

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
                    INSERT INTO Medicine (CommercialName, ScientificName, Price, Dosage, CategoryID) 
                    VALUES (@commercialName, @scientificName, @price, @dosage, @categoryId)";

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@commercialName", Medicine.CommercialName);
                cmd.Parameters.AddWithValue("@scientificName", Medicine.ScientificName);
                cmd.Parameters.AddWithValue("@price", Medicine.Price);
                cmd.Parameters.AddWithValue("@dosage", Medicine.Dosage ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@categoryId", Medicine.CategoryID);

                cmd.ExecuteNonQuery();
                con.Close();

                return RedirectToPage("./Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error saving medicine: {ex.Message}");
                return Page();
            }
        }

        public class MedicineInputModel
        {
            public string CommercialName { get; set; } = string.Empty;
            public string ScientificName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string? Dosage { get; set; }
            public int CategoryID { get; set; }
        }

        public class CategoryItem
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
        }
    }
}