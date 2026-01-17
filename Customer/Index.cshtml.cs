using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Data;
using PharmacySystem.Models;

namespace PharmacySystem.Pages.Customer
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Customer> CustomerList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Customers != null)
            {
                CustomerList = await _context.Customers.ToListAsync();
            }
        }
    }
}