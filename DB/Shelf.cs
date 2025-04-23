using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("SHELVES")]
public partial class Shelf
{
    [Key]
    public int ShelfId { get; set; }

    public int? RoomId { get; set; }

    public int? StoreId { get; set; }

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

    [Column("SHELF_NO")]
    [StringLength(15)]
    [Unicode(false)]
    public string? ShelfNo { get; set; }

    [ForeignKey("RoomId")]
    [InverseProperty("Shelves")]
    public virtual Room? Room { get; set; }

    [InverseProperty("Shelf")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    [ForeignKey("StoreId")]
    [InverseProperty("Shelves")]
    public virtual Store? Store { get; set; }

    [InverseProperty("FromShelf")]
    public virtual ICollection<StoreMovement> StoreMovements { get; set; } = new List<StoreMovement>();
}
