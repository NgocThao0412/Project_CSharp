using System;
using Microsoft.AspNetCore.Http;

namespace Catalog.Web.Features.Hao;

/// <summary>Links from the legacy MVC pages to the React storefront in development.</summary>
public static class HaoFrontendLinks
{
    public static string Origin(HttpRequest request)
    {
        // Optional explicit host for LAN, Codespaces or deployed environments.
        var configured = Environment.GetEnvironmentVariable("CATALOG_FRONTEND_ORIGIN");
        if (Uri.TryCreate(configured, UriKind.Absolute, out var url)
            && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps)
            && string.IsNullOrEmpty(url.UserInfo))
            return url.GetLeftPart(UriPartial.Authority).TrimEnd('/');

        var host = request.Host.Host;
        // Local dev proxy; in a real deployment React is served by ASP.NET Core.
        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        if (isDev && (host.Equals("localhost", StringComparison.OrdinalIgnoreCase)
            || host == "127.0.0.1"))
            return $"{request.Scheme}://{host}:5173";

        const string codespacesSuffix = "-5080.app.github.dev";
        if (isDev && host.EndsWith(codespacesSuffix, StringComparison.OrdinalIgnoreCase))
            return "https://" + host[..^codespacesSuffix.Length] + "-5173.app.github.dev";

        // Production (React built into wwwroot): route to same origin.
        return string.Empty;
    }

    public static string Home(HttpRequest request) => Origin(request) + "/";
    public static string Products(HttpRequest request) => Origin(request) + "/san-pham";
}
