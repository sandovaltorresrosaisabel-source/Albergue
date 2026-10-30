using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Albergue.Web.Controllers;

public class AccountController : Controller
{
    private readonly IHttpClientFactory _factory;
    public AccountController(IHttpClientFactory factory) => _factory = factory;

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var client = _factory.CreateClient("api");
        var body = new StringContent(
            JsonSerializer.Serialize(new { email, password }), Encoding.UTF8, "application/json");

        var resp = await client.PostAsync("api/Auth/login", body);
        if (!resp.IsSuccessStatusCode)
        {
            ViewBag.Error = "Correo o contraseña incorrectos.";
            return View();
        }

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var root = doc.RootElement;
        HttpContext.Session.SetString("token", root.GetProperty("token").GetString()!);
        HttpContext.Session.SetString("nombre", root.GetProperty("nombre").GetString()!);
        HttpContext.Session.SetString("rol", root.GetProperty("rol").GetString()!);

        return RedirectToAction("Index", "Animales");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}
