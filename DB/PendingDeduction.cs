using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class PendingDeduction
{

  [Key]
  public int Id { get; set; }

  public int StoreId { get; set; }
  public int ItemCardId { get; set; }
  public int RoomId { get; set; }

  public int ShelfId { get; set; }

  public int ReduceQty { get; set; }

  public int DeductedBy { get; set; }

  public DateTime OutDate { get; set; }

  public DateTime CreatedAt { get; set; }

  public int PartyId { get; set; }

  public int MaterialRequestId { get; set; }

  public string DocumentNumber { get; set; } = null;

  public bool Status { get; set; }

  [ForeignKey("ItemCardId")]
  public virtual ItemCard ItemCard { get; set; }

  [ForeignKey("ShelfId")]
  public virtual Shelf Shelf { get; set; }

 
  [ForeignKey("RoomId")]
  public virtual Room Room { get; set; }
    
  [ForeignKey("DeductedBy")]
  public virtual User User { get; set; }





}