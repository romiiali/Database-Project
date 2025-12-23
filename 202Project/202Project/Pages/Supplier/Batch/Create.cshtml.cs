using System; 
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using PharmacySystem.Data;
using PharmacySystem.Models;

namespace PharmacySystem.Pages.Staff.Batch
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["MedicineID"] = new SelectList(_context.Medicines, "MedicineID", "CommercialName");
            ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
            return Page();
        }

        [BindProperty]
        public PharmacySystem.Models.Batch Batch { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                ViewData["MedicineID"] = new SelectList(_context.Medicines, "MedicineID", "CommercialName");
                ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
                return Page();
            }

            if (Batch.ExpiryDate <= Batch.ArrivalDate)
            {
                ModelState.AddModelError("Batch.ExpiryDate", "Error: The Expiry Date must be after the Arrival Date.");
                
                ViewData["MedicineID"] = new SelectList(_context.Medicines, "MedicineID", "CommercialName");
                ViewData["SupplierID"] = new SelectList(_context.Suppliers, "SupplierID", "SupplierName");
                return Page();
            }

            _context.Batches.Add(Batch);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}