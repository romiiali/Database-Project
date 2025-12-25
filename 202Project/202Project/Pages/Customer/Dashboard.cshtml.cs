// Pages/Customer/Dashboard.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient; // Changed from EntityFrameworkCore
using System.Data;

namespace _202Project.Pages.Customer // Fixed namespace
{
    public class DashboardModel : PageModel
    {
        static string connectionString = "Data Source=localhost;Initial Catalog=PharmacyDatabase;Integrated Security=True;Trust Server Certificate=True";


        public List<MedicineItem> Medicines { get; set; } = new List<MedicineItem>();
        public List<CategoryItem> Categories { get; set; } = new List<CategoryItem>();

        public int TotalMedicines { get; set; }
        public int TotalCategories { get; set; }
        public int InStockCount { get; set; }

        public void OnGet(string searchTerm = "", string category = "", string sortBy = "name")
        {
            // Get categories
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();

                // Get categories
                var categoryCmd = new SqlCommand("SELECT CategoryID, CategoryName FROM Category", con);
                using (var reader = categoryCmd.ExecuteReader())
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

                // Build medicine query
                string query = @"
                    SELECT m.MedicineID, m.CommercialName, m.ScientificName, 
                           m.Price, c.CategoryName, m.RecorderQuantity,
                           COALESCE(SUM(b.QuantityRecieved), 0) as Stock
                    FROM Medicine m
                    LEFT JOIN Category c ON m.CategoryID = c.CategoryID
                    LEFT JOIN Batch b ON m.MedicineID = b.MedicineID 
                        AND b.ExpiryDate > GETDATE() 
                        AND b.QuantityRecieved > 0
                    WHERE (@searchTerm = '' OR 
                          m.CommercialName LIKE '%' + @searchTerm + '%' OR
                          m.ScientificName LIKE '%' + @searchTerm + '%')
                    AND (@category = 'all' OR @category = '' OR c.CategoryName = @category)
                    GROUP BY m.MedicineID, m.CommercialName, m.ScientificName, 
                             m.Price, c.CategoryName, m.RecorderQuantity";

                // Add ordering based on sortBy
                query += sortBy switch
                {
                    "price_low" => " ORDER BY m.Price ASC",
                    "price_high" => " ORDER BY m.Price DESC",
                    _ => " ORDER BY m.CommercialName ASC"
                };

                var cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@searchTerm", searchTerm ?? "");
                cmd.Parameters.AddWithValue("@category", category ?? "");

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Medicines.Add(new MedicineItem
                        {
                            MedicineID = reader.GetInt32(0),
                            CommercialName = reader.GetString(1),
                            ScientificName = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            CategoryName = reader.GetString(4),
                            RecorderQuantity = reader.GetInt32(5),
                            Stock = reader.GetInt32(6)
                        });
                    }
                }

                // Calculate stats
                TotalMedicines = Medicines.Count;
                TotalCategories = Categories.Count;
                InStockCount = Medicines.Count(m => m.Stock > 0);
            }
        }

        public class MedicineItem
        {
            public int MedicineID { get; set; }
            public string CommercialName { get; set; }
            public string ScientificName { get; set; }
            public decimal Price { get; set; }
            public string CategoryName { get; set; }
            public int RecorderQuantity { get; set; }
            public int Stock { get; set; }
        }

        public class CategoryItem
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
        }
    }
}