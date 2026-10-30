using System.Text;
using System.Text.Json;
using Albergue.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Albergue.Web.Controllers;

public class SolicitudesController : Controller
{
    private readonly IHttpClientFactory _factory;
    public SolicitudesController(IHttpClientFactory factory) => _factory = factory;

    // Mis solicitudes
    public async Task<IActionResult> Mias()
    {
        if (HttpContext.Session.GetString("token") == null)
            return RedirectToAction("Login", "Account");

        var client = _factory.CreateClient("api").ConToken(HttpContext);
        var resp = await client.GetAsync("api/Solicitudes/mias");
        if (!resp.IsSuccessStatusCode)
        {
            var detalle = await resp.Content.ReadAsStringAsync();
            TempData["Error"] = $"No se pudieron cargar tus solicitudes. Error {(int)resp.StatusCode}: {detalle}";
            return RedirectToAction("Index", "Animales");
        }

        var json = await resp.Content.ReadAsStringAsync();
        var lista = JsonSerializer.Deserialize<List<JsonElement>>(json) ?? new();
        return View(lista);
    }

    // Quiero adoptar
    [HttpPost]
    public async Task<IActionResult> Crear(int animalId)
    {
        if (HttpContext.Session.GetString("token") == null)
            return RedirectToAction("Login", "Account");

        var client = _factory.CreateClient("api").ConToken(HttpContext);
        var body = new StringContent(
            JsonSerializer.Serialize(new { idAnimal = animalId }), Encoding.UTF8, "application/json");

        var resp = await client.PostAsync("api/Solicitudes", body);
        if (resp.IsSuccessStatusCode)
        {
            TempData["Ok"] = "¡Solicitud enviada! Te contactaremos pronto.";
        }
        else
        {
            var detalle = await resp.Content.ReadAsStringAsync();
            TempData["Error"] = $"Error {(int)resp.StatusCode}: {detalle}";
        }

        return RedirectToAction("Index", "Animales");
    }
}