using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [Required]
        [StringLength(200)]
        public string ScientificName { get; set; }

        [Required]
        [StringLength(200)]
        public string CommercialName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(100)]
        public string Dosage { get; set; }

        public int RecorderQuantity { get; set; }

        public int CategoryID { get; set; }

        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Batch> Batches { get; set; }
        public virtual ICollection<StockAlert> StockAlerts { get; set; }
    }
}