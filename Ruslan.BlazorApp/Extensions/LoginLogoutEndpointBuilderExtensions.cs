﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Ruslan.BlazorApp.Extensions
{
    public static class LoginLogoutEndpointBuilderExtensions
    {
        internal static IEndpointConventionBuilder MapLoginAndLogout(this IEndpointRouteBuilder endpoints)
        {
            var group = endpoints.MapGroup("");
            group.MapGet("/login", (string? returnUrl) => TypedResults.Challenge(GetAuthProperties(returnUrl))).AllowAnonymous();

            group.MapPost("/logout", (string? returnUrl)
                => TypedResults.SignOut(GetAuthProperties(returnUrl),
                [
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    AppData.OidcSchemeName
                ]));

            return group;
        }

        private static AuthenticationProperties GetAuthProperties(string? returnUrl)
        {
            const string pathBase = "/";

            if (string.IsNullOrEmpty(returnUrl))
            {
                returnUrl = pathBase;
            }
            else if (!Uri.IsWellFormedUriString(returnUrl, UriKind.Relative))
            {
                returnUrl = new Uri(returnUrl, UriKind.Absolute).PathAndQuery;
            }
            else if (returnUrl[0] != '/')
            {
                returnUrl = $"{pathBase}{returnUrl}";
            }

            return new AuthenticationProperties{ RedirectUri = returnUrl};
        }
    }
}
