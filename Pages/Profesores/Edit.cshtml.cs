using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Profesores
{
    public class EditModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public EditModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Profesor Profesores { get; set; } = default!;
        public bool TieneCuentaVinculada { get; private set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Profesores == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores.FirstOrDefaultAsync(m => m.Id == id);
            if (profesor == null)
            {
                return NotFound();
            }
            Profesores = profesor;
            TieneCuentaVinculada = profesor.CuentaUsuarioId is not null;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Profesores ??= new Profesor();
            Profesores.Email = Profesores.Email?.Trim().ToLowerInvariant() ?? string.Empty;

            var profesorExistente = await _context.Profesores.FirstOrDefaultAsync(item => item.Id == Profesores.Id);
            if (profesorExistente is null)
            {
                return NotFound();
            }

            TieneCuentaVinculada = profesorExistente.CuentaUsuarioId is not null;
            if (TieneCuentaVinculada && !string.Equals(Profesores.Email, profesorExistente.Email, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("Profesores.Email", "No se puede cambiar el correo mientras la cuenta de acceso esté vinculada.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            profesorExistente.Nombre = Profesores.Nombre.Trim();
            if (!TieneCuentaVinculada)
            {
                profesorExistente.Email = Profesores.Email;
            }

            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }

        private bool ProfesorExists(int id)
        {
            return (_context.Profesores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}