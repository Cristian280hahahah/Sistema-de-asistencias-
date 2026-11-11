using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Materias
{
    public class EditModel : PageModel
    {
        private readonly AsistenciaContext _context;

        public EditModel(AsistenciaContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Materia Materia { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null) return NotFound();

            var materia = await _context.Materias.FirstOrDefaultAsync(m => m.Id == id);
            if (materia == null) return NotFound();

            Materia = materia;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Materia ??= new Materia();
            Materia.Codigo = Materia.Codigo?.Trim() ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validar unicidad del código excluyendo la materia que se edita
            bool codigoExiste = await _context.Materias.AnyAsync(m => m.Codigo == Materia.Codigo && m.Id != Materia.Id);
            if (codigoExiste)
            {
                ModelState.AddModelError("Materia.Codigo", "Ya existe otra materia con este código.");
            }

            if (!ModelState.IsValid) return Page();

            _context.Attach(Materia).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Materias.Any(e => e.Id == Materia.Id)) return NotFound();
                else throw;
            }

            return RedirectToPage("./Index");
        }
    }
}