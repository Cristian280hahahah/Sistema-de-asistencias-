using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Aulas;

public class DetailsModel : PageModel
{
    private readonly AsistenciaContext _context;

    public DetailsModel(AsistenciaContext context) => _context = context;

    public Aula Aula { get; private set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();
        var aula = await _context.Aulas
            .Include(item => item.Secciones)
                        .ThenInclude(section => section.Materia)
            .FirstOrDefaultAsync(item => item.Id == id);
        if (aula is null) return NotFound();
        Aula = aula;
        return Page();
    }
}
