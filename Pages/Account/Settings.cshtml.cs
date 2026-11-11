using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaAsistencia.Pages.Account;

[Authorize]
public class SettingsModel : PageModel
{
    public void OnGet() { }
}
