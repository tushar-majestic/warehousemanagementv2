using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("ItemType")]
public partial class ItemType
{
    [Key]
    [StringLength(15)]
    public string ItemTypeCode { get; set; } = null!;

    [StringLength(100)]
    public string TypeName { get; set; } = null!;

    [InverseProperty("ItemTypeCodeNavigation")]
    public virtual ICollection<ItemCard> ItemCards { get; set; } = new List<ItemCard>();

    [InverseProperty("ItemTypeCodeNavigation")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}
