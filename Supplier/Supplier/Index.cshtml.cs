using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Data;
using PharmacySystem.Models;

namespace PharmacySystem.Pages.Staff.Supplier
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- THIS IS THE LINE THE HTML IS LOOKING FOR ---
        public IList<Models.Supplier> SupplierList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Suppliers != null)
            {
                SupplierList = await _context.Suppliers.ToListAsync();
            }
        }
    }
}