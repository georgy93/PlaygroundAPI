﻿namespace Playground.Application.Commands;

public record SetAwaitingValidationOrderStatusCommand(int OrderNumber) : IRequest<Unit>
{
    internal class SetAwaitingValidationOrderStatusCommandHandler : IRequestHandler<SetAwaitingValidationOrderStatusCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;

        public SetAwaitingValidationOrderStatusCommandHandler(IOrderRepository orderRepository)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
        }

        public async Task<Unit> Handle(SetAwaitingValidationOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.LoadAsync(request.OrderNumber, cancellationToken);
            if (orderToUpdate is null)
                throw new RecordNotFoundException(request.OrderNumber);

            orderToUpdate.SetAwaitingValidationStatus();

            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}