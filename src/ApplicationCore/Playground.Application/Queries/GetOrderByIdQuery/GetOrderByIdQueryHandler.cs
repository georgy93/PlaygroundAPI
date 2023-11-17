namespace Playground.Application.Queries.GetOrderByIdQuery;

using Common.Exceptions;  
using Domain.Entities.Aggregates.OrderAggregate;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

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
            // https://github.com/Drizin/DapperQueryBuilder
            var query = connection.QueryBuilder(
               @$"SELECT
                    o.{nameof(Order.Id):raw} as {nameof(GetOrderByIdQueryResult.OrderNumber):raw},
                    o.{nameof(Order.OrderDate):raw} as {nameof(GetOrderByIdQueryResult.Date):raw},
                    o.Description as {nameof(GetOrderByIdQueryResult.Description):raw},
                    o.ShippingCity as {nameof(GetOrderByIdQueryResult.City):raw},
                    o.ShippingCountry as {nameof(GetOrderByIdQueryResult.Country):raw},
                    o.ShippingStreet as {nameof(GetOrderByIdQueryResult.Street):raw},
                    o.ShippingZipCode as {nameof(GetOrderByIdQueryResult.ZipCode):raw},
                    os.Name as {nameof(GetOrderByIdQueryResult.Status):raw},
                    oi.{nameof(OrderItem.ProductName):raw} as {nameof(OrderitemDto.ProductName):raw},
                    oi.{nameof(OrderItem.Units):raw} as {nameof(OrderitemDto.Units):raw},
                    oi.{nameof(OrderItem.UnitPrice):raw} as {nameof(OrderitemDto.UnitPrice):raw},
                    oi.{nameof(OrderItem.PictureUri):raw} as {nameof(OrderitemDto.PictureUrl):raw}
                FROM ordering.Orders o
                LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid
                LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                WHERE o.Id = {request.OrderId}");

            var result = await query.QueryAsync<dynamic>(commandTimeout: 10);

            if (!result.Any())
                throw new RecordNotFoundException(request.OrderId); // TODO: Use other exception

            return MapOrderItems(result);
        }

        private static GetOrderByIdQueryResult MapOrderItems(dynamic result)
        {
            var total = 0m;
            var orderItems = new List<OrderitemDto>();

            foreach (dynamic item in result)
            {
                var orderitem = new OrderitemDto
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

public record OrderitemDto
{
    public string ProductName { get; init; }

    public int Units { get; init; }

    public double UnitPrice { get; init; }

    public string PictureUrl { get; init; }
}