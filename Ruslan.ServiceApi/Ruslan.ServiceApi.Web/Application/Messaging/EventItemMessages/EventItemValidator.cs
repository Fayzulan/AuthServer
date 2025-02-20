﻿using FluentValidation;
using Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages.Queries;

namespace Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages
{
    /// <summary>
    /// RegisterViewModel Validator
    /// </summary>
    public class EventItemCreateRequestValidator : AbstractValidator<PostEventItem.Request>
    {
        public EventItemCreateRequestValidator() => RuleSet("default", () =>
        {
            RuleFor(x => x.Model.CreatedAt).NotNull();
            RuleFor(x => x.Model.Message).NotEmpty().MaximumLength(4000);
            RuleFor(x => x.Model.Level).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Model.Logger).NotEmpty().MaximumLength(255);
            RuleFor(x => x.Model.ThreadId).MaximumLength(50);
            RuleFor(x => x.Model.ExceptionMessage).MaximumLength(4000);
        });
    }
}