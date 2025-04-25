using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("Store")]
public partial class Store
{
    [Key]
    public int StoreId { get; set; }

    [StringLength(50)]
    public string StoreNumber { get; set; } = null!;

    [StringLength(50)]
    public string StoreName { get; set; } = null!;

    [StringLength(4000)]
    public string ShelfNumbers { get; set; } = null!;

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

    [Column("isActive")]
    public int? IsActive { get; set; }

    [Column("IN_Store")]
    public int? InStore { get; set; }

    [StringLength(20)]
    public string? StoreType { get; set; }

    [StringLength(10)]
    public string? BuildingNumber { get; set; }

    [InverseProperty("Store")]
    public virtual ICollection<DisbursementRequest> DisbursementRequests { get; set; } = new List<DisbursementRequest>();

    [InverseProperty("Store")]
    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    [InverseProperty("Store")]
    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();

    [InverseProperty("Store")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    [InverseProperty("FromStore")]
    public virtual ICollection<StoreMovement> StoreMovementFromStores { get; set; } = new List<StoreMovement>();

    [InverseProperty("ToStore")]
    public virtual ICollection<StoreMovement> StoreMovementToStores { get; set; } = new List<StoreMovement>();
}
