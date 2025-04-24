using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("ROOMS")]
public partial class Room
{
    [Key]
    public int RoomId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? RoomName { get; set; }

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

    public int? StoreId { get; set; }

    [Column("ROOM_NO")]
    [StringLength(15)]
    [Unicode(false)]
    public string? RoomNo { get; set; }

    [InverseProperty("Room")]
    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

    [InverseProperty("Room")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    [ForeignKey("StoreId")]
    [InverseProperty("Rooms")]
    public virtual Store? Store { get; set; }

    [InverseProperty("FromRoom")]
    public virtual ICollection<StoreMovement> StoreMovements { get; set; } = new List<StoreMovement>();
}
