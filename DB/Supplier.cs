﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("Supplier")]
public partial class Supplier
{
    [Key]
    public int SupplierId { get; set; }

    public int? ExtensionNumber { get; set; }

    [StringLength(100)]
    public string SupplierName { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? SupplierContact { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? SupplierType { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Ended { get; set; }

    [Column("EndedByID")]
    public int? EndedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Updated { get; set; }

    [Column("UpdatedByID")]
    public int? UpdatedById { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? Created { get; set; }

    [Column("CreatedByID")]
    public int? CreatedById { get; set; }

    public string CoordinatorName { get; set; } = null!;

    [InverseProperty("Supplier")]
    public virtual ICollection<ItemCardBatch> ItemCardBatches { get; set; } = new List<ItemCardBatch>();

    [InverseProperty("Supply")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [InverseProperty("Supplier")]
    public virtual ICollection<ReceivingReport> ReceivingReports { get; set; } = new List<ReceivingReport>();

    [InverseProperty("Supplier")]
    public virtual ICollection<Supply> Supplies { get; set; } = new List<Supply>();
}
