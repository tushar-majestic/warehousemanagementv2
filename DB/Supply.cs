using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("Supply")]
public partial class Supply
{
    [Key]
    public int SupplyId { get; set; }

    public int ItemId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReceivedAt { get; set; }

    public int QuantityReceived { get; set; }

    [StringLength(100)]
    public string InvoiceNumber { get; set; } = null!;

    [StringLength(100)]
    public string PurchaseOrderNo { get; set; } = null!;

    public bool InventoryBalanced { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? ItemCode { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string? ItemType { get; set; }

    public int SupplierId { get; set; }

    [Column("Expiry_date", TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }

    [InverseProperty("Supply")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    [ForeignKey("SupplierId")]
    [InverseProperty("Supplies")]
    public virtual Supplier Supplier { get; set; } = null!;
}
