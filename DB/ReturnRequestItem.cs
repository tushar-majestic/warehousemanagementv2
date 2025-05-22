using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMaterials.DB
{
    public class ReturnRequestItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ReturnRequestId { get; set; }

        [ForeignKey("ReturnRequestId")]
        public virtual ReturnRequest ReturnRequest { get; set; } = null!;

        [Required]
        public int ItemCardId { get; set; }

        [ForeignKey("ItemCardId")]
        public virtual ItemCard ItemCard { get; set; } = null!;

        [Required]
        [StringLength(20)]
        public string ItemGroup { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string ItemCode { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string ItemNameArabic { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string ItemNameEnglish { get; set; } = null!;

        [StringLength(500)]
        public string? ItemDescription { get; set; }

        [StringLength(100)]
        public string? TypeOfContract { get; set; }

        [StringLength(100)]
        public string? Chemical { get; set; }

        [StringLength(100)]
        public string? RiskRating { get; set; }

        [Required]
        [StringLength(100)]
        public string StateOfMatter { get; set; } = null!;

        // public string? RecommendedAction { get; set; } 
        [StringLength(500)]
        public string? InspectionNotes { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(20)]
        public string UnitOfMeasure { get; set; } = null!;

        [Required]
        public int ReturnedQuantity { get; set; }

        [StringLength(500)]
        public string? ReturnNotes { get; set; }

        public enum ItemCondition
        {
            New,
            Used,
            Recyclable,
            Other
        }

        public ItemCondition RecommendedAction { get; set; } = ItemCondition.Used;

        [Column(TypeName = "text")]
        public string? Notes { get; set; }

    }

}
