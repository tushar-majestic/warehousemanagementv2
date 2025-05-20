using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMaterials.DB;

[Table("REQUESTER")]
public partial class Requester
{
    [Key]
    [Column("REQ_ID")]
    public int ReqId { get; set; }

    [Column("REQ_NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? ReqName { get; set; }

    [Column("CONTACT_NO")]
    [StringLength(50)]
    [Unicode(false)]
    public string? ContactNo { get; set; }

    [Column("DESTINATION_ID")]
    public int? DestinationId { get; set; }

    [Column("DESTINATION_NAME")]
    [StringLength(100)]
    [Unicode(false)]
    public string? DestinationName { get; set; }

    [Column("CREATED", TypeName = "datetime")]
    public DateTime? Created { get; set; }

    [Column("CREATED_BY")]
    [StringLength(50)]
    [Unicode(false)]
    public string? CreatedBy { get; set; }

    [Column("UPDATED", TypeName = "datetime")]
    public DateTime? Updated { get; set; }

    [Column("UPDATED_BY")]
    [StringLength(50)]
    [Unicode(false)]
    public string? UpdatedBy { get; set; }

    [Column("ENDED", TypeName = "datetime")]
    public DateTime? Ended { get; set; }

    [Column("ENDED_BY")]
    [StringLength(50)]
    [Unicode(false)]
    public string? EndedBy { get; set; }

    [Column("IS_ACTIVE")]
    public int? IsActive { get; set; }

    [Column("STATUS")]
    public int? Status { get; set; }

    [ForeignKey("DestinationId")]
    [InverseProperty("Requesters")]
    public virtual Destination? Destination { get; set; }
    public override string ToString()
    {
        return DestinationName ?? base.ToString();
    }

}
