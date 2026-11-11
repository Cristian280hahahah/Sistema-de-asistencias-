using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Admin.TeacherAccounts;

[Authorize(Policy = "AdminAccess")]
public class CreateModel : PageModel
{
    private readonly AsistenciaContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CreateModel(AsistenciaContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [BindProperty]
    public int ProfessorId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public ProfessorSummary? Professor { get; private set; }

    public sealed record ProfessorSummary(int Id, string Name, string Email);

    public class InputModel
    {
        [Required(ErrorMessage = "La contraseña temporal es obligatoria.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirma la contraseña temporal.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public async Task<IActionResult> OnGetAsync(int? professorId)
    {
        if (professorId is null)
        {
            return NotFound();
        }

        ProfessorId = professorId.Value;
        return await LoadProfessorAsync() ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadProfessorAsync();
            return Page();
        }

        var professor = await _context.Profesores.FirstOrDefaultAsync(item => item.Id == ProfessorId);
        if (professor is null)
        {
            return NotFound();
        }

        if (professor.CuentaUsuarioId is not null)
        {
            ModelState.AddModelError(string.Empty, "Este profesor ya tiene una cuenta vinculada.");
            await LoadProfessorAsync();
            return Page();
        }

        var email = professor.Email.Trim().ToLowerInvariant();
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            ModelState.AddModelError(string.Empty, "Ya existe una cuenta con el correo del profesor. Revisa el panel de cuentas antes de continuar.");
            await LoadProfessorAsync();
            return Page();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        var account = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NombreCompleto = professor.Nombre
        };

        try
        {
            var createResult = await _userManager.CreateAsync(account, Input.Password);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                AddErrors(createResult);
                await LoadProfessorAsync();
                return Page();
            }

            var roleResult = await _userManager.AddToRoleAsync(account, "Profesor");
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync();
                AddErrors(roleResult);
                await LoadProfessorAsync();
                return Page();
            }

            professor.CuentaUsuarioId = account.Id;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "No se pudo crear o vincular la cuenta. Comprueba que el correo y el perfil no tengan otra cuenta asociada.");
            await LoadProfessorAsync();
            return Page();
        }

        TempData["TeacherAccountCreated"] = $"Se creó el acceso para {professor.Nombre}. Comparte la contraseña temporal de forma segura.";
        return RedirectToPage("./Index");
    }

    private async Task<bool> LoadProfessorAsync()
    {
        Professor = await _context.Profesores
            .AsNoTracking()
            .Where(professor => professor.Id == ProfessorId)
            .Select(professor => new ProfessorSummary(professor.Id, professor.Nombre, professor.Email))
            .FirstOrDefaultAsync();

        return Professor is not null;
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
