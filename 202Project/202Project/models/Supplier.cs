using _202Project.models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required]
        [StringLength(200)]
        public string SupplierName { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(500)]
        public string Address { get; set; }

        // Navigation Properties
        public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; }
        public virtual ICollection<Batch> Batches { get; set; }
    }
}