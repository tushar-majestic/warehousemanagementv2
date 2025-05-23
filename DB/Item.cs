using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("Item")]
public partial class Item
{
    [Key]
    public int ItemId { get; set; }

    [StringLength(10)]
    public string GroupCode { get; set; } = null!;

    [StringLength(15)]
    public string? ItemTypeCode { get; set; } = null;

    [StringLength(50)]
    public string ItemCode { get; set; } = null!;

    [StringLength(200)]
    public string ItemName { get; set; } = null!;
    [StringLength(200)]
    public string? StateofMatter { get; set; } 

    [StringLength(200)]
    public string? ItemNameAr { get; set; }     

    [Column("UnitID")]
    public int UnitId { get; set; }

    public bool? IsHazardous { get; set; }
    public bool? Chemical { get; set; } 

    [StringLength(50)]
    public string? HazardTypeName { get; set; }
    [StringLength(100)]
    public string? RiskRating { get; set; }

    public int AvailableQuantity { get; set; }

    [StringLength(200)]
    public string? ItemDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ended { get; set; }

    [Column("EndedByID")]
    public int? EndedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Updated { get; set; }

    [Column("UpdatedByID")]
    public int? UpdatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Created { get; set; }

    [Column("CreatedByID")]
    public int? CreatedById { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? BatchNo { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ExpiryDate { get; set; }

    [Column("SupplyID")]
    public int? SupplyId { get; set; }

    [InverseProperty("Item")]
    public virtual ICollection<ColorCode> ColorCodes { get; set; } = new List<ColorCode>();

    [InverseProperty("Item")]
    public virtual ICollection<DamagedItem> DamagedItems { get; set; } = new List<DamagedItem>();

    [ForeignKey("GroupCode")]
    [InverseProperty("Items")]
    public virtual ItemGroup GroupCodeNavigation { get; set; } = null!;

    [ForeignKey("HazardTypeName")]
    [InverseProperty("Items")]
    public virtual HazardType? HazardTypeNameNavigation { get; set; }

    [InverseProperty("Item")]
    public virtual ICollection<ItemCard> ItemCards { get; set; } = new List<ItemCard>();

    [ForeignKey("ItemTypeCode")]
    [InverseProperty("Items")]
    public virtual ItemType ItemTypeCodeNavigation { get; set; } = null!;

    [InverseProperty("Item")]
    public virtual ICollection<ReceivingItem> ReceivingItems { get; set; } = new List<ReceivingItem>();

    [InverseProperty("Item")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    [InverseProperty("Item")]
    public virtual ICollection<StoreMovement> StoreMovements { get; set; } = new List<StoreMovement>();

    [ForeignKey("SupplyId")]
    [InverseProperty("Items")]
    public virtual Supplier? Supply { get; set; }

    [ForeignKey("UnitId")]
    [InverseProperty("Items")]
    public virtual Unit Unit { get; set; } = null!;
}
