using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("HazardType")]
public partial class HazardType
{
    [Key]
    [StringLength(50)]
    public string HazardTypeName { get; set; } = null!;

    [InverseProperty("HazardTypeNameNavigation")]
    public virtual ICollection<ItemCard> ItemCards { get; set; } = new List<ItemCard>();

    [InverseProperty("HazardTypeNameNavigation")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
