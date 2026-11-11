using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Profesores
{
    public class IndexModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public IndexModel(AsistenciaContext context)
        {
            _context = context;
        }

        public IList<Profesor> Profesores { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Profesores != null)
            {
                Profesores = await _context.Profesores.ToListAsync();
            }
        }
    }
}