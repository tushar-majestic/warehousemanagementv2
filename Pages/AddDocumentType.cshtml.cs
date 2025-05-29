using Microsoft.AspNetCore.Mvc;

namespace LabMaterials.Pages
{
    public class AddDocumentTypeModel : BasePageModel
    {
        private readonly LabDBContext _context;

        public AddDocumentTypeModel(LabDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DocumentType DocumentType { get; set; } = new();
        public List<DocumentType> DocumentTypeList { get; set; } = new();

        public IActionResult OnGet()
        {
            base.ExtractSessionData();
            DocumentTypeList = _context.DocumentTypes.ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            _context.DocumentTypes.Add(DocumentType);
            await _context.SaveChangesAsync();

            return RedirectToPage("ViewDoctypes"); // Assumes you have an Index page
        }
    }
}
