using Microsoft.AspNetCore.Identity;
using Rodavia.Core.Entities;

namespace Rodavia.Web.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Nombre completo del usuario (usado para OAuth providers como Google)
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// URL de la foto de perfil (puede venir de Google OAuth)
    /// </summary>
    public string? ProfilePictureUrl { get; set; }

    /// <summary>
    /// Suscripciones del usuario
    /// </summary>
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}

