using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Materias
{
    public class DetailsModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public DetailsModel(AsistenciaContext context)
        {
            _context = context;
        }

        public Materia Materia { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias.FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null) return NotFound();

            Materia = materia;
            return Page();
        }
    }
}