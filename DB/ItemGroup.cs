using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("ItemGroup")]
public partial class ItemGroup
{
    [Key]
    [StringLength(10)]
    public string GroupCode { get; set; } = null!;

    [StringLength(50)]
    public string GroupDesc { get; set; } = null!;

    [InverseProperty("GroupCodeNavigation")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [InverseProperty("GroupCodeNavigation")]
    public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();
}
