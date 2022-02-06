namespace Playground.UnitTests.Domain.BuyerAggregate
{
    using Playground.Domain.Entities.Aggregates.BuyerAggregate;
    using System;
    using Xunit;

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
            Assert.Equal(emailString, email);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("alabala.gmail.com")]
        [InlineData("4234gvr@#1254egcydcf!")]
        public void Creating_Email_From_Invalid_String_Should_Throw_Exception(string emailString)
        { 
            // Arrange
            // Act
            // Assert
            Assert.ThrowsAny<ArgumentException>(() => Email.FromString(emailString));
        }
    }
}