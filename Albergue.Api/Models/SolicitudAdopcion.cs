using System;
using System.Collections.Generic;

namespace Albergue.Api.Models;

public partial class SolicitudAdopcion
{
    public int IdSolicitud { get; set; }

    public int IdUsuario { get; set; }

    public int IdAnimal { get; set; }

    public string Estado { get; set; } = null!;

    public DateTime FechaSolicitud { get; set; }

    public virtual Animal IdAnimalNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
