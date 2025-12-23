using _202Project.models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class SalesTransaction
    {
        [Key]
        public int SaleID { get; set; }

        [Required]
        public DateTime SaleTime { get; set; }

        [Required]
        [StringLength(50)]
        public string SaleStatus { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Foreign Keys
        public int CustomerID { get; set; }
        public int StaffID { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerID")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("StaffID")]
        public virtual Staff Staff { get; set; }

        public virtual ICollection<SaleItem> SaleItems { get; set; }
    }
}