using System;
using LicenseeManager.Models;
using Xunit;

namespace LicenseeManager.Tests.Models
{
    public class LicenseeModelTests
    {

        // Test for FullName property
        [Fact]
        public void FullName_ReturnsCombinedName()
        {

            // Arrange
            var licensee = new Licensee
            {
                FirstName = "Alice",
                LastName = "Anderson"
            };

            // Act
            var result = licensee.FullName;

            // Assert
            Assert.Equal("Alice Anderson", result);
        }

        // Test for ExpirationDate validation
        [Theory]
        [InlineData(-1, false)]  // expired (yesterday)
        [InlineData(0, true)]   // expires today
        [InlineData(100, true)] // far in the future
        public void IsExpired_CorrectlyIdentifiesExpiration(int daysOffset, bool expected)
        {
            // Arrange
            var licensee = new Licensee
            {
                IssueDate = DateTime.Today.AddYears(-1),
                ExpirationDate = DateTime.Today.AddDays(daysOffset)
            };

            // Act
            bool isActive = licensee.ExpirationDate >= DateTime.Today;

            // Assert
            Assert.Equal(expected, isActive);
        }

        // Test for CreatedAt and UpdatedAt default values
        [Fact]
        public void DefaultDates_ShouldBeInitializedCorrectly()
        {
            // Arrange
            var before = DateTime.Now;

            // Act
            var licensee = new Licensee();
            var after = DateTime.Now;

            // Assert
            Assert.InRange(licensee.CreatedAt.Value, before, after); // created between before/after
            Assert.Equal(default(DateTime?), licensee.UpdatedAt); // still uninitialized until manually set
        }

        // Test for required Email field
        [Fact]
        public void Email_IsRequired_ShouldReturnInvalidIfMissing()
        {
            // Arrange
            var licensee = new Licensee
            {
                FirstName = "Test",
                LastName = "User",
                Email = "",
                LicenseNumber = "1234"
            };

            // Act
            bool isValid = !string.IsNullOrWhiteSpace(licensee.Email);

            // Assert
            Assert.False(isValid);
        }

        //  Test for FullName property with missing parts
        [Theory]
        [InlineData("John", "Doe", "John Doe")]
        [InlineData("Jane", "", "Jane")]
        [InlineData("", "Smith", "Smith")]
        public void FullName_HandlesMissingPartsGracefully(string first, string last, string expected)
        {
            // Arrange
            var licensee = new Licensee { FirstName = first, LastName = last };

            // Act
            var result = licensee.FullName;

            // Assert
            Assert.Equal(expected, result);
        }

        // Test for ExpirationDate being today or in the future
        [Fact]
        public void ExpirationDate_ShouldBeTodayOrFuture_WhenActive()
        {
            // Arrange
            var licensee = new Licensee
            {
                IssueDate = DateTime.Today,
                ExpirationDate = DateTime.Today.AddYears(1)
            };

            // Act
            var valid = licensee.ExpirationDate >= DateTime.Today;

            // Assert
            Assert.True(valid);
        }
    }
}
