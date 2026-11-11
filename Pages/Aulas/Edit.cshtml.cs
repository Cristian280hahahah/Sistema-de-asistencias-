using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Aulas;

public class EditModel : PageModel
{
    private readonly AsistenciaContext _context;

    public EditModel(AsistenciaContext context) => _context = context;

    [BindProperty]
    public Aula Aula { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null) return NotFound();
        var aula = await _context.Aulas.FindAsync(id);
        if (aula is null) return NotFound();
        Aula = aula;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Aula.Nombre = Aula.Nombre?.Trim() ?? string.Empty;
        Aula.Ubicacion = string.IsNullOrWhiteSpace(Aula.Ubicacion) ? null : Aula.Ubicacion.Trim();
        Aula.Descripcion = string.IsNullOrWhiteSpace(Aula.Descripcion) ? null : Aula.Descripcion.Trim();

        if (!ModelState.IsValid) return Page();

        if (await _context.Aulas.AnyAsync(aula => aula.Nombre == Aula.Nombre && aula.Id != Aula.Id))
        {
            ModelState.AddModelError("Aula.Nombre", "Ya existe otra aula con este nombre.");
            return Page();
        }

        _context.Attach(Aula).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await _context.Aulas.AnyAsync(aula => aula.Id == Aula.Id)) return NotFound();
            throw;
        }

        return RedirectToPage("./Index");
    }
}
