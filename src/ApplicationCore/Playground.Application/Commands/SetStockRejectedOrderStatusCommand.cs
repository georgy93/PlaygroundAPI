namespace Playground.Application.Commands;

public record SetStockRejectedOrderStatusCommand : IRequest
{
    public int OrderNumber { get; init; }

    public IEnumerable<int> OrderStockItems { get; init; } = Enumerable.Empty<int>();

    internal class SetStockRejectedOrderStatusCommandHandler : IRequestHandler<SetStockRejectedOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public SetStockRejectedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task Handle(SetStockRejectedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken) 
                ?? throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(request.OrderStockItems);

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}