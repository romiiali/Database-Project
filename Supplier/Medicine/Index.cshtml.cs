using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Data;
using PharmacySystem.Models;

namespace PharmacySystem.Pages.Staff.Medicine
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Medicine> MedicineList { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Medicines != null)
            {
                MedicineList = await _context.Medicines.ToListAsync();
            }
        }
    }
}