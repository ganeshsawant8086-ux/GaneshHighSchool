using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganesh1.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Material / Item Name")]
        [StringLength(150)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [StringLength(80)]
        public string Category { get; set; } = "Stationery";

        [Required]
        [Display(Name = "Item Code / SKU")]
        [StringLength(50)]
        public string ItemCode { get; set; } = string.Empty;

        [Required]
        [Range(0, 100000)]
        public int Quantity { get; set; } = 0;

        [Display(Name = "Minimum Threshold")]
        public int MinThreshold { get; set; } = 10;

        [Required]
        [StringLength(30)]
        public string Unit { get; set; } = "Units";

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Unit Price (₹)")]
        public decimal UnitPrice { get; set; } = 0.00m;

        [Required]
        [StringLength(100)]
        [Display(Name = "Storage Location")]
        public string Location { get; set; } = "Central Store Room";

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "In Stock";

        [StringLength(150)]
        public string? Supplier { get; set; }

        [Display(Name = "Last Restocked")]
        public DateTime LastRestockedDate { get; set; } = DateTime.Now;

        [StringLength(250)]
        public string? Remarks { get; set; }

        [NotMapped]
        public decimal TotalValue => Quantity * UnitPrice;

        [NotMapped]
        public bool IsLowStock => Quantity <= MinThreshold;
    }
}
