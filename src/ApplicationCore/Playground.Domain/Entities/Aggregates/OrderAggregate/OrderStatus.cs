namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using SeedWork;

    public class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Submitted = new(1, "Submitted");
        public static readonly OrderStatus AwaitingValidation = new(2, "AwaitingValidation");
        public static readonly OrderStatus StockConfirmed = new(3, "StockConfirmed");
        public static readonly OrderStatus Paid = new(4, "Paid");
        public static readonly OrderStatus Shipped = new(5, "Shipped");
        public static readonly OrderStatus Cancelled = new(6, "Cancelled");

        protected OrderStatus() { }

        protected OrderStatus(int id, string name) : base(id, name) { }
    }
}