using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public class Message
{
    [Key]
    public int Id { get; set; }
    public int ReceivingReportId { get; set; }
    public string Sender { get; set; } = null!; 
    public string Recipient { get; set; } = null!; 
    public string Content { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [ForeignKey("ReceivingReportId")]
    public virtual ReceivingReport ReceivingReport { get; set; } = null!;
}
