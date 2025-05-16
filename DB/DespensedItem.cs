using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("ItemCardId", Name = "IX_ItemCards_ItemCardId")]
[Index("MaterialRequestId", Name = "IX_MaterialRequests_MaterialRequestId")]
public partial class DespensedItem
{
    [Key]
    public int Id { get; set; }

    public int MaterialRequestId { get; set; }
    public int ItemCardId { get; set; }
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }

    public int AmountSpent { get; set; }


    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalPrice { get; set; }

    public string Comments { get; set; } = null!;

    [ForeignKey("ItemCardId")]
    public virtual ItemCard ItemCard { get; set; } = null!;

    [ForeignKey("MaterialRequestId")]
    public virtual MaterialRequest MaterialRequest { get; set; } = null!;

}