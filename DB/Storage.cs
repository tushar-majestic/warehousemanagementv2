using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("Storage")]
public partial class Storage
{
    [Key]
    public int StorageId { get; set; }

    public int ItemId { get; set; }

    public int StoreId { get; set; }

    [StringLength(50)]
    public string ShelfNumber { get; set; } = null!;

    public int AvailableQuantity { get; set; }

    public int? RoomId { get; set; }

    public int? ShelfId { get; set; }

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

    [Column("Expiry_date", TypeName = "datetime")]
    public DateTime? ExpiryDate { get; set; }

    [Column("supplyID")]
    public int? SupplyId { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("Storages")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("RoomId")]
    [InverseProperty("Storages")]
    public virtual Room? Room { get; set; }

    [ForeignKey("ShelfId")]
    [InverseProperty("Storages")]
    public virtual Shelf? Shelf { get; set; }

    [ForeignKey("StoreId")]
    [InverseProperty("Storages")]
    public virtual Store Store { get; set; } = null!;

    [ForeignKey("SupplyId")]
    [InverseProperty("Storages")]
    public virtual Supply? Supply { get; set; }
}
