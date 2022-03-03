namespace Playground.Application.Common.Dtos
{
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
}