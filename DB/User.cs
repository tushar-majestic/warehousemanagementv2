using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(200)]
    public string FullName { get; set; } = null!;

    [MaxLength(512)]
    public byte[] Password { get; set; } = null!;

    [StringLength(50)]
    public string Email { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public bool IsActive { get; set; }

    public int FailedPasswordAttemptCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginTime { get; set; }

    public bool Locked { get; set; }

    [Column("CreatedByID")]
    public int? CreatedById { get; set; }

    public bool IsActiveDirectoryUser { get; set; }

    [Column("UserGroupID")]
    public int UserGroupId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ended { get; set; }

    [Column("EndedByID")]
    public int? EndedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Updated { get; set; }

    [Column("UpdatedByID")]
    public int? UpdatedById { get; set; }

    [Column("lang")]
    [StringLength(2)]
    public string? Lang { get; set; }

    [StringLength(50)]
    public string EmpAffiliation { get; set; } = null!;

    public int? JobNumber { get; set; }

    public int? Transfer { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();

    [InverseProperty("CurrentApproverUser")]
    public virtual ICollection<MaterialRequest> MaterialRequestCurrentApproverUsers { get; set; } = new List<MaterialRequest>();

    [InverseProperty("RequestedByUser")]
    public virtual ICollection<MaterialRequest> MaterialRequestRequestedByUsers { get; set; } = new List<MaterialRequest>();

    [ForeignKey("UserGroupId")]
    [InverseProperty("Users")]
    public virtual UserGroup UserGroup { get; set; } = null!;
}
