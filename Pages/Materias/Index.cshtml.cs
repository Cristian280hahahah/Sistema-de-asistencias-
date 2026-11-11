using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Materias
{
    public class IndexModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public IndexModel(AsistenciaContext context)
        {
            _context = context;
        }

        public IList<Materia> Materias { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Materias = await _context.Materias.ToListAsync();
        }
    }
}