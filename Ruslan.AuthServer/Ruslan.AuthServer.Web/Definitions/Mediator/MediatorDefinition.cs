﻿using Calabonga.AspNetCore.AppDefinitions;
using Ruslan.AuthServer.Web.Definitions.FluentValidating;
using MediatR;

namespace Ruslan.AuthServer.Web.Definitions.Mediator;

/// <summary>
/// Register Mediator as application definition
/// </summary>
public class MediatorDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="builder"></param>
    public override void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
    }
}