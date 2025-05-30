using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages.Alerts
{
    public class IndexModel : BasePageModel
    {
        private readonly LabMaterials.DB.LabDBContext _context;

        public IndexModel(LabMaterials.DB.LabDBContext context)
        {
            _context = context;
        }

        public IList<ItemCard> ItemCardminimum { get; set; } = default!;
        public IList<ItemCard> ItemCardCeiling { get; set; } = default!;
        public IList<ItemCard> ItemCardReorder { get; set; } = default!;
        public IList<ItemCardBatch> ItemCardNotMoved { get; set; } = default!;

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            ItemCardminimum = await _context.ItemCards.Where(i => i.QuantityAvailable < i.Minimum)
                .Include(i => i.GroupCodeNavigation)
                .Include(i => i.HazardTypeNameNavigation)
                .Include(i => i.Item)
                .Include(i => i.ItemTypeCodeNavigation)
                .Include(i => i.Store).ToListAsync();

            ItemCardCeiling = await _context.ItemCards.Where(i => i.QuantityAvailable >= i.Ceiling)
                .Include(i => i.GroupCodeNavigation)
                .Include(i => i.HazardTypeNameNavigation)
                .Include(i => i.Item)
                .Include(i => i.ItemTypeCodeNavigation)
                .Include(i => i.Store).ToListAsync();

            ItemCardReorder = await _context.ItemCards.Where(i => i.QuantityAvailable <= i.ReorderLimit)
                .Include(i => i.GroupCodeNavigation)
                .Include(i => i.HazardTypeNameNavigation)
                .Include(i => i.Item)
                .Include(i => i.ItemTypeCodeNavigation)
                .Include(i => i.Store).ToListAsync();

            ItemCardNotMoved = await _context.ItemCardBatches.Where(i => i.DateOfEntry <= DateTime.Today.AddYears(-3))
                 .Include(i => i.ItemCard)
                   .ToListAsync();
        }
    }
}
