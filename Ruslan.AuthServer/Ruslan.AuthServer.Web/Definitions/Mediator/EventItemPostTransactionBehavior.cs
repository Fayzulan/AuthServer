using Ruslan.AuthServer.Web.Application.Messaging.EventItemMessages.ViewModels;
using Ruslan.AuthServer.Web.Definitions.Mediator.Base;
using Calabonga.OperationResults;
using Calabonga.UnitOfWork;
using MediatR;

namespace Ruslan.AuthServer.Web.Definitions.Mediator;

public class EventItemPostTransactionBehavior(IUnitOfWork unitOfWork)
    : TransactionBehavior<IRequest<Operation<EventItemViewModel>>, Operation<EventItemViewModel>>(unitOfWork);