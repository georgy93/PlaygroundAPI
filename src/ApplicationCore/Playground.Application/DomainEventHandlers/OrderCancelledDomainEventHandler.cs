﻿namespace Playground.Application.DomainEventHandlers;

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
        _orderRepository = orderRepository;
        _logger = logger;
        _buyerRepository = buyerRepository;
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderCancelledDomainEvent orderCancelledDomainEvent, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status}.",
            orderCancelledDomainEvent.Order.Id, OrderStatus.Cancelled.Name);

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