namespace Playground.Application.Commands;

public record SetPaidOrderStatusCommand(int OrderNumber) : IRequest
{
    internal class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken) 
                ?? throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetPaidStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}