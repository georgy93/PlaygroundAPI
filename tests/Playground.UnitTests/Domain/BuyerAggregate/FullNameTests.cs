namespace Playground.UnitTests.Domain.BuyerAggregate;

public class FullNameTests
{
    [Theory]
    [InlineData("Ivan", "Nikolov", "Ivanov")]
    [InlineData("Stoyan", "Petrov", "Atanasov")]
    public void Successfully_Create_FullName_From_String(string firstName, string surname, string lastName)
    {
        // Arrange
        // Act
        var fullName = new FullName(firstName, surname, lastName);

        // Assert
        Assert.Equal(firstName, fullName.FirstName);
        Assert.Equal(surname, fullName.Surname);
        Assert.Equal(lastName, fullName.LastName);
    }

    [Theory]
    [InlineData("Ivan", "Nikolov", "")]
    [InlineData("Stoyan", "Petrov", " ")]
    [InlineData("Stoyan", "Petrov", null)]
    [InlineData("Ivan", "", "Ivanov")]
    [InlineData("Stoyan", " ", "Atanasov")]
    [InlineData("Stoyan", null, "Atanasov")]
    [InlineData("", "Nikolov", "Ivanov")]
    [InlineData(" ", "Petrov", "Atanasov")]
    [InlineData(null, "Petrov", "Atanasov")]
    [InlineData(null, null, null)]
    [InlineData(null, "", " ")]
    [InlineData("", " ", null)]
    public void Creating_FullName_From_Invalid_String_Should_Throw_Exception(string firstName, string surname, string lastName)
    {
        // Arrange
        // Act
        // Assert
        Assert.ThrowsAny<ArgumentException>(() => new FullName(firstName, surname, lastName));
    }

    [Theory]
    [InlineData("Ivan", "Nikolov", "Ivanov")]
    [InlineData("Stoyan", "Petrov", "Atanasov")]
    public void FullName_Should_Have_Correct_String_Representation(string firstName, string surname, string lastName)
    {
        // Arrange
        var expectedFullName = $"{firstName} {surname} {lastName}";

        // Act
        var fullName = new FullName(firstName, surname, lastName);

        // Assert
        Assert.Equal(expectedFullName, fullName);
        Assert.Equal(expectedFullName, fullName.ToString());
    }
}