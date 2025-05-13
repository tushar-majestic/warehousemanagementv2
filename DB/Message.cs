using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Index("ReceivingReportId", Name = "IX_Messages_ReceivingReportId")]
public partial class Message
{
    [Key]
    public int Id { get; set; }

    public int ReceivingReportId { get; set; }

    public string Sender { get; set; } = null!;

    public string Recipient { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    [ForeignKey("ReceivingReportId")]
    [InverseProperty("Messages")]
    public virtual ReceivingReport ReceivingReport { get; set; } = null!;
}
