using Microsoft.AspNetCore.Identity;
using Rodavia.Core.Entities;

namespace Rodavia.Web.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Suscripciones del usuario
    /// </summary>
    public virtual ICollection<Suscripcion> Suscripciones { get; set; } = new List<Suscripcion>();
}

