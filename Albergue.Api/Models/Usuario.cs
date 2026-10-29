using System;
using System.Collections.Generic;

namespace Albergue.Api.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public int IdRol { get; set; }

    public virtual Rol IdRolNavigation { get; set; } = null!;

    public virtual ICollection<SolicitudAdopcion> SolicitudAdopcions { get; set; } = new List<SolicitudAdopcion>();
}
