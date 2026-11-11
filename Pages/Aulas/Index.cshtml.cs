using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Aulas;

public class IndexModel : PageModel
{
    private readonly AsistenciaContext _context;

    public IndexModel(AsistenciaContext context) => _context = context;

    public IList<Aula> Aulas { get; private set; } = new List<Aula>();

    public async Task OnGetAsync()
    {
        Aulas = await _context.Aulas
            .Include(aula => aula.Secciones)
            .OrderBy(aula => aula.Nombre)
            .ToListAsync();
    }
}
