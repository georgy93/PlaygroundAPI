namespace Playground.UnitTests.Domain.OrderAggregate;

public class OrderItemTests
{
    private const int DefaultProductId = 1;
    private const string DefaultProductName = "product name";

    private readonly Uri DefaultPictureUri = new("https://www.google.com");

    [Fact]
    public void OrderItem_Should_Be_Created_With_One_Unit_Successfully()
    {
        // Arrange
        var unitPrice = 10.23m;
        var discount = 0.2m;

        // Act
        var orderitem = new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri);

        // Assert
        Assert.Equal(DefaultProductId, orderitem.ProductId);
        Assert.Equal(DefaultProductName, orderitem.ProductName);
        Assert.Equal(unitPrice, orderitem.UnitPrice);
        Assert.Equal(discount, orderitem.Discount);
        Assert.Equal(DefaultPictureUri.ToString(), orderitem.PictureUri);
        Assert.Equal(1, orderitem.Units);
    }

    [Theory]
    [InlineData(-2.23)]
    [InlineData(0)]
    public void Creating_OrderItem_With_Negative_Or_Zero_Unit_Price_Should_Throw_Exception(decimal unitPrice)
    {
        // Arrange
        var discount = 0.2m;

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri));
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(0)]
    public void Creating_OrderItem_With_Negative_Or_Zero_Units_Should_Throw_Exception(int units)
    {
        // Arrange
        var unitPrice = 10.23m;
        var discount = 0.2m;

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri, units));
    }

    [Fact]
    public void Creating_OrderItem_With_Discount_Greater_Than_Value_Should_Throw_Exception()
    {
        // Arrange
        var unitPrice = 10.23m;
        var units = 1;
        var discount = unitPrice + 0.1m;

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri, units));
    }

    [Theory]
    [InlineData(-2.54)]
    [InlineData(-3.11)]
    [InlineData(-10.01)]
    public void Creating_OrderItem_With_Negative_Discount_Should_Throw_Exception(decimal negativeDiscount)
    {
        // Arrange
        var unitPrice = 10.23m;

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => new OrderItem(DefaultProductId, DefaultProductName, unitPrice, negativeDiscount, DefaultPictureUri));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(10)]
    public void Adding_More_Units_To_OrderItem_Should_Be_Successful(int unitsToAdd)
    {
        // Arrange
        var unitPrice = 10.23m;
        var discount = 0.2m;
        var orderitem = new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri);

        // Act
        orderitem.AddUnits(unitsToAdd);

        // Assert
        var expectedUnits = unitsToAdd + 1;

        Assert.Equal(expectedUnits, orderitem.Units);
    }

    [Theory]
    [InlineData(2, 2.5)]
    [InlineData(3, 4.0)]
    [InlineData(10, 3.2)]
    public void Get_Total_Without_Discount_Should_Return_Correct_Price(int units, decimal unitPrice)
    {
        // Arrange
        var discount = 0;
        var orderitem = new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri, units);

        // Act
        var totalWithoutDiscount = orderitem.GetTotalWithoutDiscount();

        // Assert
        var expectedTotal = units * unitPrice;

        Assert.Equal(expectedTotal, totalWithoutDiscount);
    }

    [Theory]
    [InlineData(2, 2.5, 2)]
    [InlineData(3, 4.0, 3)]
    [InlineData(10, 3.2, 10)]
    public void Get_Total_With_Discount_Should_Return_Correct_Price(int units, decimal unitPrice, decimal discountValue)
    {
        // Arrange
        var orderitem = new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discountValue, DefaultPictureUri, units);

        // Act
        var totalWithDiscount = orderitem.GetTotalWithDiscount();

        // Assert
        var expectedTotal = (units * unitPrice) - discountValue;

        Assert.Equal(expectedTotal, totalWithDiscount);
    }

    [Fact]
    public void Setting_OrderItem_Discount_Greater_Than_Value_Should_Throw_Exception()
    {
        // Arrange
        var unitPrice = 10.23m;
        var discount = 0;
        var orderItem = new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri);
        var newDiscounrt = unitPrice + 0.1m;

        // Act
        // Assert
        Assert.Throws<InvalidOperationException>(() => orderItem.SetNewDiscount(newDiscounrt));
    }

    [Theory]
    [InlineData(-2.54)]
    [InlineData(-3.11)]
    [InlineData(-10.01)]
    public void Setting_OrderItem_With_Negative_Discount_Should_Throw_Exception(decimal negativeDiscount)
    {
        // Arrange
        var unitPrice = 10.23m;
        var discount = 0;
        var orderItem = new OrderItem(DefaultProductId, DefaultProductName, unitPrice, discount, DefaultPictureUri);

        // Act
        // Assert
        Assert.Throws<ArgumentException>(() => orderItem.SetNewDiscount(negativeDiscount));
    }
}