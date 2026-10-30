namespace Albergue.Web.Models;

public class AnimalVm
{
    public int IdAnimal { get; set; }
    public string Nombre { get; set; } = "";
    public string Especie { get; set; } = "";
    public int? Edad { get; set; }
    public string Estado { get; set; } = "";
    public string? FechaIngreso { get; set; }
}