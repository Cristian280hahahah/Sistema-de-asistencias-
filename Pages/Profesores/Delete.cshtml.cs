using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Profesores
{
    public class DeleteModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DeleteModel(AsistenciaContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Profesores == null)
            {
                return NotFound();
            }

            var profesor = await _context.Profesores.FindAsync(id);

            if (profesor is null)
            {
                return RedirectToPage("./Index");
            }

            if (profesor.CuentaUsuarioId is not null)
            {
                Profesores = profesor;
                TieneCuentaVinculada = true;
                ModelState.AddModelError(string.Empty, "No se puede eliminar este perfil mientras tenga una cuenta de acceso vinculada. Revoca primero el acceso desde Administración.");
                return Page();
            }

            _context.Profesores.Remove(profesor);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}