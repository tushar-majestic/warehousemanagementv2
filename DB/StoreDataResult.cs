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

    [StringLength(50)]
    public string? StoreType { get; set; }

    [StringLength(20)]
    public string? WarehouseManagerName { get; set; }

    public int? WarehouseManagerID { get; set; }

    [StringLength(10)]
    public string? WarehouseStatus { get; set; }

    public int? ManagerJobNum { get; set; }

    public int? KeeperJobNum { get; set; }

    [StringLength(20)]
    public string? KeeperName { get; set; }

    [StringLength(10)]
    public string? BuildingNumber { get; set; }

    public int? NoOfShelves { get; set; }

    [StringLength(80)]
    public string? RoomDesc { get; set; }

    [StringLength(10)]
    public string? RoomStatus { get; set; }
}
