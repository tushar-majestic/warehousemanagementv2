using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class Unit
{
    [StringLength(20)]
    public string UnitCode { get; set; } = null!;

    [StringLength(50)]
    public string UnitDesc { get; set; } = null!;

    [StringLength(10)]
    public string GroupCode { get; set; } = null!;

    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [ForeignKey("GroupCode")]
    [InverseProperty("Units")]
    public virtual ItemGroup GroupCodeNavigation { get; set; } = null!;

    [InverseProperty("Unit")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
