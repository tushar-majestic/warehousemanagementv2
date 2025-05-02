using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("ItemId", Name = "IX_ReceivingItems_ItemId")]
[Index("ReceivingReportId", Name = "IX_ReceivingItems_ReceivingReportId")]
public partial class ReceivingItem
{
    [Key]
    public int Id { get; set; }

    public int ReceivingReportId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("ReceivingItems")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("ReceivingReportId")]
    [InverseProperty("ReceivingItems")]
    public virtual ReceivingReport ReceivingReport { get; set; } = null!;
}
