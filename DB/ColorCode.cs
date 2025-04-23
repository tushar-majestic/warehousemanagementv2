using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class ColorCode
{
    [Key]
    [Column("CODE_ID")]
    public int CodeId { get; set; }

    [Column("COLOR_NAME")]
    [StringLength(250)]
    [Unicode(false)]
    public string? ColorName { get; set; }

    [Column("COLOR_CODE")]
    [StringLength(250)]
    [Unicode(false)]
    public string? ColorCode1 { get; set; }

    [Column("ITEM_CODE")]
    [StringLength(250)]
    [Unicode(false)]
    public string? ItemCode { get; set; }

    [Column("ITEM_ID")]
    public int? ItemId { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("ColorCodes")]
    public virtual Item? Item { get; set; }
}
