namespace Playground.Application.Commands;

public record SetAwaitingValidationOrderStatusCommand(int OrderNumber) : IRequest
{
    internal class SetAwaitingValidationOrderStatusCommandHandler : IRequestHandler<SetAwaitingValidationOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;

        public SetAwaitingValidationOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task Handle(SetAwaitingValidationOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken) 
                ?? throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetAwaitingValidationStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}