namespace Playground.UnitTests.Domain.BuyerAggregate
{
    public class EmailTests
    {
        [Theory]
        [InlineData("alabala1@abv.bg")]
        [InlineData("alabala2@yahoo.com")]
        [InlineData("alabala3@gmail.com")]
        public void Successfully_Create_Email_From_String(string emailString)
        {
            // Arrange
            // Act
            var email = Email.FromString(emailString);

            // Assert
            Assert.Equal(emailString, email.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("alabala.gmail.com")]
        [InlineData("4234gvr#1254egcydcf!")]
        public void Creating_Email_From_Invalid_String_Should_Throw_Exception(string emailString)
        { 
            // Arrange
            // Act
            // Assert
            Assert.ThrowsAny<ArgumentException>(() => Email.FromString(emailString));
        }

        [Theory]
        [InlineData("alabala1@abv.bg")]
        [InlineData("alabala2@yahoo.com")]
        [InlineData("alabala3@gmail.com")]
        public void Email_Should_Have_Correct_String_Representation(string emailString)
        {
            // Arrange
            // Act
            var email = Email.FromString(emailString);

            // Assert
            Assert.Equal(emailString, email);
            Assert.Equal(emailString, email.ToString());
        }
    }
}