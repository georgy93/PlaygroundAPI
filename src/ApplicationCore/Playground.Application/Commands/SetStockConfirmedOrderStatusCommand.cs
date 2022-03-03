namespace Playground.Application.Commands;

public record SetStockConfirmedOrderStatusCommand(int OrderNumber) : IRequest<Unit>
{
    internal class SetStockConfirmedOrderStatusCommandHandler : IRequestHandler<SetStockConfirmedOrderStatusCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public SetStockConfirmedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task<Unit> Handle(SetStockConfirmedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetStockConfirmedStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}