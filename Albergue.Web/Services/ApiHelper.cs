using System.Net.Http.Headers;

namespace Albergue.Web.Services;

public static class ApiHelper
{
    public static HttpClient ConToken(this HttpClient client, HttpContext ctx)
    {
        var token = ctx.Session.GetString("token");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}