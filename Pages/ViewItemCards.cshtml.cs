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
        public async Task OnGetAsync(int id)
        {
            base.ExtractSessionData();
            this.ItemCardId = HttpContext.Session.GetString("ItemCardId");
         
            if (ItemCardId == null)
            {
                SingleItemCard = null;
                return;
            }
            var dbContext = new LabDBContext();

            SingleItemCard = await _context.ItemCards.FirstOrDefaultAsync(r => r.Id == int.Parse(this.ItemCardId));
            Console.WriteLine(SingleItemCard);
        }
    }
}
