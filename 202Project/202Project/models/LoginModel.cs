using System.ComponentModel.DataAnnotations;

namespace PharmacyLogin.Models
{
    public class LoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string UserType { get; set; } = "Customer"; // "Customer" or "Staff"
    }
}