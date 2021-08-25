namespace Playground.Application.Queries.GetOrderByIdQuery
{
    using System;
    using System.Collections.Generic;

    public class GetOrderByIdQueryResult
    {
        public int OrderNumber { get; init; }

        public DateTime Date { get; init; }

        public string Status { get; init; }

        public string Description { get; init; }

        public string Street { get; init; }

        public string City { get; init; }

        public string ZipCode { get; init; }

        public string Country { get; init; }

        public IEnumerable<OrderitemDTO> OrderItems { get; init; }

        public decimal Total { get; init; }
    }
}
