using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("CurrentApproverUserId", Name = "IX_MaterialRequests_CurrentApproverUserId")]
[Index("RequestedByUserId", Name = "IX_MaterialRequests_RequestedByUserId")]
public partial class MaterialRequest
{
    [Key]
    public int RequestId { get; set; }

    [StringLength(100)]
    public string MaterialName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime RequestedDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = null!;

    public int RequestedByUserId { get; set; }

    public int? CurrentApproverUserId { get; set; }

    [ForeignKey("CurrentApproverUserId")]
    [InverseProperty("MaterialRequestCurrentApproverUsers")]
    public virtual User? CurrentApproverUser { get; set; }

    [ForeignKey("RequestedByUserId")]
    [InverseProperty("MaterialRequestRequestedByUsers")]
    public virtual User RequestedByUser { get; set; } = null!;
}
