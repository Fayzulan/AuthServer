using Calabonga.AspNetCore.AppDefinitions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ruslan.ServiceApi.Web.Application.Messaging.ProfileMessages.Queries;
using Ruslan.ServiceApi.Web.Definitions.Authorizations;

namespace Ruslan.ServiceApi.Web.Endpoints
{
    public sealed class ProfilesEndpointDefinition : AppDefinition
    {
        public override void ConfigureApplication(WebApplication app)
            => app.MapProfilesEndpoints();
    }

    internal static class ProfilesEndpointDefinitionExtensions
    {
        public static void MapProfilesEndpoints(this IEndpointRouteBuilder routes)
        {
            var group = routes.MapGroup("/api/profiles").WithTags("Profiles");

            group.MapGet("roles", async ([FromServices] IMediator mediator, HttpContext context)
                    => await mediator.Send(new GetProfile.Request(), context.RequestAborted))
                .RequireAuthorization(x =>
                    x.AddAuthenticationSchemes(AuthData.AuthSchemes)
                        .RequireAuthenticatedUser()
                        .RequireClaim("Profiles:Roles:Get"))
                .Produces(200)
                .ProducesProblem(401)
                .WithOpenApi();
        }
    }
}