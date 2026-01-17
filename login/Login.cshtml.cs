using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PharmacyLogin.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginInputModel Input { get; set; } = new LoginInputModel();

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Simple check to ensure app works without database
            if (Input.Email == "admin@pharma.com" && Input.Password == "admin123")
            {
                return RedirectToPage("/Index");
            }

            ViewData["Message"] = "Invalid Email or Password!";
            return Page();
        }

        public class LoginInputModel
        {
            public string Email { get; set; } = "";
            public string Password { get; set; } = "";
        }
    }
}