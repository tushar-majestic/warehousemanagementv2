using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.DB;

[Keyless]
public partial class VActivityLog
{
    [StringLength(50)]
    public string UserName { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string Action { get; set; } = null!;

    [StringLength(1024)]
    public string Description { get; set; } = null!;

    [StringLength(25)]
    [Unicode(false)]
    public string SourceIp { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime Time { get; set; }
}
