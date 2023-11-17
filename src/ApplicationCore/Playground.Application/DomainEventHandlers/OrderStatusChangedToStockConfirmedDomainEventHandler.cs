namespace Playground.Application.DomainEventHandlers;

public class OrderStatusChangedToStockConfirmedDomainEventHandler : INotificationHandler<OrderStatusChangedToStockConfirmedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;
    private readonly ILogger<OrderStatusChangedToStockConfirmedDomainEventHandler> _logger;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderStatusChangedToStockConfirmedDomainEventHandler(IOrderRepository orderRepository,
                                                                IBuyerRepository buyerRepository,
                                                                ILogger<OrderStatusChangedToStockConfirmedDomainEventHandler> logger,
                                                                IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _buyerRepository = buyerRepository;
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToStockConfirmedDomainEvent orderStatusChangedToStockConfirmedDomainEvent, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status}.",
            orderStatusChangedToStockConfirmedDomainEvent.OrderId, OrderStatus.StockConfirmed.Name);

        var order = await _orderRepository.LoadAsync(orderStatusChangedToStockConfirmedDomainEvent.OrderId, CancellationToken.None);
        var buyer = await _buyerRepository.LoadAsync(order.BuyerId, CancellationToken.None);

        await _orderingIntegrationEventService.AddAndSaveEventAsync(new OrderStatusChangedToStockConfirmedIntegrationEvent
        {
            OrderId = order.Id,
            OrderStatus = order.OrderStatus.Name,
            BuyerName = buyer.FullName
        });
    }
}