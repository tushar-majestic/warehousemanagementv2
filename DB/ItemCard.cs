using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace LabMaterials.DB
{
    public class ItemCard
    {
        [Key]
        public int Id { get; set; }

        [StringLength(10)]
        [ForeignKey(nameof(ItemGroup))]
        public string GroupCode { get; set; } = null!;

        public virtual ItemGroup ItemGroup { get; set; } = null!;


        [StringLength(15)]
        [ForeignKey(nameof(ItemType))]
        public string ItemTypeCode { get; set; } = null!;
        public virtual ItemType ItemType { get; set; } = null!;

        [StringLength(50)]
        public string ItemCode { get; set; } = null!;

        [StringLength(200)]
        public string ItemName { get; set; } = null!;
        [StringLength(200)]
        public string? ItemDescription { get; set; }
        [StringLength(20)]
        public string? UnitOfmeasure { get; set; }
        [StringLength(20)]
        public string? Chemical { get; set; }
        [StringLength(50)]
        [ForeignKey(nameof(HazardType))]
        public string? HazardTypeName { get; set; }
        public virtual HazardType HazardType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpiryDate { get; set; }
        public enum AssetType
        {
            Sustainable,
            Consumable
        }

        public AssetType TypeOfAsset { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum must be positive ")]
        public int Minimum { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Minimum must be positive ")]
        public int ReorderLimit { get; set; }

        public int Ceiling => Minimum + ReorderLimit;
        public int StoreId { get; set; }  // Foreign key
        public virtual Store Store { get; set; }  // Navigation property

        public int QuantityReceived { get; set; }

        public DateOnly DateOfEntry { get; set; }
        public int RoomId { get; set; }  // Foreign key

        public virtual Room Room { get; set; }  // Navigation property
        public int ShelfId { get; set; }  // Foreign key
        public virtual Shelf Shelf { get; set; }  // Navigation property
        public int SupplierId { get; set; }  // Foreign key
        public virtual Supplier Supplier { get; set; }  // Navigation property

        public enum Receiptype
        {
            [Display(Name = "Destruction Request")]
            DestructionRequest,

            [Display(Name = "Certification")]
            Certification,

            [Display(Name = "Purchase Order")]
            PurchaseOrder
        }

        public Receiptype DocumentType { get; set; }
        public int ReceiptDocumentnumber { get; set; }
        public int ItemId { get; set; }  // Foreign key
        public virtual Item Item { get; set; }  // Navigation property

        public int TotalAvailable => (Item?.AvailableQuantity ?? 0) + QuantityReceived;

    }
}
