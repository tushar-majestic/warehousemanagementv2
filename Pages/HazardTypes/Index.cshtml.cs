using LabMaterials.Pages;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages_HazardTypes
{
    public class IndexModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public IndexModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        public IList<HazardType> HazardType { get; set; } = default!;
        public string lblUsers, lblHazardTypeName, lblAction, lblHazardType;
        public async Task OnGetAsync()
        {
            FillLables();
            base.ExtractSessionData();
            HazardType = await _context.HazardTypes.ToListAsync();
        }
        private void FillLables()
        {
            this.lblHazardType = (Program.Translations["HazardType"])[Lang];
            this.lblUsers = (Program.Translations["Users"])[Lang];
            this.lblHazardTypeName = (Program.Translations["HazardTypeName"])[Lang];
            this.lblAction = (Program.Translations["Action"])[Lang];
        }
        
    }
}
