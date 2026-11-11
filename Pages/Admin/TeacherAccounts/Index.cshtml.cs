using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Admin.TeacherAccounts;

[Authorize(Policy = "AdminAccess")]
public class IndexModel : PageModel
{
    private readonly AsistenciaContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(AsistenciaContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IList<TeacherAccountRow> Teachers { get; private set; } = new List<TeacherAccountRow>();

    public async Task<IActionResult> OnPostRevokeAsync(int professorId)
    {
        var professor = await _context.Profesores.FirstOrDefaultAsync(item => item.Id == professorId);
        if (professor?.CuentaUsuarioId is null)
        {
            TempData["TeacherAccountError"] = "El perfil no tiene una cuenta vinculada.";
            return RedirectToPage();
        }

        var user = await _userManager.FindByIdAsync(professor.CuentaUsuarioId);
        if (user is null)
        {
            professor.CuentaUsuarioId = null;
            await _context.SaveChangesAsync();
            TempData["TeacherAccountError"] = "La cuenta vinculada ya no existía; se limpió la asociación del perfil.";
            return RedirectToPage();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        var roles = await _userManager.GetRolesAsync(user);
        var otherRoles = roles.Where(role => role != "Profesor").ToList();

        if (otherRoles.Count > 0)
        {
            var removeRoleResult = await _userManager.RemoveFromRoleAsync(user, "Profesor");
            if (!removeRoleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                TempData["TeacherAccountError"] = "No se pudo revocar el rol Profesor.";
                return RedirectToPage();
            }
        }
        else
        {
            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
            {
                await transaction.RollbackAsync();
                TempData["TeacherAccountError"] = string.Join(" ", deleteResult.Errors.Select(error => error.Description));
                return RedirectToPage();
            }
        }

        professor.CuentaUsuarioId = null;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        TempData["TeacherAccountCreated"] = $"Se revocó el acceso de {professor.Nombre}.";
        return RedirectToPage();
    }

    public async Task OnGetAsync()
    {
        Teachers = await _context.Profesores
            .AsNoTracking()
            .OrderBy(professor => professor.Nombre)
            .Select(professor => new TeacherAccountRow(
                professor.Id,
                professor.Nombre,
                professor.Email,
                professor.CuentaUsuarioId != null))
            .ToListAsync();
    }

    public sealed record TeacherAccountRow(int ProfessorId, string Name, string Email, bool HasAccount);
}
