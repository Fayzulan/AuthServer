﻿using AutoMapper;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;
using Ruslan.ServiceApi.Domain;
using Ruslan.ServiceApi.Domain.Base;
using Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages.ViewModels;

namespace Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages.Queries
{
    /// <summary>
    /// Request: EventItem delete
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
                    return Operation.Error(AppData.Exceptions.NotFoundException);
                }

                repository.Delete(entity);
                await unitOfWork.SaveChangesAsync();
                if (!unitOfWork.LastSaveChangesResult.IsOk)
                {
                    return Operation.Error(unitOfWork.LastSaveChangesResult.Exception?.Message ?? AppData.Exceptions.SomethingWrong);
                }

                var mapped = mapper.Map<EventItemViewModel>(entity);

                return mapped is not null
                    ? Operation.Result(mapped)
                    : Operation.Error(AppData.Exceptions.MappingException);
            }
        }
    }
}