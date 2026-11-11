using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Secciones
{
    public class DeleteModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DeleteModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Seccion Seccion { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var seccion = await _context.Secciones
                .Include(s => s.Materia)
                .Include(s => s.Profesor)
                .Include(s => s.Aula)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (seccion == null) return NotFound();

            Seccion = seccion;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var seccion = await _context.Secciones.FindAsync(id);

            if (seccion != null)
            {
                Seccion = seccion;
                _context.Secciones.Remove(Seccion);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}