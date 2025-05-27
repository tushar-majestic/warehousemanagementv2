using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMaterials.DB
{
    public class ReturnRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string OrderNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string ToSector { get; set; } = "General Administration of Educational Services";

        [Required]
        public int FromSectorId { get; set; }
        [ForeignKey("FromSectorId")]
        public virtual Requester? FromSector { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? DestructionReportPath { get; set; } = null!;


        // Change this:
        [Required]
        public int WarehouseId { get; set; }

        [ForeignKey("WarehouseId")]
        public virtual Store? Warehouse { get; set; }  // Navigation property

        public bool IsSurplus { get; set; }
        public bool IsExpired { get; set; }
        public bool IsInvalid { get; set; }
        public bool IsDamaged { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public int? ManagerId { get; set; }
        public int? InspOffId { get; set; }
        public int? SupervisorId { get; set; }
        public int? DestOffId { get; set; }
        public int? CreatedBy { get; set; }

        public int? KeeperId { get; set; }
        public int? RecOffId { get; set; }

        [ForeignKey("ManagerId")]
        public virtual User? Manager { get; set; }

        [ForeignKey("InspOffId")]
        public virtual User? InspectionOfficer { get; set; }

        [ForeignKey("SupervisorId")]
        public virtual User? Supervisor { get; set; }

        [ForeignKey("DestOffId")]
        public virtual User? DestinationOfficer { get; set; }

        [ForeignKey("KeeperId")]
        public virtual User? WarehouseKeeper { get; set; }

        [ForeignKey("RecOffId")]
        public virtual User? ReceivingOfficer { get; set; }

        //  Approval Dates
        public DateTime? ManagerApprovalDate { get; set; }
        public DateTime? InspOffApprovalDate { get; set; }
        public DateTime? SupervisorApprovalDate { get; set; }
        public DateTime? DestOffApprovalDate { get; set; }
        public DateTime? KeeperApprovalDate { get; set; }

        public virtual ICollection<ReturnRequestItem> Items { get; set; } = new List<ReturnRequestItem>();
        [InverseProperty("ReturnRequest")]
        public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    }
}
