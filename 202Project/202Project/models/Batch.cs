using _202Project.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class Batch
    {
        [Key]
        public int BatchID { get; set; }

        [Required]
        [StringLength(50)]
        public string BatchNumber { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int QuantityReceived { get; set; }

        [Required]
        public DateTime ArrivalDate { get; set; }

        // Foreign Keys
        public int MedicineID { get; set; }
        public int SupplierID { get; set; }

        // Navigation Properties
        [ForeignKey("MedicineID")]
        public virtual Medicine Medicine { get; set; }

        [ForeignKey("SupplierID")]
        public virtual Supplier Supplier { get; set; }

        public virtual ICollection<StockAlert> StockAlerts { get; set; }
        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }
}