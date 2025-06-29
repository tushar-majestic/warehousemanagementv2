﻿    using System;
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

    public int? MaterialRequestId { get; set; }

    public int? ReturnRequestId { get; set; }


    public string ReportType { get; set; }

    public int? SenderId { get; set; }

    public int? RecipientId { get; set; }

    public string Content { get; set; } = null!;
    public string? ArContent { get; set; } 


    public DateTime CreatedAt { get; set; }

    public string Type { get; set; } = null!;

    [ForeignKey("ReportId")]
    [InverseProperty("Messages")]
    public virtual ReceivingReport? ReceivingReport { get; set; } = null!;

    [ForeignKey("MaterialRequestId")]
    [InverseProperty("Messages")]
    public virtual MaterialRequest? MaterialRequest { get; set; } = null!;
        
    [ForeignKey("ReturnRequestId")]
    [InverseProperty("Messages")]
    public virtual ReturnRequest? ReturnRequest { get; set; } = null!;

    }
