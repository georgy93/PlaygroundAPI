namespace Playground.Application.Commands;

public record CreateOrderCommand : IRequest<bool>
{
    public string UserId { get; init; }

    public string UserName { get; init; }

    public string City { get; init; }

    public string Street { get; init; }

    public string State { get; init; }

    public string Country { get; init; }

    public string ZipCode { get; init; }

    public string CardNumber { get; init; }

    public string CardHolderName { get; init; }

    public DateTime CardExpiration { get; init; }

    public string CardSecurityNumber { get; init; }

    public int CardTypeId { get; init; }

    public IEnumerable<OrderItemDTO> OrderItems { get; init; } = Enumerable.Empty<OrderItemDTO>();

    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ILogger<CreateOrderCommandHandler> _logger;

        // Using DI to inject infrastructure persistence Repositories
        public CreateOrderCommandHandler(
            IOrderingIntegrationEventService orderingIntegrationEventService,
            IOrderRepository orderRepository,
            ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
            _orderingIntegrationEventService = Guard.Against.Null(orderingIntegrationEventService);
            _logger = Guard.Against.Null(logger);
        }

        public async Task<bool> Handle(CreateOrderCommand message, CancellationToken cancellationToken)
        {
            // Add Integration event to clean the basket
            var orderStartedIntegrationEvent = new OrderStartedIntegrationEvent(message.UserId);
            await _orderingIntegrationEventService.AddAndSaveEventAsync(orderStartedIntegrationEvent);

            // Add/Update the Buyer AggregateRoot
            // DDD patterns comment: Add child entities and value-objects through the Order Aggregate-Root
            // methods and constructor so validations, invariants and business logic 
            // make sure that consistency is preserved across the whole aggregate
            var address = new Address(message.Street, message.City, message.State, message.Country, message.ZipCode);
            //  var order = new Order(message.UserId, message.UserName, address, message.CardTypeId, message.CardNumber, message.CardSecurityNumber, message.CardHolderName, message.CardExpiration);
            Order order = null;
            foreach (var item in message.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, new Uri(item.PictureUrl), item.Units);
            }

            _logger.LogInformation("----- Creating Order - Order: {@Order}", order);

            await _orderRepository.AddAsync(order);

            return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
    }
}

public record OrderItemDTO
{
    public int ProductId { get; init; }

    public string ProductName { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal Discount { get; init; }

    public int Units { get; init; }

    public string PictureUrl { get; init; }
}