using System.ComponentModel.DataAnnotations;

namespace _202Project.models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        public string SupplierName { get; set; }

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? Address { get; set; }
    }
}