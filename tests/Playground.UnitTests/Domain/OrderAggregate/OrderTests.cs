namespace Playground.UnitTests.Domain.OrderAggregate
{
    using Mocks;

    public class OrderTests
    {
        private static readonly int DefaultBuyerId = 1;
        private static readonly Address DefaultShippingAddress = new("lala st.", "NY", "NY", "USA", "232");
        private static readonly Address DefaultBillingAddress = new("lal2a st.", "LA", "LA", "USA", "112");
        private static readonly FullName DefaultBuyerFullName = new("Dwayne", "Rock", "Johnson");
        private static readonly string DefaultUserName = "username22";

        private readonly IDateTimeService _dateTimeService;
        private readonly PaymentMethod _defaultPaymentMethod;

        public OrderTests()
        {
            _dateTimeService = new DateTimeServiceMock();
            _defaultPaymentMethod = new PaymentMethod(CardType.Visa, "dada", "23232", "212", "Hahao", DateTime.Now.AddYears(2), _dateTimeService);
        }

        [Fact]
        public void Create_New_Order_Should_Succeed_And_Add_New_Domain_Event()
        {
            // Arrange

            // Act
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

            // Assert
            Assert.Equal(DefaultBuyerId, order.BuyerId);
            Assert.Equal(_defaultPaymentMethod.Id, order.PaymentMethodId);
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
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

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
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

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
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

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
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

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
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

            // Act
            order.SetAwaitingValidationStatus();
            order.SetCancelledStatusWhenStockIsRejected(Enumerable.Empty<int>());

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.OrderStatus);
        }


        [Fact]
        public void AddOrderItem_When_OrderItem_Does_Not_Exist_Should_Add_New_Order_Item()
        {
            // Arrange
            var productId = 1;
            var productName = "product";
            var unitPrice = 2;
            var discount = 1.00m;
            var pictureUri = new Uri("https://www.google.com");
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

            // Act
            order.AddOrderItem(productId, productName, unitPrice, discount, pictureUri, 1);

            // Assert
            Assert.Single(order.OrderItems);
            Assert.Contains(order.OrderItems, x => x.Id == order.Id);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        [InlineData(5, 6)]
        public void AddOrderItem_When_OrderItem_Exists_Should_Add_More_Quantity_And_Update_Discount(int quantityToAdd, int expectedQuantity)
        {
            // Arrange
            var productId = 1;
            var productName = "product";
            var unitPrice = 2;
            var discount = 1.00m;
            var newDiscount = discount + 1;
            var pictureUri = new Uri("https://www.google.com");
            var order = Order.New(_dateTimeService, DefaultShippingAddress, DefaultBillingAddress, DefaultBuyerId, DefaultBuyerFullName, DefaultUserName, _defaultPaymentMethod);

            // Act
            order.AddOrderItem(productId, productName, unitPrice, discount, pictureUri);
            order.AddOrderItem(productId, productName, unitPrice, newDiscount, pictureUri, quantityToAdd);

            // Assert
            Assert.Single(order.OrderItems);

            var updatedOrderItem = order.OrderItems.Single(x => x.Id == order.Id);

            Assert.Equal(newDiscount, updatedOrderItem.Discount);
            Assert.Equal(expectedQuantity, updatedOrderItem.Units);
        }
    }
}