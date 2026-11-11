using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Account;

[AllowAnonymous]
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public class InputModel
    {
        [Required(ErrorMessage = "Escribe tu correo electrónico.")]
        [EmailAddress(ErrorMessage = "Escribe un correo electrónico válido.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Escribe tu contraseña.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Mantener la sesión iniciada")]
        public bool RememberMe { get; set; }
    }

    public IActionResult OnGet(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;

        if (User.Identity?.IsAuthenticated != true)
        {
            return Page();
        }

        if (Url.IsLocalUrl(ReturnUrl))
        {
            return LocalRedirect(ReturnUrl!);
        }

        if (User.IsInRole("Admin"))
        {
            return RedirectToPage("/Index");
        }

        if (User.IsInRole("Profesor"))
        {
            return RedirectToPage("/PortalProfesor/Index");
        }

        return User.IsInRole("Estudiante")
            ? RedirectToPage("/PortalEstudiante/Index")
            : RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl ?? ReturnUrl;
        Input ??= new InputModel();
        Input.Email = Input.Email?.Trim().ToLowerInvariant() ?? string.Empty;

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (Url.IsLocalUrl(ReturnUrl))
            {
                return LocalRedirect(ReturnUrl!);
            }

            var signedInUser = await _userManager.FindByEmailAsync(Input.Email);
            var destination = signedInUser is not null && await _userManager.IsInRoleAsync(signedInUser, "Admin")
                ? Url.Page("/Index")!
                : signedInUser is not null && await _userManager.IsInRoleAsync(signedInUser, "Profesor")
                    ? Url.Page("/PortalProfesor/Index")!
                    : signedInUser is not null && await _userManager.IsInRoleAsync(signedInUser, "Estudiante")
                        ? Url.Page("/PortalEstudiante/Index")!
                        : Url.Page("/Index")!;

            return LocalRedirect(destination);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "La cuenta está temporalmente bloqueada. Inténtalo de nuevo más tarde.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
        }

        return Page();
    }
}
