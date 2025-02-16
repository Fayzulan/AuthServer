using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Client.AspNetCore;
using OpenIddict.Server.AspNetCore;
using Ruslan.BlazorApp;
using Ruslan.BlazorApp.Components;
using Ruslan.BlazorApp.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(AppData.OidcSchemeName)
    .AddOpenIdConnect(AppData.OidcSchemeName, options =>
    {
        builder.Configuration.GetSection("OpenIdConnectSettings").Bind(options);

        options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.RequireHttpsMetadata = false;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false; // Remove Microsoft mappings
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "name"
        };

        // для возможности без новой авторизации делать запросы на другой сервис
        options.Events = new OpenIdConnectEvents()
        {
            OnTicketReceived = context =>
            {
                if (context.Properties is null)
                {
                    return Task.CompletedTask;
                }

                var properties = new AuthenticationProperties(context.Properties.Items);
                properties.StoreTokens(context.Properties.GetTokens().Where(token => token switch
                {
                    {
                        Name: OpenIddictClientAspNetCoreConstants.Tokens.BackchannelAccessToken
                        or OpenIddictClientAspNetCoreConstants.Tokens.RefreshToken
                        or OpenIddictServerAspNetCoreConstants.Tokens.AccessToken
                    } => true,
                    _ => false // ignore the other tokens
                }));

                return Task.CompletedTask;
            }
        };
    })
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);

builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, AppData.OidcSchemeName);
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapGroup("/authentication").MapLoginAndLogout();
app.Run();
