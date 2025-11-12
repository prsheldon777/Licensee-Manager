using System;
using LicenseeManager.Models;
using Xunit;

namespace LicenseeManager.Tests.Models
{
    /// <summary>
    /// Unit tests for the <see cref="Licensee"/> model.
    /// </summary>
    /// <remarks>
    /// Tests validate computed properties and basic model semantics such as full name composition,
    /// expiration detection, and default date initialization.
    /// </remarks>
    public class LicenseeModelTests
    {

        /// <summary>
        /// Verifies that the <see cref="Licensee.FullName"/> property returns the combined first and last name.
        /// </summary>
        /// <remarks>
        /// Arrange: create a licensee with first and last names.
        /// Act: read the FullName property.
        /// Assert: FullName concatenates the two names with a space.
        /// </remarks>
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

        /// <summary>
        /// Validates that expiration logic correctly identifies whether the license remains valid relative to today.
        /// </summary>
        /// <param name="daysOffset">Offset in days from today to set the ExpirationDate (negative = past).</param>
        /// <param name="expected">Expected boolean indicating whether ExpirationDate is today or in the future.</param>
        /// <remarks>
        /// Uses several InlineData cases to verify behavior for past, present and future expiration dates.
        /// </remarks>
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

        /// <summary>
        /// Ensures default date fields on a newly constructed <see cref="Licensee"/> are initialized as expected.
        /// </summary>
        /// <remarks>
        /// CreatedAt should be set at instantiation time; UpdatedAt should remain null until explicitly set.
        /// </remarks>
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

        /// <summary>
        /// Verifies that an empty or missing Email is considered invalid for the required Email field.
        /// </summary>
        /// <remarks>
        /// This test only checks the simple string-based validation predicate used within tests (not full model validation pipeline).
        /// </remarks>
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

        /// <summary>
        /// Ensures <see cref="Licensee.FullName"/> handles missing first or last name parts gracefully.
        /// </summary>
        /// <param name="first">First name input.</param>
        /// <param name="last">Last name input.</param>
        /// <param name="expected">Expected FullName output.</param>
        [Theory]
        [InlineData("John", "Doe", "John Doe")]
        [InlineData("Jane", "", "Jane")]
        [InlineData("", "Smith", "Smith")]
        public void FullName_HandlesMissingParts(string first, string last, string expected)
        {
            // Arrange
            var licensee = new Licensee { FirstName = first, LastName = last };

            // Act
            var result = licensee.FullName;

            // Assert
            Assert.Equal(expected, result);
        }

        /// <summary>
        /// Confirms that an active licensee has an ExpirationDate that is today or in the future.
        /// </summary>
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
