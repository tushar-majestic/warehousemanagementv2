using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

public partial class PrimaryKey
{
    [Key]
    [StringLength(255)]
    [Unicode(false)]
    public string TableName { get; set; } = null!;

    public int NextId { get; set; }
}
