namespace Playground.Domain.Entities.Aggregates.OrderAggregate
{
    using SeedWork;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderStatus : Enumeration
    {
        public static readonly OrderStatus Submitted = new(1, "Submitted");
        public static readonly OrderStatus AwaitingValidation = new(2, "AwaitingValidation");
        public static readonly OrderStatus StockConfirmed = new(3, "StockConfirmed");
        public static readonly OrderStatus Paid = new(4, "Paid");
        public static readonly OrderStatus Shipped = new(5, "Shipped");
        public static readonly OrderStatus Cancelled = new(6, "Cancelled");

        public OrderStatus(int id, string name) : base(id, name)
        { }

        public static IEnumerable<OrderStatus> List() => new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Shipped, Cancelled };

        public static OrderStatus FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => string.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static OrderStatus From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new Exception($"Possible values for OrderStatus: {string.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }
    }
}