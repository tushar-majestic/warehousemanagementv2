using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("CurrentApproverUserId", Name = "IX_MaterialRequests_CurrentApproverUserId")]
[Index("RequestedByUserId", Name = "IX_MaterialRequests_RequestedByUserId")]
public partial class MaterialRequest
{
    [Key]
    public int RequestId { get; set; }

    public int SerialNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }
    public string FiscalYear { get; set; } = null;

    public int RequestingSector { get; set; } 
    public int RequestedByUserId { get; set; }

    public int DeptManagerId { get; set; }
    public int KeeperId { get; set; }
    public int SupervisorId { get; set; }
    public int SectorManagerId { get; set; }

    public string RequestDocumentType { get; set; } = null;
    public string DocumentNumber { get; set; } = null;
    public string Sector { get; set; } = null!;
    public string WarehouseName { get; set; } = null;
    public bool KeeperApproval { get; set; }
    public bool SupervisorApproval { get; set; }
    public bool SectorManagerApproval { get; set; }


    public bool DepartmentManagerApproval { get; set; }
    public int? CurrentApproverUserId { get; set; }

    [ForeignKey("CurrentApproverUserId")]
    [InverseProperty("MaterialRequestCurrentApproverUsers")]
    public virtual User? CurrentApproverUser { get; set; }

    [ForeignKey("RequestedByUserId")]
    [InverseProperty("MaterialRequestRequestedByUsers")]
    public virtual User RequestedByUser { get; set; } = null!;
}
