using global::_202Project.models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _202Project.models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation Properties
        public virtual ICollection<Medicine> Medicines { get; set; }
    }
}
