using _202Project.models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; }

        // Navigation Properties
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<InventoryTransactionType> InventoryTransactions { get; set; }
        public virtual ICollection<SalesTransaction> SalesTransactions { get; set; }
    }
}