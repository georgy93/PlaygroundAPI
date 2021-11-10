namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using SeedWork;
    using System;

    public class OrderItem : Entity<int>
    {
        protected OrderItem() { }

        public OrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
        {
            if (units <= 0)
                throw new InvalidOperationException("Invalid number of units");

            if ((unitPrice * units) < discount)
                throw new InvalidOperationException("The total of order item is lower than applied discount");

            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Units = units;
            Discount = discount;
            PictureUrl = pictureUrl;
        }

        public decimal Discount { get; private set; }

        public string PictureUrl { get; init; }

        public int ProductId { get; init; }

        public string ProductName { get; init; }

        public decimal UnitPrice { get; private set; }

        public int Units { get; private set; }

        public void AddUnits(int units)
        {
            if (units < 0)
                throw new InvalidOperationException("Invalid units");

            UnitPrice += units;
        }

        public void SetNewDiscount(decimal discount)
        {
            if (discount < 0)
                throw new InvalidOperationException("Discount is not valid");

            Discount = discount;
        }
    }
}