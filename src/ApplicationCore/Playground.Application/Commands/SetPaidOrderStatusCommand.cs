namespace Playground.Application.Commands;

public record SetPaidOrderStatusCommand(int OrderNumber) : IRequest<Unit>
{
    internal class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(SetPaidOrderStatusCommand command, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(command.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(command.OrderNumber);

            orderToUpdate.SetPaidStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}