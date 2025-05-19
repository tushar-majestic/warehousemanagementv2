using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;


namespace LabMaterials.DB;
public class DeductionExtended
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public int ItemCardId { get; set; }
     public string ItemCode { get; set; } = null!;
    public string ItemName { get; set; } = null!;
    public string GroupCode { get; set; } = null!;
    public string ItemTypeCode { get; set; } = null!;
    public string ItemDescription { get; set; } = null!;
    public string? UnitOfmeasure { get; set; }
    public int StoreId { get; set; }

    public string? Chemical { get; set; }

    public string HazardTypeName { get; set; } = null!;
    public DateTime? ExpiryDate { get; set; }
    public int QuantityReceived { get; set; }
    public int QuantityAvailable { get; set; }

    public int RoomId { get; set; }

    public int ShelfId { get; set; }


  
    public string TypeOfAsset { get; set; } = null!;


}