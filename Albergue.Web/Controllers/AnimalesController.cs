using System.Text.Json;
using Albergue.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Albergue.Web.Controllers;

public class AnimalesController : Controller
{
    private readonly IHttpClientFactory _factory;
    public AnimalesController(IHttpClientFactory factory) => _factory = factory;

    public async Task<IActionResult> Index()
    {
        var client = _factory.CreateClient("api");
        var json = await client.GetStringAsync("api/Animales");
        var lista = JsonSerializer.Deserialize<List<AnimalVm>>(
            json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return View(lista);
    }
}