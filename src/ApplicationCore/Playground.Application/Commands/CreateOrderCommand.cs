namespace Playground.Application.Commands;

public record CreateOrderCommand : IRequest<Unit>
{
    public long UserId { get; init; }

    public string UserName { get; init; }

    public string FirstName { get; init; }

    public string Surname { get; init; }

    public string LastName { get; init; }

    public AddressDto ShippingAddress { get; init; }

    public AddressDto BillingAddress { get; init; }

    public string CardNumber { get; init; }

    public string CardHolderName { get; init; }

    public DateTime CardExpiration { get; init; }

    public string CardSecurityNumber { get; init; }

    public int CardTypeId { get; init; }

    public IEnumerable<OrderItemDTO> OrderItems { get; init; } = Enumerable.Empty<OrderItemDTO>();

    internal class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        private readonly IDateTimeService _dateTimeService;

        // Using DI to inject infrastructure persistence Repositories
        public CreateOrderCommandHandler(IOrderingIntegrationEventService orderingIntegrationEventService,
                                         IOrderRepository orderRepository,
                                         ILogger<CreateOrderCommandHandler> logger,
                                         IDateTimeService dateTimeService)
        {
            _orderRepository = Guard.Against.Null(orderRepository);
            _orderingIntegrationEventService = Guard.Against.Null(orderingIntegrationEventService);
            _logger = Guard.Against.Null(logger);
            _dateTimeService = Guard.Against.Null(dateTimeService);
        }

        public async Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var shippingAddress = new Address(request.ShippingAddress.Street, request.ShippingAddress.City, request.ShippingAddress.State, request.ShippingAddress.Country, request.ShippingAddress.ZipCode);
            var billingAddress = new Address(request.BillingAddress.Street, request.BillingAddress.City, request.BillingAddress.State, request.BillingAddress.Country, request.BillingAddress.ZipCode);
            var cardType = CardType.FromValue(request.CardTypeId);
            var paymentMethod = new PaymentMethod(cardType, "", request.CardNumber, request.CardSecurityNumber, request.CardHolderName, request.CardExpiration, _dateTimeService);
            var fullName = new FullName(request.FirstName, request.Surname, request.LastName);

            await _orderingIntegrationEventService.AddAndSaveEventAsync(new OrderStartedIntegrationEvent(request.UserId));

            var order = Order.New(_dateTimeService, shippingAddress, billingAddress, request.UserId, fullName, request.UserName, paymentMethod);

            foreach (var item in request.OrderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, new Uri(item.PictureUrl), item.Units);
            }

            _logger.LogInformation("----- Creating Order - Order: {@Order}", order);

            await _orderRepository.AddAsync(order);
            await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}