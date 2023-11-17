namespace Playground.Application.DomainEventHandlers;

public class OrderStatusChangedToAwaitingValidationDomainEventHandler : INotificationHandler<OrderStatusChangedToAwaitingValidationDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderStatusChangedToAwaitingValidationDomainEventHandler> _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderStatusChangedToAwaitingValidationDomainEventHandler(IOrderRepository orderRepository,
                                                                    ILogger<OrderStatusChangedToAwaitingValidationDomainEventHandler> logger,
                                                                    IBuyerRepository buyerRepository,
                                                                    IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _buyerRepository = buyerRepository;
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderStatusChangedToAwaitingValidationDomainEvent orderStatusChangedToAwaitingValidationDomainEvent, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status}.",
                orderStatusChangedToAwaitingValidationDomainEvent.OrderId, OrderStatus.AwaitingValidation.Name);

        var order = await _orderRepository.LoadAsync(orderStatusChangedToAwaitingValidationDomainEvent.OrderId, CancellationToken.None);
        var buyer = await _buyerRepository.LoadAsync(order.BuyerId, CancellationToken.None);

        var orderStockList = orderStatusChangedToAwaitingValidationDomainEvent
            .OrderItems
            .Select(orderItem => new OrderStockItem(orderItem.ProductId, orderItem.Units));

        await _orderingIntegrationEventService.AddAndSaveEventAsync(new OrderStatusChangedToAwaitingValidationIntegrationEvent
        {
            OrderId = order.Id,
            OrderStatus = order.OrderStatus.Name,
            BuyerName = buyer.FullName.ToString(),
            OrderStockItems = orderStockList
        });
    }
}