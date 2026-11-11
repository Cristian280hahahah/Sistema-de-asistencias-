using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Materias
{
    public class DeleteModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DeleteModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Materia Materia { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias.FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null) return NotFound();

            Materia = materia;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias.FindAsync(id);

            if (materia != null)
            {
                Materia = materia;
                _context.Materias.Remove(Materia);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}