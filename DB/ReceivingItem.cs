using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMaterials.DB;

public class ReceivingItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(ReceivingReport))]
    public int ReceivingReportId { get; set; }

    [ForeignKey(nameof(Item))]
    public int ItemId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }

    [NotMapped]
    public decimal TotalPrice => Quantity * UnitPrice;

    public virtual ReceivingReport ReceivingReport { get; set; } = null!;
    public virtual Item Item { get; set; } = null!;
}
