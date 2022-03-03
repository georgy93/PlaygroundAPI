using Playground.Application.Commands;

public class CreateOrderDraftCommand : IRequest<OrderDraftDTO>
{
    public string BuyerId { get; init; }

    public IEnumerable<BasketItem> Items { get; init; } = Enumerable.Empty<BasketItem>();


    internal class CreateOrderDraftCommandHandler : IRequestHandler<CreateOrderDraftCommand, OrderDraftDTO>
    {
        private readonly IOrderRepository _orderRepository;

        // Using DI to inject infrastructure persistence Repositories
        public CreateOrderDraftCommandHandler()
        {
        }

        public Task<OrderDraftDTO> Handle(CreateOrderDraftCommand message, CancellationToken cancellationToken)
        {
            var order = Order.NewDraft();
            var orderItems = message.Items.Select(i => i.ToOrderItemDTO());

            foreach (var item in orderItems)
            {
                order.AddOrderItem(item.ProductId, item.ProductName, item.UnitPrice, item.Discount, new Uri(item.PictureUrl), item.Units);
            }

            return Task.FromResult(OrderDraftDTO.FromOrder(order));
        }
    }

}
public record OrderDraftDTO
{
    public IEnumerable<OrderItemDTO> OrderItems { get; init; } = Enumerable.Empty<OrderItemDTO>();

    public decimal Total { get; init; }

    public static OrderDraftDTO FromOrder(Order order) => new()
    {
        OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
        {
            Discount = oi.Discount,
            ProductId = oi.ProductId,
            UnitPrice = oi.UnitPrice,
            PictureUrl = oi.PictureUri,
            Units = oi.Units,
            ProductName = oi.ProductName
        }),
        Total = order.GetTotal()
    };
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
    public static IEnumerable<OrderItemDTO> ToOrderItemsDTO(this IEnumerable<BasketItem> basketItems)
    {
        foreach (var item in basketItems)
        {
            yield return item.ToOrderItemDTO();
        }
    }

    public static OrderItemDTO ToOrderItemDTO(this BasketItem item)
    {
        return new OrderItemDTO()
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            PictureUrl = item.PictureUrl,
            UnitPrice = item.UnitPrice,
            Units = item.Quantity
        };
    }
}

