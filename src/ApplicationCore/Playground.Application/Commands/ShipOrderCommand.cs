namespace Playground.Application.Commands;

public record ShipOrderCommand(int OrderNumber) : IRequest
{
    internal class ShipOrderCommandHandler : IRequestHandler<ShipOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public ShipOrderCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken) 
                ?? throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetShippedStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}