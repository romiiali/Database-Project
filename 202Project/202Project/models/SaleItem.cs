using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class SaleItem
    {
        [Key]
        public int SaleItemID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int QuantitySold { get; set; }

        // Foreign Keys
        public int BatchID { get; set; }
        public int SaleID { get; set; }

        // Navigation Properties
        [ForeignKey("BatchID")]
        public virtual Batch Batch { get; set; }

        [ForeignKey("SaleID")]
        public virtual SalesTransaction SalesTransaction { get; set; }
    }
}