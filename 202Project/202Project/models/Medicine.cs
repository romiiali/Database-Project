using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineID { get; set; }

        [Required]
        public string CommercialName { get; set; } 

        public string ScientificName { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string Dosage { get; set; }

        public int ReorderQuantity { get; set; } 

        public int CategoryID { get; set; }
    }
}