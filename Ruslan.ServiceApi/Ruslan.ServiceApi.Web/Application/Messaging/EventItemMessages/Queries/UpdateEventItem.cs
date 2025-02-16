using AutoMapper;
using Calabonga.Microservices.Core;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;
using Ruslan.ServiceApi.Domain;
using Ruslan.ServiceApi.Domain.Base;
using Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages.ViewModels;

namespace Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages.Queries
{
    /// <summary>
    /// Request: EventItem edit
    /// </summary>
    public sealed class PutEventItem
    {
        public record Request(Guid Id, EventItemUpdateViewModel Model) : IRequest<Operation<EventItemViewModel, string>>;

        public class Handler(IUnitOfWork unitOfWork, IMapper mapper)
            : IRequestHandler<Request, Operation<EventItemViewModel, string>>
        {
            public async Task<Operation<EventItemViewModel, string>> Handle(Request eventItemRequest, CancellationToken cancellationToken)
            {
                var repository = unitOfWork.GetRepository<EventItem>();
                var entity = await repository.GetFirstOrDefaultAsync(predicate: x => x.Id == eventItemRequest.Id, disableTracking: false);
                if (entity == null)
                {
                    return Operation.Error(AppContracts.Exceptions.NotFoundException);
                }

                mapper.Map(eventItemRequest.Model, entity);

                repository.Update(entity);
                await unitOfWork.SaveChangesAsync();

                var lastResult = unitOfWork.LastSaveChangesResult;
                if (lastResult.IsOk)
                {
                    var mapped = mapper.Map<EventItem, EventItemViewModel>(entity);
                    return mapped is not null
                        ? Operation.Result(mapped)
                        : Operation.Error(AppData.Exceptions.MappingException);
                }

                var errorMessage = lastResult.Exception?.Message ?? AppData.Exceptions.SomethingWrong;

                return Operation.Error(errorMessage);
            }
        }
    }
}