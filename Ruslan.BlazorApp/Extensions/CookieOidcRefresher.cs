using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Ruslan.BlazorApp.Extensions
{
    /// <summary>
    /// https://github.com/dotnet/aspnetcore/issues/8175
    /// </summary>
    /// <param name="oidcOptionsMonitor"></param>
    internal sealed class CookieOidcRefresher(IOptionsMonitor<OpenIdConnectOptions> oidcOptionsMonitor)
    {
        private readonly OpenIdConnectProtocolValidator _oidcTokenValidator = new()
        {
            RequireNonce = false
        };

        public async Task ValidateOrRefreshCookieAsync(CookieValidatePrincipalContext validateContext, string oidcScheme)
        {
            var accessTokenExpirationText = validateContext.Properties.GetTokenValue("expire_at");
            if (!DateTimeOffset.TryParse(accessTokenExpirationText, out var accessTokenExpiration))
            {
                return;
            }

            var oidcOptions = oidcOptionsMonitor.Get(oidcScheme);
            var now = oidcOptions.TimeProvider!.GetUtcNow();
            if (now + TimeSpan.FromMinutes(5) < accessTokenExpiration)
            {
                return;
            }

            var oidcConfiguration = await oidcOptions.ConfigurationManager!.GetConfigurationAsync(validateContext.HttpContext.RequestAborted);
            var tokenEndpoint = oidcConfiguration.TokenEndpoint ?? throw new InvalidOperationException("Cannot refresh cookie. TokenEndpoint");

            using var refreshResponse = await oidcOptions.Backchannel.PostAsync(tokenEndpoint,
                new FormUrlEncodedContent(new Dictionary<string, string?>()
                {
                    ["grant_type"] = "refresh_token",
                    ["client_id"] = oidcOptions.ClientId,
                    ["client_seret"] = oidcOptions.ClientSecret,
                    ["scope"] = string.Join(" ", oidcOptions.Scope),
                    ["refresh_token"] = validateContext.Properties.GetTokenValue("refresh_token")
                }));

            if (!refreshResponse.IsSuccessStatusCode)
            {
                validateContext.RejectPrincipal();
                return;
            }

            var refreshJson = await refreshResponse.Content.ReadAsStringAsync();
            var message = new OpenIdConnectMessage(refreshJson);

            var validateParameters = oidcOptions.TokenValidationParameters.Clone();
            if (oidcOptions.ConfigurationManager is BaseConfigurationManager baseConfigurationManager)
            {
                validateParameters.ConfigurationManager = baseConfigurationManager;
            }
            else
            {
                validateParameters.ValidIssuer = oidcConfiguration.Issuer;
                validateParameters.IssuerSigningKeys = oidcConfiguration.SigningKeys;
            }

            var validationResult = await oidcOptions.TokenHandler.ValidateTokenAsync(message.IdToken, validateParameters);

            if(!validationResult.IsValid)
            {
                validateContext.RejectPrincipal();
                return;
            }

            var validateIdToken = JwtSecurityTokenConverter.Convert(validationResult.SecurityToken as JsonWebToken);
            validateIdToken.Payload["nonce"] = null;
            _oidcTokenValidator.ValidateTokenResponse(new()
            {
                ProtocolMessage = message,
                ClientId = oidcOptions.ClientId,
                ValidatedIdToken = validateIdToken
            });

            validateContext.ShouldRenew = true;
            validateContext.ReplacePrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));

            var expiresIn = int.Parse(message.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture);
            var expiresAt = now + TimeSpan.FromSeconds(expiresIn);
            validateContext.Properties.StoreTokens([
                new () {Name = "access_token", Value = message.AccessToken},
                new () {Name = "id_token", Value = message.IdToken},
                new () {Name = "refresh_token", Value = message.RefreshToken},
                new () {Name = "token_type", Value = message.TokenType},
                new () {Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)}
                ]);
        }
    }
}
