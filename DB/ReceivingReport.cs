using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("SupplierId", Name = "IX_ReceivingReports_SupplierId")]
public partial class ReceivingReport
{
    [Key]
    public int Id { get; set; }

    public string ReceivingWarehouse { get; set; } = null!;

    public DateTime ReceivingDate { get; set; }

    public string FiscalYear { get; set; } = null!;

    public string AttachmentPath { get; set; } = null!;

    public string BasedOnDocument { get; set; } = null!;



    public string CreatedBy { get; set; } = null!;

    public DateTime DocumentDate { get; set; }

    public string DocumentNumber { get; set; } = null!;

    public int RecipientEmployeeId { get; set; }

    public string RecipientSector { get; set; } = null!;

    public string SectorNumber { get; set; } = null!;

    public int SerialNumber { get; set; }

    public int TechnicalMemberId { get; set; }

    public int ChiefResponsibleId { get; set; }

    


    public int SupplierId { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool KeeperApproval { get; set; }
    public bool TechnicalMemberApproval { get; set; }
    public bool StoreManagerApproval { get; set; }
    public bool GeneralSupApproval { get; set; }


    [InverseProperty("ReceivingReport")]
    public virtual ICollection<ReceivingItem> ReceivingItems { get; set; } = new List<ReceivingItem>();

    [ForeignKey("SupplierId")]
    [InverseProperty("ReceivingReports")]
    public virtual Supplier Supplier { get; set; } = null!;
}
