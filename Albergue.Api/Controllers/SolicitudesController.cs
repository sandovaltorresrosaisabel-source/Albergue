using Albergue.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albergue.Api.Controllers;

public record SolicitudCrearDto(int IdAnimal);

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SolicitudesController : ControllerBase
{
    private readonly AlbergueContext _db;

    public SolicitudesController(AlbergueContext db)
    {
        _db = db;
    }

    // El adoptante crea una solicitud
    [HttpPost]
    public async Task<IActionResult> Crear(SolicitudCrearDto dto)
    {
        var idUsuario = int.Parse(User.FindFirst("sub")!.Value);

        var animal = await _db.Animals.FindAsync(dto.IdAnimal);
        if (animal is null) return NotFound("El animal no existe.");
        if (animal.Estado != "Disponible")
            return BadRequest("El animal no está disponible.");

        var yaExiste = await _db.SolicitudAdopcions.AnyAsync(s =>
            s.IdUsuario == idUsuario && s.IdAnimal == dto.IdAnimal && s.Estado == "Enviada");
        if (yaExiste)
            return BadRequest("Ya tienes una solicitud pendiente para este animal.");

        var solicitud = new Albergue.Api.Models.SolicitudAdopcion
        {
            IdUsuario = idUsuario,
            IdAnimal = dto.IdAnimal,
            Estado = "Enviada",
            FechaSolicitud = DateTime.Now
        };

        _db.SolicitudAdopcions.Add(solicitud);
        await _db.SaveChangesAsync();

        return Ok(new { solicitud.IdSolicitud, solicitud.Estado, Animal = animal.Nombre });
    }

    // El adoptante ve sus propias solicitudes
    [HttpGet("mias")]
    public async Task<IActionResult> Mias()
    {
        var idUsuario = int.Parse(User.FindFirst("sub")!.Value);

        var lista = await _db.SolicitudAdopcions
            .Where(s => s.IdUsuario == idUsuario)
            .Select(s => new
            {
                s.IdSolicitud,
                s.Estado,
                s.FechaSolicitud,
                Animal = s.IdAnimalNavigation.Nombre
            })
            .ToListAsync();

        return Ok(lista);
    }

    // Admin y Personal ven todas (filtro opcional: ?estado=Enviada)
    [HttpGet]
    [Authorize(Roles = "Admin,Personal")]
    public async Task<IActionResult> Listar([FromQuery] string? estado)
    {
        var query = _db.SolicitudAdopcions.AsQueryable();
        if (!string.IsNullOrEmpty(estado))
            query = query.Where(s => s.Estado == estado);

        var lista = await query
            .Select(s => new
            {
                s.IdSolicitud,
                s.Estado,
                s.FechaSolicitud,
                s.IdAnimal,
                Animal = s.IdAnimalNavigation.Nombre,
                s.IdUsuario,
                Solicitante = s.IdUsuarioNavigation.Nombre
            })
            .ToListAsync();

        return Ok(lista);
    }

    // Aprobar: el animal pasa a "Adoptado" y las otras solicitudes pendientes se rechazan
    [HttpPut("{id}/aprobar")]
    [Authorize(Roles = "Admin,Personal")]
    public async Task<IActionResult> Aprobar(int id)
    {
        var s = await _db.SolicitudAdopcions
            .Include(x => x.IdAnimalNavigation)
            .FirstOrDefaultAsync(x => x.IdSolicitud == id);

        if (s is null) return NotFound();
        if (s.Estado != "Enviada")
            return BadRequest("La solicitud ya fue procesada.");

        s.Estado = "Aprobada";
        s.IdAnimalNavigation.Estado = "Adoptado";

        var otras = await _db.SolicitudAdopcions
            .Where(x => x.IdAnimal == s.IdAnimal && x.IdSolicitud != id && x.Estado == "Enviada")
            .ToListAsync();
        foreach (var o in otras) o.Estado = "Rechazada";

        await _db.SaveChangesAsync();
        return Ok(new { s.IdSolicitud, s.Estado, Animal = s.IdAnimalNavigation.Nombre });
    }

    [HttpPut("{id}/rechazar")]
    [Authorize(Roles = "Admin,Personal")]
    public async Task<IActionResult> Rechazar(int id)
    {
        var s = await _db.SolicitudAdopcions.FindAsync(id);
        if (s is null) return NotFound();
        if (s.Estado != "Enviada")
            return BadRequest("La solicitud ya fue procesada.");

        s.Estado = "Rechazada";
        await _db.SaveChangesAsync();
        return Ok(new { s.IdSolicitud, s.Estado });
    }
}