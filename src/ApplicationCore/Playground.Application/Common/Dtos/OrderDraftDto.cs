namespace Playground.Application.Common.Dtos;

public record OrderDraftDto
{
    public IEnumerable<OrderItemDto> OrderItems { get; init; } = [];

    public decimal Total { get; init; }

    public static OrderDraftDto FromOrder(Order order) => new()
    {
        OrderItems = order.OrderItems.Select(oi => new OrderItemDto
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