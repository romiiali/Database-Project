using _202Project.models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class StockAlert
    {
        [Key]
        public int AlertID { get; set; }

        [Required]
        [StringLength(50)]
        public string AlertType { get; set; }

        [Required]
        public DateTime AlertDateTime { get; set; }

        public bool Resolved { get; set; }

        public int CurrentValueAtAlert { get; set; }

        public int ThresholdValue { get; set; }

        // Foreign Keys
        public int BatchID { get; set; }
        public int MedicineID { get; set; }
        public int TransactionID { get; set; }

        // Navigation Properties
        [ForeignKey("BatchID")]
        public virtual Batch Batch { get; set; }

        [ForeignKey("MedicineID")]
        public virtual Medicine Medicine { get; set; }

        [ForeignKey("TransactionID")]
        public virtual InventoryTransactionType InventoryTransaction { get; set; }
    }
}