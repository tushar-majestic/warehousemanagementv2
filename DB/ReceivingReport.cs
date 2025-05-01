using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMaterials.DB;

public class ReceivingReport
{
    [Key]
    public int Id { get; set; }
    // General info
    public int SerialNumber { get; set; }
    public string FiscalYear { get; set; }

    [DataType(DataType.Date)]
    public DateTime ReceivingDate { get; set; }

    public string RecipientSector { get; set; }
    public string SectorNumber { get; set; }

    public string ReceivingWarehouse { get; set; }
    public string BasedOnDocument { get; set; }

    public string DocumentNumber { get; set; }

    [DataType(DataType.Date)]
    public DateTime DocumentDate { get; set; }

    public string AttachmentPath { get; set; }

    // Foreign key reference to Supplier
    public int SupplierId { get; set; }
    public virtual Supplier Supplier { get; set; }  // Navigation property to Supplier



    // Metadata
    public string Comments { get; set; }
    public string RecipientEmployeeId { get; set; }
    public string TechnicalMember { get; set; }
    public string ChiefResponsible { get; set; }

    public string CreatedBy { get; set; }

    public virtual ICollection<ReceivingItem> Items { get; set; } = new List<ReceivingItem>();
}
