using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;

namespace SistemaAsistencia.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public IndexModel(AsistenciaContext context)
        {
            _context = context;
        }

        public int Estudiantes { get; private set; }
        public int Profesores { get; private set; }
        public int Materias { get; private set; }
        public int Secciones { get; private set; }


        public async Task OnGetAsync()
        {

            Estudiantes = await _context.Estudiantes.CountAsync();
            Profesores = await _context.Profesores.CountAsync();
            Materias = await _context.Materias.CountAsync();
            Secciones = await _context.Secciones.CountAsync();

        }
    }
}
