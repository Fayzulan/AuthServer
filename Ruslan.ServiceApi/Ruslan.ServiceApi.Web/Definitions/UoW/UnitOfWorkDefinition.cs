﻿using Calabonga.AspNetCore.AppDefinitions;
using Calabonga.UnitOfWork;
using Ruslan.ServiceApi.Infrastructure;

namespace Ruslan.ServiceApi.Web.Definitions.UoW
{
    /// <summary>
    /// Unit of Work registration as MicroserviceDefinition
    /// </summary>
    public class UnitOfWorkDefinition : AppDefinition
    {
        /// <summary>
        /// Configure services for current microservice
        /// </summary>
        /// <param name="builder"></param>
        public override void ConfigureServices(WebApplicationBuilder builder)
            => builder.Services.AddUnitOfWork<ApplicationDbContext>();
    }
}