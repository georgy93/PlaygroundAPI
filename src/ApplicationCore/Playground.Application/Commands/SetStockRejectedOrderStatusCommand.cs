namespace Playground.Application.Commands;

public record SetStockRejectedOrderStatusCommand : IRequest<Unit>
{
    public int OrderNumber { get; init; }

    public IEnumerable<int> OrderStockItems { get; init; } = Enumerable.Empty<int>();

    internal class SetStockRejectedOrderStatusCommandHandler : IRequestHandler<SetStockRejectedOrderStatusCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public SetStockRejectedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task<Unit> Handle(SetStockRejectedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(request.OrderStockItems);

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}