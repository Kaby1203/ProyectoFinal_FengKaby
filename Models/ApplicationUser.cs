using Microsoft.AspNetCore.Identity;

namespace ProyectoFinal_FengKaby.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NombreCompleto { get; set; }
    }
}
