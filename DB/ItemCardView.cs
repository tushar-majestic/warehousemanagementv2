using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace LabMaterials.DB;
public class ItemCardViewModels
{
    public int Id { get; set; }
    public string GroupCode { get; set; }
    public int ItemCardId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string ItemDescription { get; set; }
    public string UnitOfMeasure { get; set; }
    public string Chemical { get; set; }
    public string HazardTypeName { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public string TypeOfAsset { get; set; }
    public int Minimum { get; set; }
    public int ReorderLimit { get; set; }

    public string WarehouseName { get; set; }  
    public int QuantityReceived { get; set; }

    public int QuantityAvailable { get; set; }

    public DateTime? DateOfEntry { get; set; }

    public string RoomName { get; set; }
    public string ShelfName { get; set; }
    public string SupplierName { get; set; }

    public string DocumentType { get; set; }
    public string ReceiptDocumentNumber { get; set; }
}
