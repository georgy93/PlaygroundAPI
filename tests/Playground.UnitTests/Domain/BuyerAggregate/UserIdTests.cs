namespace Playground.UnitTests.Domain.BuyerAggregate
{
    public class UserIdTests
    {
        [Fact]
        public void Successfully_Create_UserId_From_Valid_Guid()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var userId = new UserId(guid);

            // Assert
            Assert.Equal(guid, userId);
            Assert.Equal(guid, userId.Value);
        }

        [Fact]
        public void Creating_UserId_From_Default_Guid_Should_Throw_Exception()
        {
            // Arrange
            var guid = Guid.Empty;

            // Act
            // Assert
            Assert.ThrowsAny<ArgumentException>(() => new UserId(guid));
        }

        [Fact]
        public void UserId_Should_Have_Correct_String_Representation()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var userId = new UserId(guid);

            // Assert
            Assert.Equal(guid, userId);
            Assert.Equal(guid.ToString(), userId.ToString());
        }
    }
}