using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Estudiantes
{
    public class DeleteModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DeleteModel(AsistenciaContext context)
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes.FindAsync(id);

            if (estudiante != null)
            {
                Estudiante = estudiante;
                _context.Estudiantes.Remove(Estudiante);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}