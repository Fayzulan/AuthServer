using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;
using Ruslan.ServiceApi.Web.Application.Messaging.EventItemMessages.ViewModels;
using Ruslan.ServiceApi.Web.Definitions.Mediator.Base;

namespace Ruslan.ServiceApi.Web.Definitions.Mediator
{
    public class LogPostTransactionBehavior : TransactionBehavior<IRequest<Operation<EventItemViewModel>>, Operation<EventItemViewModel>>
    {
        public LogPostTransactionBehavior(IUnitOfWork unitOfWork) : base(unitOfWork) { }
    }
}