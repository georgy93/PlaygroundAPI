namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using Ardalis.GuardClauses;
    using SeedWork;
    using System;

    public class OrderItem : Entity<int>
    {
        protected OrderItem() { }

        public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, Uri pictureUrl, int units = 1)
        {
            Guard.Against.NegativeOrZero(units, nameof(units), "Invalid number of units");
            Guard.Against.NegativeOrZero(unitPrice, nameof(unitPrice), "Invalid price");
            Guard.Against.Negative(discount, nameof(discount), "Invalid discount");

            if ((unitPrice * units) < discount)
                throw new InvalidOperationException("The total value of order item is lower than applied discount");

            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Units = units;
            Discount = discount;
            PictureUrl = pictureUrl.ToString();
        }

        public decimal Discount { get; private set; }

        public string PictureUrl { get; private set; }

        public int ProductId { get; private set; }

        public string ProductName { get; private set; }

        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public decimal GetTotalWithoutDiscount() => Units * UnitPrice;

        public decimal GetTotalWithDiscount() => GetTotalWithoutDiscount() - Discount;

        public void AddUnits(int units)
        {
            Guard.Against.Negative(units, nameof(units), "Invalid number of units");

            Units += units;
        }

        public void SetNewDiscount(decimal discount)
        {
            Guard.Against.Negative(discount, nameof(discount), "Discount is not valid");

            if (GetTotalWithoutDiscount() < discount)
                throw new InvalidOperationException("The total value of order item is lower than applied discount");

            Discount = discount;
        }
    }
}