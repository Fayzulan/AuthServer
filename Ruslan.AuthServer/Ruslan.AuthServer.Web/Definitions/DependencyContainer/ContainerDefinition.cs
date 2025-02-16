using Calabonga.AspNetCore.AppDefinitions;
using Ruslan.AuthServer.Web.Application.Services;
using Ruslan.AuthServer.Web.Definitions.Authorizations;

namespace Ruslan.AuthServer.Web.Definitions.DependencyContainer;

/// <summary>
/// Dependency container definition
/// </summary>
public class ContainerDefinition : AppDefinition
{
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<IAccountService, AccountService>();
        builder.Services.AddTransient<ApplicationUserClaimsPrincipalFactory>();
    }
}