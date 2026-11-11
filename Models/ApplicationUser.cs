using Microsoft.AspNetCore.Identity;

namespace SistemaAsistencia.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; } = string.Empty;
    }
}
