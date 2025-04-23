using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class Destination
{
    [Key]
    [Column("D_ID")]
    public int DId { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? DestinationName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? DestinationAddress { get; set; }

    public int? IsActive { get; set; }

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

    [InverseProperty("Destination")]
    public virtual ICollection<Requester> Requesters { get; set; } = new List<Requester>();
}
