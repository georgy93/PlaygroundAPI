namespace Playground.Application.DomainEventHandlers;

internal class OrderCancelledDomainEventHandler : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderCancelledDomainEventHandler> _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderCancelledDomainEventHandler(IOrderRepository orderRepository,
                                            ILogger<OrderCancelledDomainEventHandler> logger,
                                            IBuyerRepository buyerRepository,
                                            IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = Guard.Against.Null(orderRepository);
        _logger = Guard.Against.Null(logger);
        _buyerRepository = Guard.Against.Null(buyerRepository);
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderCancelledDomainEvent orderCancelledDomainEvent, CancellationToken cancellationToken)
    {
        _logger.LogTrace($"Order with Id: {orderCancelledDomainEvent.Order.Id} has been successfully updated to status {OrderStatus.Cancelled.Name}.");

        var order = await _orderRepository.LoadAsync(orderCancelledDomainEvent.Order.Id, CancellationToken.None);
        var buyer = await _buyerRepository.LoadAsync(order.BuyerId, CancellationToken.None);

        await _orderingIntegrationEventService.AddAndSaveEventAsync(new OrderStatusChangedToCancelledIntegrationEvent
        {
            OrderId = order.Id,
            OrderStatus = order.OrderStatus.Name,
            BuyerName = buyer.FullName.ToString()
        });
    }
}