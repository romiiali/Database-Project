using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.Models
{
    public class Batch
    {
        [Key]
        public int BatchID { get; set; }

        public int BatchNumber { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ArrivalDate { get; set; }

        public int QuantityRecieved { get; set; }

        // Relationship to Medicine
        public int MedicineID { get; set; }
        public Medicine? Medicine { get; set; }

        // Relationship to Supplier
        public int SupplierID { get; set; }
        public Supplier? Supplier { get; set; }
    }
}