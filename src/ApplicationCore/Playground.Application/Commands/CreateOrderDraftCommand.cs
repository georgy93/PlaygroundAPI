namespace Playground.Application.Commands;

public class CreateOrderDraftCommand : IRequest<OrderDraftDTO>
{
    public string BuyerId { get; init; }

    public IEnumerable<BasketItem> Items { get; init; } = [];

    internal class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO>
    {
        private readonly IOrderRepository _orderRepository;

        // Using DI to inject infrastructure persistence Repositories
        public CreateOrderDraftCommandHandler()
        {
        }

        public Task<OrderDraftDTO> Handle(CreateOrderDraftCommand request, CancellationToken cancellationToken)
        {
            var order = Order.NewDraft();
            var orderItems = request.Items.Select(i => i.ToOrderItemDTO());

            foreach (var item in orderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, new Uri(item.PictureUrl), item.Units);
            }

            return Task.FromResult(OrderDraftDTO.FromOrder(order));
        }
    }
}

public record BasketItem
{
    public string Id { get; init; }

    public int ProductId { get; init; }

    public string ProductName { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal OldUnitPrice { get; init; }

    public int Quantity { get; init; }

    public string PictureUrl { get; init; }
}

public static class BasketItemExtensions
{
    public static IEnumerable<OrderItemDTO> ToOrderItemsDTO(this IEnumerable<BasketItem> basketItems) => basketItems.Select(item => item.ToOrderItemDTO());

    public static OrderItemDTO ToOrderItemDTO(this BasketItem item) => new()
    {
        ProductId = item.ProductId,
        ProductName = item.ProductName,
        PictureUrl = item.PictureUrl,
        UnitPrice = item.UnitPrice,
        Units = item.Quantity
    };
}