using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;

namespace SistemaAsistencia.Pages.Admin.StudentAccounts;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AsistenciaContext _context;

    public IndexModel(AsistenciaContext context)
    {
        _context = context;
    }

    public IList<StudentAccountRow> Students { get; private set; } = new List<StudentAccountRow>();

    public async Task OnGetAsync()
    {
        Students = await _context.Estudiantes
            .AsNoTracking()
            .OrderBy(student => student.Nombre)
            .Select(student => new StudentAccountRow(
                student.Id,
                student.Codigo,
                student.Nombre,
                student.Email,
                student.CuentaUsuarioId != null))
            .ToListAsync();
    }

    public sealed record StudentAccountRow(int StudentId, string Code, string Name, string Email, bool HasAccount);
}
