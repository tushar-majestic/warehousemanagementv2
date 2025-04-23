using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("DAMAGED_ITEMS")]
public partial class DamagedItem
{
    [Key]
    [Column("DamagedID")]
    public int DamagedId { get; set; }

    public int? ItemId { get; set; }

    public int? DamagedQuantity { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DamagedReason { get; set; }

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

    [Column(TypeName = "datetime")]
    public DateTime? DamagedDate { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("DamagedItems")]
    public virtual Item? Item { get; set; }
}
