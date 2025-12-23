using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Data;
using PharmacySystem.Models;

namespace PharmacySystem.Pages.Staff.Batch
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Batch> BatchList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Batches != null)
            {
                // Include Medicine and Supplier so we can see their names, not just IDs
                BatchList = await _context.Batches
                .Include(b => b.Medicine)
                .Include(b => b.Supplier)
                .ToListAsync();
            }
        }
    }
}