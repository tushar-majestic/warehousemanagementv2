using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("STORE_MOVEMENT")]
public partial class StoreMovement
{
    [Key]
    [Column("StoreMovementID")]
    public int StoreMovementId { get; set; }

    [Column("FromStoreID")]
    public int? FromStoreId { get; set; }

    [Column("FromRoomID")]
    public int? FromRoomId { get; set; }

    [Column("FromShelfID")]
    public int? FromShelfId { get; set; }

    [Column("ToStoreID")]
    public int? ToStoreId { get; set; }

    public int? ItemId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ItemCode { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ItemType { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ItemQuantity { get; set; }

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

    [ForeignKey("FromRoomId")]
    [InverseProperty("StoreMovements")]
    public virtual Room? FromRoom { get; set; }

    [ForeignKey("FromShelfId")]
    [InverseProperty("StoreMovements")]
    public virtual Shelf? FromShelf { get; set; }

    [ForeignKey("FromStoreId")]
    [InverseProperty("StoreMovementFromStores")]
    public virtual Store? FromStore { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("StoreMovements")]
    public virtual Item? Item { get; set; }

    [ForeignKey("ToStoreId")]
    [InverseProperty("StoreMovementToStores")]
    public virtual Store? ToStore { get; set; }
}
