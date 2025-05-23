using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LabMaterials.Pages
{
    public class ViewDoctypesModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public ViewDoctypesModel(LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DocumentType DocumentType { get; set; } = new();

        [BindProperty]
        public int EditId { get; set; }

        public List<DocumentType> DocumentTypeList { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            base.ExtractSessionData();
            DocumentTypeList = await _context.DocumentTypes.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                DocumentTypeList = await _context.DocumentTypes.ToListAsync();
                return Page();
            }

            _context.DocumentTypes.Add(DocumentType);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditInitAsync()
        {
            base.ExtractSessionData();
            DocumentTypeList = await _context.DocumentTypes.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            base.ExtractSessionData();
            if (!ModelState.IsValid)
            {
                DocumentTypeList = await _context.DocumentTypes.ToListAsync();
                return Page();
            }

            var existing = await _context.DocumentTypes.FindAsync(DocumentType.Id);
            if (existing != null)
            {
                existing.DocType = DocumentType.DocType;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var documentType = await _context.DocumentTypes.FindAsync(id);
            if (documentType != null)
            {
                _context.DocumentTypes.Remove(documentType);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
