using Albergue.Api.Data;
using Albergue.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Albergue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalesController : ControllerBase
{
    private readonly AlbergueContext _db;

    public AnimalesController(AlbergueContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<Animal>>> Listar()
    {
        return await _db.Animals.ToListAsync();
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<Animal>> Obtener(int id)
    {
        var animal = await _db.Animals.FindAsync(id);
        if (animal is null) return NotFound();
        return animal;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Personal")]
    public async Task<ActionResult<Animal>> Crear(Animal animal)
    {
        _db.Animals.Add(animal);
        await _db.SaveChangesAsync();
        return Ok(animal);
    }

    [HttpPut]
    [Authorize(Roles = "Admin,Personal")]
    public async Task<IActionResult> Actualizar(Animal animal)
    {
        _db.Entry(animal).State = EntityState.Modified;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Personal")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var animal = await _db.Animals.FindAsync(id);
        if (animal is null) return NotFound();
        _db.Animals.Remove(animal);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}