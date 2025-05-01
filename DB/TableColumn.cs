using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Table("tablecolumn")]
public partial class Tablecolumn
{
    [Key]
    public int Id { get; set; }

    public string DisplayColumns { get; set; } = null!;

    public string Page { get; set; } = null!;

    public int UserId { get; set; }
}
