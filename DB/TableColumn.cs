using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LabMaterials.DB
{
    [Table("tablecolumn")]
    public class TableColumn
    {
        [Key]
        public int Id { get; set; }
        public string DisplayColumns { get; set; }
        public string Page { get; set; }
        
        [ForeignKey("UserId")]
        [InverseProperty("User")]
        public int UserId { get; set; }
    }
}
