using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Profesores
{
    public class CreateModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public CreateModel(AsistenciaContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Profesor Profesores { get; set; } = default!;

        public async Task<IActionResult> OnPostAsync()
        {
            Profesores ??= new Profesor();
            Profesores.Email = Profesores.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            if (!ModelState.IsValid || _context.Profesores == null)
            {
                return Page();
            }

            _context.Profesores.Add(Profesores);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}