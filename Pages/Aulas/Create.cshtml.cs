using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Aulas;

public class CreateModel : PageModel
{
    private readonly AsistenciaContext _context;

    public CreateModel(AsistenciaContext context) => _context = context;

    [BindProperty]
    public Aula Aula { get; set; } = new();

    public IActionResult OnGet() => Page();

    public async Task<IActionResult> OnPostAsync()
    {
        Aula.Nombre = Aula.Nombre?.Trim() ?? string.Empty;
        Aula.Ubicacion = string.IsNullOrWhiteSpace(Aula.Ubicacion) ? null : Aula.Ubicacion.Trim();
        Aula.Descripcion = string.IsNullOrWhiteSpace(Aula.Descripcion) ? null : Aula.Descripcion.Trim();

        if (!ModelState.IsValid) return Page();

        if (await _context.Aulas.AnyAsync(aula => aula.Nombre == Aula.Nombre))
        {
            ModelState.AddModelError("Aula.Nombre", "Ya existe un aula con este nombre.");
            return Page();
        }

        _context.Aulas.Add(Aula);
        await _context.SaveChangesAsync();
        return RedirectToPage("./Index");
    }
}
