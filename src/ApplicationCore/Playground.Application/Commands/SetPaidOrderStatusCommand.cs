namespace Playground.Application.Commands;

public record SetPaidOrderStatusCommand(int OrderNumber) : IRequest<Unit>
{
    internal class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task<Unit> Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetPaidStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}