using System.ComponentModel.DataAnnotations;

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
        [StringLength(100)]
        public string FromSector { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required]
        //[StringLength(100)]
        public int Warehouse { get; set; }

        public bool IsSurplus { get; set; }
        public bool IsExpired { get; set; }
        public bool IsInvalid { get; set; }
        public bool IsDamaged { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        public virtual ICollection<ReturnRequestItem> Items { get; set; } = new List<ReturnRequestItem>();
    }

}
