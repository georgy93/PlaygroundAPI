namespace Playground.Domain.Entities.Aggregates.OrderAggregate;

public class OrderItem : Entity<int>
{
    protected OrderItem() { }

    internal OrderItem(int productId, string productName, decimal unitPrice, decimal discount, Uri pictureUrl, int units = 1)
    {
        ProductId = productId;
        Units = Guard.Against.NegativeOrZero(units, message: "Invalid number of units");
        UnitPrice = Guard.Against.NegativeOrZero(unitPrice, message: "Invalid price");
        Discount = Guard.Against.Negative(discount, message: "Invalid discount");
        ProductName = Guard.Against.NullOrWhiteSpace(productName);
        PictureUri = Guard.Against.Null(pictureUrl).ToString();

        if (GetTotalWithoutDiscount() < discount)
            throw new InvalidOperationException("The total value of order item is lower than applied discount");
    }

    public decimal Discount { get; private set; }

    public string PictureUri { get; private set; }

    public int ProductId { get; private set; }

    public string ProductName { get; private set; }

    public decimal UnitPrice { get; private set; }

    public int Units { get; private set; }

    public decimal GetTotalWithoutDiscount() => Units * UnitPrice;

    public decimal GetTotalWithDiscount() => GetTotalWithoutDiscount() - Discount;

    internal void AddUnits(int units)
    {
        Guard.Against.Negative(units, message: "Invalid number of units");

        Units += units;
    }

    internal void SetNewDiscount(decimal discount)
    {
        Guard.Against.Negative(discount, message: "Discount is not valid");

        if (GetTotalWithoutDiscount() < discount)
            throw new InvalidOperationException("The total value of order item is lower than applied discount");

        Discount = discount;
    }
}