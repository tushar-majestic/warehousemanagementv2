using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class Privilege
{
    [Key]
    public int PrivilegeId { get; set; }

    [StringLength(50)]
    public string? PrivilegeName { get; set; }

    [InverseProperty("Privilege")]
    public virtual ICollection<UserGroupPrivilege> UserGroupPrivileges { get; set; } = new List<UserGroupPrivilege>();
}
