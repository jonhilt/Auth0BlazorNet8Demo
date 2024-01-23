using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Auth0BlazorDemo.Auth;

public static class AuthExtensions
{
    public static void MapAuthRoutes(this WebApplication app)
    {
        app.MapGet("/Account/Login", async (HttpContext httpContext, string returnUrl = "/") =>
        {
            var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

            authenticationProperties.IsPersistent = true;

            await httpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        });

        app.MapPost("/Account/LogOut", async (HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(Auth0Constants.AuthenticationScheme);
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        });
    }
}