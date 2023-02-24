namespace Playground.Application.Commands;

public record SetStockConfirmedOrderStatusCommand(int OrderNumber) : IRequest
{
    internal class SetStockConfirmedOrderStatusCommandHandler : IRequestHandler<SetStockConfirmedOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public SetStockConfirmedOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task Handle(SetStockConfirmedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken)
                ?? throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetStockConfirmedStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}