using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LName { get; set; }

       
        public int? Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        // Navigation Properties
        public virtual ICollection<SalesTransaction>? SalesTransactions { get; set; }
    }
}