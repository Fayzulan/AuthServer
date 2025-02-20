﻿using AutoMapper;
using Ruslan.AuthServer.Domain;
using Ruslan.AuthServer.Domain.Base;
using Ruslan.AuthServer.Web.Application.Messaging.EventItemMessages.ViewModels;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;

namespace Ruslan.AuthServer.Web.Application.Messaging.EventItemMessages.Queries;

/// <summary>
/// EventItem delete
/// </summary>
public sealed class DeleteEventItem
{
    public record Request(Guid Id) : IRequest<Operation<EventItemViewModel, string>>;

    public class Handler(IUnitOfWork unitOfWork, IMapper mapper)
        : IRequestHandler<Request, Operation<EventItemViewModel, string>>
    {
        /// <summary>Handles a request</summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        public async Task<Operation<EventItemViewModel, string>> Handle(Request request, CancellationToken cancellationToken)
        {
            var repository = unitOfWork.GetRepository<EventItem>();
            var entity = await repository.FindAsync(request.Id);
            if (entity == null)
            {
                return Operation.Error("Entity not found");
            }

            repository.Delete(entity);
            await unitOfWork.SaveChangesAsync();
            if (!unitOfWork.LastSaveChangesResult.IsOk)
            {
                return Operation.Error(unitOfWork.LastSaveChangesResult.Exception?.Message ?? AppData.Exceptions.SomethingWrong);
            }

            var mapped = mapper.Map<EventItemViewModel>(entity);
            if (mapped is not null)
            {
                return Operation.Result(mapped);
            }

            return Operation.Error(AppData.Exceptions.MappingException);

        }
    }
}