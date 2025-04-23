using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("ActivityLog")]
public partial class ActivityLog
{
    [Key]
    [Column("ActivityLogID")]
    public int ActivityLogId { get; set; }

    [Column("UserID")]
    public int UserId { get; set; }

    [Column("SourceIP")]
    [StringLength(25)]
    [Unicode(false)]
    public string SourceIp { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string Type { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ActivityTime { get; set; }

    [StringLength(1024)]
    public string Description { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("ActivityLogs")]
    public virtual User User { get; set; } = null!;
}
