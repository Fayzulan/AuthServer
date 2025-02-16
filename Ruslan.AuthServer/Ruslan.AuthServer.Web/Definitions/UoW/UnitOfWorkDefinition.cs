using Calabonga.AspNetCore.AppDefinitions;
using Ruslan.AuthServer.Infrastructure;
using Calabonga.UnitOfWork;

namespace Ruslan.AuthServer.Web.Definitions.UoW;

/// <summary>
/// Unit of Work registration as application definition
/// </summary>
public class UnitOfWorkDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="builder"></param>
    public override void ConfigureServices(WebApplicationBuilder builder)
        => builder.Services.AddUnitOfWork<ApplicationDbContext>();
}