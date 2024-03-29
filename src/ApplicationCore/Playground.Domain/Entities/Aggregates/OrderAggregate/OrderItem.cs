﻿namespace Playground.Domain.Entities.Aggregates.OrderAggregate;

public class OrderItem : Entity<int>
{
    protected OrderItem() { }

    internal OrderItem(int productId, string productName, decimal unitPrice, decimal discount, Uri pictureUrl, int units = 1)
    {
        ProductId = productId;
        Units = Guard.Against.NegativeOrZero(units, nameof(units), "Invalid number of units");
        UnitPrice = Guard.Against.NegativeOrZero(unitPrice, nameof(unitPrice), "Invalid price");
        Discount = Guard.Against.Negative(discount, nameof(discount), "Invalid discount");
        ProductName = Guard.Against.NullOrWhiteSpace(productName, nameof(productName));
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
        Guard.Against.Negative(units, nameof(units), "Invalid number of units");

        Units += units;
    }

    internal void SetNewDiscount(decimal discount)
    {
        Guard.Against.Negative(discount, nameof(discount), "Discount is not valid");

        if (GetTotalWithoutDiscount() < discount)
            throw new InvalidOperationException("The total value of order item is lower than applied discount");

        Discount = discount;
    }
}