﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Ruslan.BlazorApp.Extensions
{
    internal static partial class CookieOidcServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureCookieOidcRefresh(this IServiceCollection services, string cookieScheme, string oidcScheme)
        {
            services.AddSingleton<CookieOidcRefresher>();
            services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<CookieOidcRefresher>((cookieOptions, refresher) =>
            {
                cookieOptions.Events.OnValidatePrincipal = context => refresher.ValidateOrRefreshCookieAsync(context, oidcScheme);
            });
            services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
            {
                oidcOptions.Scope.Add(OpenIdConnectScope.Email);
                oidcOptions.Scope.Add(OpenIdConnectScope.Profile);
                oidcOptions.SaveTokens = true;
            });
            return services;
        }
    }
}
