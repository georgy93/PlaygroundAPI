namespace Playground.Application.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest<Unit>
{
    internal class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public ShipOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(ShipOrderCommand command, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(command.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(command.OrderNumber);

            orderToUpdate.SetShippedStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}