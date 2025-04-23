using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Keyless]
[Table("StoreDataResult")]
public partial class StoreDataResult
{
    public int StoreId { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? StoreNumber { get; set; }

    public int? RoomId { get; set; }

    [Column("ROOM_NO")]
    [StringLength(250)]
    [Unicode(false)]
    public string? RoomNo { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? RoomName { get; set; }

    [Column("isActive")]
    public int? IsActive { get; set; }

    [Column("SHELF_NUMBER")]
    [StringLength(250)]
    [Unicode(false)]
    public string? ShelfNumber { get; set; }

    [StringLength(250)]
    [Unicode(false)]
    public string? StoreName { get; set; }
}
