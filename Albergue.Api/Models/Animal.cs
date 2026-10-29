using System;
using System.Collections.Generic;

namespace Albergue.Api.Models;

public partial class Animal
{
    public int IdAnimal { get; set; }

    public string Nombre { get; set; } = null!;

    public string Especie { get; set; } = null!;

    public int? Edad { get; set; }

    public string Estado { get; set; } = null!;

    public DateOnly FechaIngreso { get; set; }

    public virtual ICollection<SolicitudAdopcion> SolicitudAdopcions { get; set; } = new List<SolicitudAdopcion>();
}
