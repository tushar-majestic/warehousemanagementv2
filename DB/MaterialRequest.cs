using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MaterialRequest
{
    public MaterialRequest() { }

    [Key]
    public int RequestId { get; set; }

    [Required]
    [StringLength(100)]
    public string MaterialName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime RequestedDate { get; set; } = DateTime.Now;

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    [ForeignKey(nameof(RequestedByUser))]
    public int RequestedByUserId { get; set; }

    public virtual User RequestedByUser { get; set; } = null!;

    public int? CurrentApproverUserId { get; set; }

    public virtual User? CurrentApproverUser { get; set; }
}
