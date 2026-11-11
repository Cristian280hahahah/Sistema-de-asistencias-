using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Profesores
{
    public class DetailsModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DetailsModel(AsistenciaContext context)
        {
            _context = context;
        }

        public Profesor Profesores { get; set; } = default!;

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
            return Page();
        }
    }
}