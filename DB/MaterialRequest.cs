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

    public int? DeptManagerId { get; set; } = null;


    public int? KeeperId { get; set; } = null;

    public int? SupervisorId { get; set; } = null;


    public string RequestDocumentType { get; set; } = null;
    public string DocumentNumber { get; set; } = null;
    public string Sector { get; set; } = null!;
    public int? WarehouseId { get; set; } = null;
    public bool KeeperApproval { get; set; }
    public DateTime? KeeperApprovalDate { get; set; }

    public bool SupervisorApproval { get; set; }
    public DateTime? SupervisorApprovalDate { get; set; }

    public bool DepartmentManagerApproval { get; set; }
    public DateTime? DeptManagerApprovalDate { get; set; }

    public DateTime? CreatedAt { get; set; }


    public int? CurrentApproverUserId { get; set; }

    [ForeignKey("CurrentApproverUserId")]
    [InverseProperty("MaterialRequestCurrentApproverUsers")]
    public virtual User? CurrentApproverUser { get; set; }

    [ForeignKey("RequestedByUserId")]
    [InverseProperty("MaterialRequestRequestedByUsers")]
    public virtual User RequestedByUser { get; set; } = null!;
    
    [InverseProperty("MaterialRequest")]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
