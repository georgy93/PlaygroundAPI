﻿namespace Playground.Application.DomainEventHandlers;

internal class OrderShippedDomainEventHandler : INotificationHandler<OrderShippedDomainEvent>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderShippedDomainEventHandler> _logger;
    private readonly IBuyerRepository _buyerRepository;
    private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

    public OrderShippedDomainEventHandler(IOrderRepository orderRepository,
                                          ILogger<OrderShippedDomainEventHandler> logger,
                                          IBuyerRepository buyerRepository,
                                          IOrderingIntegrationEventService orderingIntegrationEventService)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _buyerRepository = buyerRepository;
        _orderingIntegrationEventService = orderingIntegrationEventService;
    }

    public async Task Handle(OrderShippedDomainEvent orderShippedDomainEvent, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Order with Id: {OrderId} has been successfully updated to status {Status}.",
            orderShippedDomainEvent.Order.Id, OrderStatus.Shipped.Name);

        var order = await _orderRepository.LoadAsync(orderShippedDomainEvent.Order.Id, CancellationToken.None);
        var buyer = await _buyerRepository.LoadAsync(order.BuyerId, CancellationToken.None);

        await _orderingIntegrationEventService.AddAndSaveEventAsync(new OrderStatusChangedToShippedIntegrationEvent
        {
            OrderId = order.Id,
            OrderStatus = order.OrderStatus.Name,
            BuyerName = buyer.FullName.ToString()
        });
    }
}