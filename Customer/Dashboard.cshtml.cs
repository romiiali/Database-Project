// Pages/Customer/Dashboard.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using user_pages.Data;
using user_pages.Models;

namespace user_pages.Pages.Customer
{
    public class DashboardModel : PageModel
    {
        private readonly AppDbContext _context;

        public DashboardModel(AppDbContext context)
        {
            _context = context;
        }

        public List<Medicine> Medicines { get; set; } = new List<Medicine>();
        public List<Category> Categories { get; set; } = new List<Category>();

        public int TotalMedicines { get; set; }
        public int TotalCategories { get; set; }
        public int InStockCount { get; set; }

        public async Task OnGetAsync(string searchTerm = "", string category = "", string sortBy = "name")
        {
            // Get all categories
            Categories = await _context.Categories.ToListAsync();

            // Start with all medicines
            var query = _context.Medicines.AsQueryable();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(m =>
                    m.CommercialName.Contains(searchTerm) ||
                    m.ScientificName.Contains(searchTerm));
            }

            // Apply category filter
            if (!string.IsNullOrEmpty(category) && category != "all")
            {
                query = query.Where(m => m.CategoryName == category);
            }

            // Apply sorting
            query = sortBy switch
            {
                "price_low" => query.OrderBy(m => m.Price),
                "price_high" => query.OrderByDescending(m => m.Price),
                _ => query.OrderBy(m => m.CommercialName)
            };

            Medicines = await query.ToListAsync();

            // Calculate stats
            TotalMedicines = Medicines.Count;
            TotalCategories = Categories.Count;
            InStockCount = Medicines.Count;
        }
    }
}