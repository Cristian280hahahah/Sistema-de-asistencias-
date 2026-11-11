using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Materias
{
    public class CreateModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public CreateModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Materia Materia { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Materia ??= new Materia();
            Materia.Codigo = Materia.Codigo?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validar si el código de materia ya existe en la base de datos
            bool codigoExiste = await _context.Materias.AnyAsync(m => m.Codigo == Materia.Codigo);
            if (codigoExiste)
            {
                ModelState.AddModelError("Materia.Codigo", "Ya existe una materia registrada con este código.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Materias.Add(Materia);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}