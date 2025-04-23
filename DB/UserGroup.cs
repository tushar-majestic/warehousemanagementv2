using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class UserGroup
{
    [Key]
    [Column("UserGroupID")]
    public int UserGroupId { get; set; }

    [StringLength(50)]
    public string? UserGroupName { get; set; }

    [InverseProperty("UserGroup")]
    public virtual ICollection<UserGroupPrivilege> UserGroupPrivileges { get; set; } = new List<UserGroupPrivilege>();

    [InverseProperty("UserGroup")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
