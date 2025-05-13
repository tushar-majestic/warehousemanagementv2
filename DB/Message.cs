using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("ReportId", Name = "IX_Messages_ReportId")]
public partial class Message
{
    [Key]
    public int Id { get; set; }

    public int? ReportId { get; set; }

    public string ReportType {get; set;}

    public int? SenderId { get; set; }

    public int? RecipientId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    [ForeignKey("ReportId")]
    [InverseProperty("Messages")]
    public virtual ReceivingReport ReceivingReport { get; set; } = null!;
}
