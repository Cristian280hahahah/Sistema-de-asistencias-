using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Aulas;

public class DeleteModel : PageModel
{
    private readonly AsistenciaContext _context;

    public DeleteModel(AsistenciaContext context) => _context = context;

    [BindProperty]
    public Aula Aula { get; set; } = new();

    public int SeccionesAsignadas { get; private set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();
        var aula = await _context.Aulas.FindAsync(id);
        if (aula is null) return NotFound();
        Aula = aula;
        SeccionesAsignadas = await _context.Secciones.CountAsync(section => section.AulaId == id);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null) return NotFound();
        var aula = await _context.Aulas.FindAsync(id);
        if (aula is null) return NotFound();

        SeccionesAsignadas = await _context.Secciones.CountAsync(section => section.AulaId == id);
        if (SeccionesAsignadas > 0)
        {
            Aula = aula;
            ModelState.AddModelError(string.Empty, "No se puede eliminar porque hay secciones asignadas a esta aula. Primero cambia o quita el aula de esas secciones.");
            return Page();
        }

        _context.Aulas.Remove(aula);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
