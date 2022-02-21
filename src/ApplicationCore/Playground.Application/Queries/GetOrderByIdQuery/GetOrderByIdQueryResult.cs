namespace Playground.Application.Queries.GetOrderByIdQuery
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public record GetOrderByIdQueryResult
    {
        public int OrderNumber { get; init; }

        public DateTime Date { get; init; }

        public string Status { get; init; }

        public string Description { get; init; }

        public string Street { get; init; }

        public string City { get; init; }

        public string ZipCode { get; init; }

        public string Country { get; init; }

        public IEnumerable<OrderitemDto> OrderItems { get; init; } = Enumerable.Empty<OrderitemDto>();

        public decimal Total { get; init; }
    }
}