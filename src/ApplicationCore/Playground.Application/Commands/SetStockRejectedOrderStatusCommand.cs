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
            _orderRepository = orderRepository;
        }

        public async Task<Unit> Handle(SetStockRejectedOrderStatusCommand command, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(command.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(command.OrderNumber);

            orderToUpdate.SetCancelledStatusWhenStockIsRejected(command.OrderStockItems);

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}