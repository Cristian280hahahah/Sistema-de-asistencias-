using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SistemaAsistencia.Models;

namespace SistemaAsistencia.Pages.Admin;

[Authorize(Policy = "AdminAccess")]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public IList<UserRow> Users { get; private set; } = new List<UserRow>();

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users.OrderBy(user => user.Email).ToListAsync();
        var rows = new List<UserRow>(users.Count);

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var isLockedOut = await _userManager.IsLockedOutAsync(user);
            rows.Add(new UserRow(user.Id, user.NombreCompleto, user.Email ?? user.UserName ?? "—", roles, isLockedOut));
        }

        Users = rows;
    }

    public sealed record UserRow(string Id, string Name, string Email, IList<string> Roles, bool IsLockedOut);
}
