namespace Playground.Application.DomainEventHandlers;

internal class OrderStatusChangedToPaidDomainEventHandler : INotificationHandler<OrderStatusChangedToPaidDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderStatusChangedToPaidDomainEventHandler> _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderStatusChangedToPaidDomainEventHandler(IOrderRepository orderRepository,
                                                      ILogger<OrderStatusChangedToPaidDomainEventHandler> logger,
                                                      IBuyerRepository buyerRepository,
                                                      IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _buyerRepository = buyerRepository;
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToPaidDomainEvent orderStatusChangedToPaidDomainEvent, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status}",
            orderStatusChangedToPaidDomainEvent.OrderId, OrderStatus.Paid.Name);

        var order = await _orderRepository.LoadAsync(orderStatusChangedToPaidDomainEvent.OrderId, CancellationToken.None);
        var buyer = await _buyerRepository.LoadAsync(order.BuyerId, CancellationToken.None);

        var orderStockList = orderStatusChangedToPaidDomainEvent.OrderItems
            .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));

        await _orderingIntegrationEventService.AddAndSaveEventAsync(new OrderStatusChangedToPaidIntegrationEvent
        {
            OrderId = orderStatusChangedToPaidDomainEvent.OrderId,
            OrderStatus = order.OrderStatus.Name,
            BuyerName = buyer.FullName.ToString(),
            OrderStockItems = orderStockList
        });
    }
}