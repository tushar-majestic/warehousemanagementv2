using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("GroupCode", Name = "IX_ItemCards_GroupCode")]
[Index("HazardTypeName", Name = "IX_ItemCards_HazardTypeName")]
[Index("ItemId", Name = "IX_ItemCards_ItemId")]
[Index("ItemTypeCode", Name = "IX_ItemCards_ItemTypeCode")]
[Index("StoreId", Name = "IX_ItemCards_StoreId")]
public partial class ItemCard
{
    [Key]
    public int Id { get; set; }

    [StringLength(10)]
    public string GroupCode { get; set; } = null!;

    [StringLength(15)]
    public string ItemTypeCode { get; set; } = null!;

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
    public string? HazardTypeName { get; set; }

    public int CreatedBy { get; set; }

    public int StoreId { get; set; }

    public int QuantityAvailable { get; set; }

    // public string DocumentType { get; set; } = null!;

    // public int ReceiptDocumentnumber { get; set; }

    public int ItemId { get; set; }

    // public DateOnly? DateOfEntry { get; set; }

    [ForeignKey("GroupCode")]
    [InverseProperty("ItemCards")]
    public virtual ItemGroup GroupCodeNavigation { get; set; } = null!;

    [ForeignKey("HazardTypeName")]
    [InverseProperty("ItemCards")]
    public virtual HazardType? HazardTypeNameNavigation { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("ItemCards")]
    public virtual Item Item { get; set; } = null!;

    [InverseProperty("ItemCard")]
    public virtual ICollection<ItemCardBatch> ItemCardBatches { get; set; } = new List<ItemCardBatch>();

    [ForeignKey("ItemTypeCode")]
    [InverseProperty("ItemCards")]
    public virtual ItemType ItemTypeCodeNavigation { get; set; } = null!;

    [ForeignKey("StoreId")]
    [InverseProperty("ItemCards")]
    public virtual Store Store { get; set; } = null!;
}
