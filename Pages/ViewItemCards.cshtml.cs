using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewItemCardsModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public ViewItemCardsModel(LabDBContext context)
        {
            _context = context;
        }

        public List<ItemCard> ItemCards { get; set; }

        public async Task OnGetAsync()
        {
            base.ExtractSessionData();
            // ItemCards = await _context.ItemCards.ToListAsync();
        }
    }
}
