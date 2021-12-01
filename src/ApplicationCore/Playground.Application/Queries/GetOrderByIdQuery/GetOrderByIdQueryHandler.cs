namespace Playground.Application.Queries.GetOrderByIdQuery
{
    using Common.Exceptions;
    using Dapper;
    using MediatR;
    using Microsoft.Extensions.Configuration;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public record GetOrderByIdQuery(int OrderId) : IRequest<GetOrderByIdQueryResult>
    {
        public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, GetOrderByIdQueryResult>
        {
            private readonly string _connectionString;

            public GetOrderByIdQueryHandler(IConfiguration config)
            {
                _connectionString = config.GetConnectionString("DefaultConnection");
            }

            public async Task<GetOrderByIdQueryResult> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
            {
                using var connection = new SqlConnection(_connectionString);

                await connection.OpenAsync(cancellationToken);

                // TODO: FIX DB SCHEMA and query properties
                var result = await connection.QueryAsync<dynamic>(sql:
                @"SELECT
                    o.[Id] as OrderNumber,
                    o.OrderDate as Date,
                    o.Description as Description,
                    o.ShippingAddress_City as City,
                    o.ShippingAddress_Country as Country,
                    o.ShippingAddress_State as State,
                    o.ShippingAddress_Street as Street,
                    o.ShippingAddress_ZipCode as ZipCode,
                    os.Name as Status,
                    oi.ProductName as Productname,
                    oi.Units as Units,
                    oi.UnitPrice as UnitPrice,
                    oi.PictureUrl as Pictureurl
                FROM ordering.Orders o
                LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid
                LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                WHERE o.Id=@OrderId",
                param: new { request.OrderId },
                commandTimeout: 10);

                if (!result.AsList().Any())
                    throw new RecordNotFoundException(request.OrderId); // TODO: Use other exception

                return MapOrderItems(result);
            }

            private static GetOrderByIdQueryResult MapOrderItems(dynamic result)
            {
                var total = 0m;
                var orderItems = new List<OrderitemDTO>();

                foreach (dynamic item in result)
                {
                    var orderitem = new OrderitemDTO
                    {
                        ProductName = item.productname,
                        Units = item.units,
                        UnitPrice = (double)item.unitprice,
                        PictureUrl = item.pictureurl
                    };

                    total += item.units * item.unitprice;
                    orderItems.Add(orderitem);
                }

                return new()
                {
                    OrderNumber = result[0].ordernumber,
                    Date = result[0].date,
                    Status = result[0].status,
                    Description = result[0].description,
                    Street = result[0].street,
                    City = result[0].city,
                    ZipCode = result[0].zipcode,
                    Country = result[0].country,
                    OrderItems = orderItems,
                    Total = total
                };
            }
        }
    }

    public class OrderitemDTO
    {
        public string ProductName { get; init; }

        public int Units { get; init; }

        public double UnitPrice { get; init; }

        public string PictureUrl { get; init; }
    }
}