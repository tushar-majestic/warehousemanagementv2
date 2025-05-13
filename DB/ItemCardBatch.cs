using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("ItemCardId", Name = "IX_ItemCardBatches_ItemCardId")]
[Index("RoomId", Name = "IX_ItemCardBatches_RoomId")]
[Index("ShelfId", Name = "IX_ItemCardBatches_ShelfId")]
[Index("SupplierId", Name = "IX_ItemCardBatches_SupplierId")]
public partial class ItemCardBatch
{
    [Key]
    public int Id { get; set; }

    public int ItemCardId { get; set; }
    public DateTime DateOfEntry { get; set; }
    public string DocumentType { get; set; } = null!;

    public string ReceiptDocumentnumber { get; set; }  = null!;


    public int QuantityReceived { get; set; }
    public enum AssetType
    {
        Sustainable,
        Consumable
    }
    public AssetType TypeOfAsset { get; set; }
    public int Minimum { get; set; }

    public int ReorderLimit { get; set; }

    public int RoomId { get; set; }
    public int Ceiling => Minimum + ReorderLimit;
    public int ShelfId { get; set; }

    public int SupplierId { get; set; }

    [ForeignKey("ItemCardId")]
    [InverseProperty("ItemCardBatches")]
    public virtual ItemCard ItemCard { get; set; } = null!;

    [ForeignKey("RoomId")]
    [InverseProperty("ItemCardBatches")]
    public virtual Room Room { get; set; } = null!;

    [ForeignKey("ShelfId")]
    [InverseProperty("ItemCardBatches")]
    public virtual Shelf Shelf { get; set; } = null!;

    [ForeignKey("SupplierId")]
    [InverseProperty("ItemCardBatches")]
    public virtual Supplier Supplier { get; set; } = null!;
}
