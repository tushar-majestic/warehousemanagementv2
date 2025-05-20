using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LabMaterials.DB;
using System.Linq;
using LabMaterials.DB;
using LabMaterials.dtos;
using Org.BouncyCastle.Cms;


namespace LabMaterials.Pages
{
    public class EditDispensingReportsModel : BasePageModel
    {           public string ErrorMsg { get; set; }


    }
}