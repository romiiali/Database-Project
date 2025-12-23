using _202Project.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class PurchaseOrder
    {
        [Key]
        public int PurchaseID { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public DateTime ExpectedDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Foreign Keys
        public int SupplierID { get; set; }
        public int StaffID { get; set; }

        // Navigation Properties
        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; }

        [ForeignKey("StaffID")]
        public virtual Staff Staff { get; set; }

        public virtual ICollection<InventoryTransactionType> InventoryTransactions { get; set; }
    }
}
