using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Estudiantes
{
    public class CreateModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public CreateModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Estudiante Estudiante { get; set; } = default!;

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Estudiante ??= new Estudiante();
            Estudiante.Codigo = Estudiante.Codigo?.Trim() ?? string.Empty;
            Estudiante.Email = Estudiante.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool codigoExiste = await _context.Estudiantes.AnyAsync(e => e.Codigo == Estudiante.Codigo);
            if (codigoExiste)
            {
                ModelState.AddModelError("Estudiante.Codigo", "Ya existe un estudiante registrado con este código/carnet.");
            }

            bool emailExiste = await _context.Estudiantes.AnyAsync(e => e.Email == Estudiante.Email);
            if (emailExiste)
            {
                ModelState.AddModelError("Estudiante.Email", "Ya existe un estudiante registrado con este correo.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Estudiantes.Add(Estudiante);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}