namespace Playground.UnitTests.Domain.OrderAggregate
{
    using Mocks;
    using Playground.Domain.Entities.Aggregates.BuyerAggregate;
    using Playground.Domain.Entities.Aggregates.OrderAggregate;
    using Playground.Domain.Events;
    using Playground.Domain.Services;
    using System;
    using System.Linq;
    using Xunit;

    public class OrderTests
    {
        private static readonly Address DefaultShippingAddress = new("lala st.", "NY", "NY", "USA", "232");
        private static readonly Address DefaultBillingAddress = new("lal2a st.", "LA", "LA", "USA", "112");
        private static readonly FullName DefaultBuyerFullName = new("Dwayne", "Rock", "Johnson");

        private readonly IDateTimeService _dateTimeService;

        public OrderTests()
        {
            _dateTimeService = new DateTimeServiceMock();
        }

        [Fact]
        public void Create_New_Order_Should_Succeed_And_Add_New_Domain_Event()
        {
            // Arrange
            var buyerId = 1;
            var paymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);

            // Act
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, buyerId, DefaultBuyerFullName, paymentMethod);

            // Assert
            Assert.Equal(buyerId, order.BuyerId);
            Assert.Equal(paymentMethod.Id, order.PaymentMethodId);
            Assert.Equal(DefaultShippingAddress, order.ShippingAddress);
            Assert.Equal(DefaultBillingAddress, order.BillingAddress);
            Assert.Equal(OrderStatus.Submitted, order.OrderStatus);
            // Assert.Equal(, order.OrderItems);
            Assert.Single(order.DomainEvents);
            Assert.Contains(order.DomainEvents, ev => ev.GetType() == typeof(OrderStartedDomainEvent));
        }

        [Fact]
        public void Processing_Order_From_Creation_To_Shipped_Should_Create_Correct_Domain_Events_And_Have_Correct_Statuses()
        {
            // Arrange
            var buyerId = 1;
            var paymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, buyerId, DefaultBuyerFullName, paymentMethod);

            // Act - Assert
            order.SetAwaitingValidationStatus();
            Assert.Equal(OrderStatus.AwaitingValidation, order.OrderStatus);
            Assert.Contains(order.DomainEvents, ev => ev.GetType() == typeof(OrderStatusChangedToAwaitingValidationDomainEvent));

            order.SetStockConfirmedStatus();
            Assert.Equal(OrderStatus.StockConfirmed, order.OrderStatus);
            Assert.Contains(order.DomainEvents, ev => ev.GetType() == typeof(OrderStatusChangedToStockConfirmedDomainEvent));

            order.SetPaidStatus();
            Assert.Equal(OrderStatus.Paid, order.OrderStatus);
            Assert.Contains(order.DomainEvents, ev => ev.GetType() == typeof(OrderStatusChangedToPaidDomainEvent));

            order.SetShippedStatus();
            Assert.Equal(OrderStatus.Shipped, order.OrderStatus);
            Assert.Contains(order.DomainEvents, ev => ev.GetType() == typeof(OrderShippedDomainEvent));
        }

        [Fact]
        public void Cancel_Order_Should_Succeed_If_Not_Yet_Shipped_Or_Paid()
        {
            // Arrange
            var buyerId = 1;
            var paymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, buyerId, DefaultBuyerFullName, paymentMethod);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetCancelledStatus();

            // Assert             
            Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
            Assert.Contains(order.DomainEvents, ev => ev.GetType() == typeof(OrderCancelledDomainEvent));
        }

        [Fact]
        public void Cancel_Order_When_Paid_Should_Throw_Exceptio()
        {
            // Arrange
            var buyerId = 1;
            var paymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, buyerId, DefaultBuyerFullName, paymentMethod);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();

            // Assert             
            Assert.Throws<InvalidOperationException>(() => order.SetCancelledStatus());
        }

        [Fact]
        public void Cancel_Order_When_Shipped_Should_Throw_Exceptio()
        {
            // Arrange
            var buyerId = 1;
            var paymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, buyerId, DefaultBuyerFullName, paymentMethod);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetStockConfirmedStatus();
            order.SetPaidStatus();
            order.SetShippedStatus();

            // Assert             
            Assert.Throws<InvalidOperationException>(() => order.SetCancelledStatus());
        }

        [Fact]
        public void SetCancelledStatusWhenStockIsRejected_When_AwaitingValidation_Should_Cancel_Order()
        {
            // Arrange
            var buyerId = 1;
            var paymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, buyerId, DefaultBuyerFullName, paymentMethod);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetCancelledStatusWhenStockIsRejected(Enumerable.Empty<int>());

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
        }
    }
}