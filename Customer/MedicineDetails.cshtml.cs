using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using user_pages.Data;
using user_pages.Models;

namespace user_pages.Pages.Customer
{
    public class MedicineDetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public MedicineDetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public Medicine? Medicine { get; set; }
        public List<Batch> AvailableBatches { get; set; } = new List<Batch>();
        public List<Medicine> RelatedMedicines { get; set; } = new List<Medicine>();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Get medicine
            Medicine = await _context.Medicines
                .FirstOrDefaultAsync(m => m.MedicineID == id);

            if (Medicine == null)
            {
                return NotFound();
            }

            // Get batches for this medicine (only non-expired)
            AvailableBatches = await _context.Batches
                .Where(b => b.MedicineID == id &&
                           b.QuantityReceived > 0 &&
                           b.ExpiryDate > DateTime.Now)
                .ToListAsync();

            // Get related medicines (same category)
            if (!string.IsNullOrEmpty(Medicine.CategoryName))
            {
                RelatedMedicines = await _context.Medicines
                    .Where(m => m.CategoryName == Medicine.CategoryName &&
                               m.MedicineID != id)
                    .Take(4)
                    .ToListAsync();
            }

            return Page();
        }
    }
}