using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Estudiantes
{
    public class DetailsModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DetailsModel(AsistenciaContext context)
        {
            _context = context;
        }

        public Estudiante Estudiante { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var estudiante = await _context.Estudiantes.FirstOrDefaultAsync(m => m.Id == id);
            if (estudiante == null) return NotFound();

            Estudiante = estudiante;
            return Page();
        }
    }
}