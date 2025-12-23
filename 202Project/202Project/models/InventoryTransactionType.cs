using _202Project.models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class InventoryTransactionType
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        public int Quantity { get; set; }

        public int? ReferenceID { get; set; }

        // Foreign Keys
        public int? PurchaseID { get; set; }
        public int StaffID { get; set; }

        // Navigation Properties
        [ForeignKey("PurchaseID")]
        public virtual PurchaseOrder PurchaseOrder { get; set; }

        [ForeignKey("StaffID")]
        public virtual Staff Staff { get; set; }

        public virtual StockAlert StockAlert { get; set; }
    }
}