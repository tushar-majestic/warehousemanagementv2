using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("DisbursementRequest")]
public partial class DisbursementRequest
{
    [Key]
    public int DisbursementRequestId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReqReceivedAt { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(300)]
    public string? Comments { get; set; }

    public bool InventoryBalanced { get; set; }

    [StringLength(200)]
    public string RequesterName { get; set; } = null!;

    [StringLength(600)]
    public string RequestingPlace { get; set; } = null!;

    [Column("ITEMCODE")]
    [StringLength(50)]
    [Unicode(false)]
    public string? Itemcode { get; set; }

    [Column("ITEMTYPECODE")]
    [StringLength(15)]
    [Unicode(false)]
    public string? Itemtypecode { get; set; }

    [Column("ITEM_QUANTITY")]
    public int? ItemQuantity { get; set; }

    public int? StoreId { get; set; }

    [ForeignKey("StoreId")]
    [InverseProperty("DisbursementRequests")]
    public virtual Store? Store { get; set; }
}
