using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Data;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Admin.StudentAccounts;

[Authorize(Roles = "Admin")]
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
    public int StudentId { get; set; }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public StudentSummary? Student { get; private set; }

    public sealed record StudentSummary(int Id, string Code, string Name, string Email);

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

    public async Task<IActionResult> OnGetAsync(int? studentId)
    {
        if (studentId is null)
        {
            return NotFound();
        }

        StudentId = studentId.Value;
        return await LoadStudentAsync() ? Page() : NotFound();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            await LoadStudentAsync();
            return Page();
        }

        var student = await _context.Estudiantes.FirstOrDefaultAsync(item => item.Id == StudentId);
        if (student is null)
        {
            return NotFound();
        }

        if (student.CuentaUsuarioId is not null)
        {
            ModelState.AddModelError(string.Empty, "Este estudiante ya tiene una cuenta vinculada.");
            await LoadStudentAsync();
            return Page();
        }

        var email = student.Email.Trim().ToLowerInvariant();
        if (await _userManager.FindByEmailAsync(email) is not null)
        {
            ModelState.AddModelError(string.Empty, "Ya existe una cuenta con el correo del estudiante. Revisa el panel de cuentas antes de continuar.");
            await LoadStudentAsync();
            return Page();
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        var account = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true,
            NombreCompleto = student.Nombre
        };

        var createResult = await _userManager.CreateAsync(account, Input.Password);
        if (!createResult.Succeeded)
        {
            await transaction.RollbackAsync();
            AddErrors(createResult);
            await LoadStudentAsync();
            return Page();
        }

        var roleResult = await _userManager.AddToRoleAsync(account, "Estudiante");
        if (!roleResult.Succeeded)
        {
            await transaction.RollbackAsync();
            AddErrors(roleResult);
            await LoadStudentAsync();
            return Page();
        }

        student.CuentaUsuarioId = account.Id;
        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (DbUpdateException)
        {
            await transaction.RollbackAsync();
            ModelState.AddModelError(string.Empty, "No se pudo vincular la cuenta. Comprueba que el estudiante y el correo no tengan otra cuenta asociada.");
            await LoadStudentAsync();
            return Page();
        }

        TempData["StudentAccountCreated"] = $"Se creó el acceso para {student.Nombre}. Comparte la contraseña temporal de forma segura.";
        return RedirectToPage("./Index");
    }

    private async Task<bool> LoadStudentAsync()
    {
        Student = await _context.Estudiantes
            .AsNoTracking()
            .Where(student => student.Id == StudentId)
            .Select(student => new StudentSummary(student.Id, student.Codigo, student.Nombre, student.Email))
            .FirstOrDefaultAsync();

        return Student is not null;
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
}
