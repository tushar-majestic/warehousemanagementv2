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

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            HazardType = await _context.HazardTypes.ToListAsync();
        }
    }
}
