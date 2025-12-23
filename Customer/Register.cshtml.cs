// Pages/Customer/Register.cshtml.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using user_pages.Data;
using user_pages.Models;

namespace user_pages.Pages.Customer
{
    public class RegisterModel : PageModel
    {
        private readonly AppDbContext _context;

        public RegisterModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CustomerInputModel CustomerInput { get; set; } = new CustomerInputModel();

        [TempData]
        public string? SuccessMessage { get; set; }

        [TempData]
        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var customer = new Customer
                {
                    FName = CustomerInput.FName,
                    LName = CustomerInput.LName,
                    Phone = CustomerInput.Phone,
                    Email = string.IsNullOrEmpty(CustomerInput.Email) ? null : CustomerInput.Email
                };

                _context.Customers.Add(customer);
                _context.SaveChanges();

                SuccessMessage = $"Registration successful! Your customer ID is: {customer.CustomerID}";
                return RedirectToPage("/Customer/Dashboard");
            }
            catch (Exception)
            {
                ErrorMessage = "An error occurred during registration. Please try again.";
                return Page();
            }
        }

        public class CustomerInputModel
        {
            public string FName { get; set; } = string.Empty;
            public string LName { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string? Email { get; set; }
        }
    }
}