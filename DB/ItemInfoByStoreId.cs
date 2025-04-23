using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Keyless]
[Table("ItemInfoByStoreID")]
public partial class ItemInfoByStoreId
{
    public int? ItemId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? ItemName { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? ItemCode { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? ItemTypeCode { get; set; }

    public int? AvailableQuantity { get; set; }
}
