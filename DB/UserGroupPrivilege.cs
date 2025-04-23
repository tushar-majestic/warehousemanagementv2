using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("UserGroup_Privileges")]
public partial class UserGroupPrivilege
{
    [Column("UserGroupID")]
    public int UserGroupId { get; set; }

    [Column("PrivilegeID")]
    public int PrivilegeId { get; set; }

    [Key]
    [Column("UserGroupPrivilageID")]
    public int UserGroupPrivilageId { get; set; }

    [ForeignKey("PrivilegeId")]
    [InverseProperty("UserGroupPrivileges")]
    public virtual Privilege Privilege { get; set; } = null!;

    [ForeignKey("UserGroupId")]
    [InverseProperty("UserGroupPrivileges")]
    public virtual UserGroup UserGroup { get; set; } = null!;
}
