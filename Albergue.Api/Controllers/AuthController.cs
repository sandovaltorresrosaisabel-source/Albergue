using Albergue.Api.Data;
using Albergue.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Albergue.Api.Controllers;

public record LoginDto(string Email, string Password);
public record RegistroDto(string Nombre, string Email, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AlbergueContext _db;
    private readonly IConfiguration _config;

    public AuthController(AlbergueContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // Registro público: siempre crea un Adoptante
    [HttpPost("registro")]
    public async Task<IActionResult> Registro(RegistroDto dto)
    {
        if (await _db.Usuarios.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("El correo ya está registrado.");

        var rol = await _db.Rols.FirstOrDefaultAsync(r => r.Nombre == "Adoptante");
        if (rol is null)
            return BadRequest("Falta el rol 'Adoptante' en la tabla Rol.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            IdRol = rol.IdRol
        };

        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return Ok(new { usuario.IdUsuario, usuario.Nombre, usuario.Email });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
            return Unauthorized("Correo o contraseña incorrectos.");

        var rol = await _db.Rols.FirstAsync(r => r.IdRol == usuario.IdRol);

        var claims = new[]
        {
            new Claim("sub", usuario.IdUsuario.ToString()),
            new Claim("name", usuario.Nombre),
            new Claim("email", usuario.Email),
            new Claim("role", rol.Nombre)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            nombre = usuario.Nombre,
            rol = rol.Nombre
        });
    }
}