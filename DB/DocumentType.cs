using System.ComponentModel.DataAnnotations;

namespace LabMaterials.DB
{
    public class DocumentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)] // Optional: add constraint to prevent unbounded strings
        public string DocType { get; set; } = string.Empty; // Good practice to initialize non-nullable string
    }
}
