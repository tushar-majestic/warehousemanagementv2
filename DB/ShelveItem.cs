using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;


public partial class ShelveItem
{
    [Key]
    public int Id { get; set; }
    public int ItemCardId { get; set; }

    public int ShelfId { get; set; }

    public int QuantityAvailable { get; set; }

    [ForeignKey("ItemCardId")]
    [InverseProperty("ShelveItems")]
    public virtual ItemCard ItemCard { get; set; } = null!;

    [ForeignKey("ShelfId")]
    [InverseProperty("ShelveItems")]
    public virtual Shelf Shelf { get; set; } = null!;

}