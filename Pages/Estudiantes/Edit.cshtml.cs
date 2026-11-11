using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Estudiantes
{
    public class EditModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public EditModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Estudiante Estudiante { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes.FirstOrDefaultAsync(m => m.Id == id);
            if (estudiante == null) return NotFound();

            Estudiante = estudiante;
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

            bool codigoExiste = await _context.Estudiantes.AnyAsync(e => e.Codigo == Estudiante.Codigo && e.Id != Estudiante.Id);
            if (codigoExiste)
            {
                ModelState.AddModelError("Estudiante.Codigo", "Ya existe otro estudiante con este código/carnet.");
            }

            bool emailExiste = await _context.Estudiantes.AnyAsync(e => e.Email == Estudiante.Email && e.Id != Estudiante.Id);
            if (emailExiste)
            {
                ModelState.AddModelError("Estudiante.Email", "Ya existe otro estudiante con este correo.");
            }

            if (!ModelState.IsValid) return Page();

            _context.Attach(Estudiante).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Estudiantes.Any(e => e.Id == Estudiante.Id)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }
    }
}