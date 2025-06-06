using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using LabMaterials.DB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;

using SkiaSharp;
using System.Data;
namespace LabMaterials.Pages
{
    public class ViewItemCardsModel : BasePageModel
    {
        private readonly LabDBContext _context;
        public string ItemCardId { get; set; }
        public Room Room { get; set; }
        public Supplier Supplier { get; set; }
        public List<Destination> Party { get; set; }
        // public DespensedItem DespensedItem { get; set; }
        // public Store Store { get; set; }

        public string ItemTypeCode;
        public string ItemCode;

        public string ItemName;
        public string ItemDescription;

        public ViewItemCardsModel(LabDBContext context)
        {
            _context = context;
        }
        public List<ItemCard> ItemCards { get; set; }
        public ItemCard? SingleItemCard  { get; set; }
        public List<ItemCardBatch> BatchDetails { get; set; }
        public List<PendingDeduction> dispensed { get; set; }
        public ICollection<ItemCardBatch> ItemCardBatches { get; set; }
        public ICollection<Store> Store { get; set; }

        public async Task OnGetAsync(int id)
        {
            base.ExtractSessionData();
            this.ItemCardId = HttpContext.Session.GetString("ItemCardId");
            Console.WriteLine(ItemCardId);

            if (ItemCardId == null)
            {
                SingleItemCard = null;
                BatchDetails = null;
                return;
            }
            var dbContext = new LabDBContext();

            SingleItemCard = await _context.ItemCards
                .Include(r => r.ItemCardBatches)
                .Include(r => r.Store)
                .FirstOrDefaultAsync(r => r.Id == int.Parse(this.ItemCardId));
            Console.WriteLine(SingleItemCard);

            dispensed = await _context.PendingDeductions.Where(d => d.ItemCardId == int.Parse(this.ItemCardId)).ToListAsync();

            Party = dbContext.Destinations.ToList();

            
            BatchDetails = await _context.ItemCardBatches
                .Include(b => b.Room)
                .Include(b => b.Supplier)
                // .Include(b => b.DespensedItem)
                .Where(b => b.ItemCardId == int.Parse(this.ItemCardId))
                .ToListAsync();


        }
    }
}
