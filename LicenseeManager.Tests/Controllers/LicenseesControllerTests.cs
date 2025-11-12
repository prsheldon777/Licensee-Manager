using Licensee_Manager.Models;
using LicenseeManager.Controllers;
using LicenseeManager.Models;
using LicenseeManager.Tests.TestData;
using LicenseeManager.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;

namespace LicenseeManager.Tests.Controllers
{
    /// <summary>
    /// Unit tests for <see cref="LicenseesController"/>.
    /// </summary>
    /// <remarks>
    /// Tests focus on controller action behavior (Index, Details, Create, Edit) using an in-memory EF Core context.
    /// Each test follows Arrange / Act / Assert pattern and uses <see cref="InMemoryDbHelper"/> to provide isolated contexts.
    /// </remarks>
    public class LicenseesControllerTests
    {
        /// <summary>
        /// Verifies that the Index action returns a ViewResult containing the list of licensees.
        /// </summary>
        /// <remarks>
        /// Arrange: seed a license type, office and one licensee into the in-memory context.
        /// Act: call <see cref="LicenseesController.Index"/>.
        /// Assert: the result is a view with a single licensee model and expected first name.
        /// </remarks>
        [Fact]
        public async Task Index_ReturnsView_WithLicensees()
        {
            // Arrange
            var context = InMemoryDbHelper.GetDbContext("LicenseeIndexTest");

            var licenseType = new LicenseType { LicenseTypeID = 1, Name = "Residential" };
            var office = new Office { OfficeID = 1, Name = "Main Office", City = "Raleigh", State = "NC" };
            context.LicenseType.Add(licenseType);
            context.Offices.Add(office);
            await context.SaveChangesAsync();

            context.Licensees.Add(new Licensee
            {
                FirstName = "Alice",
                LastName = "Anderson",
                Email = "alice@example.com",
                LicenseNumber = "12345",
                LicenseTypeID = 1,
                OfficeID = 1,
                IssueDate = DateTime.Today.AddYears(-1),
                ExpirationDate = DateTime.Today.AddYears(1),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
            await context.SaveChangesAsync();

            var controller = new LicenseesController(context);

            // Act
            var result = await controller.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Licensee>>(result.Model);
            Assert.Single(model);
            Assert.Equal("Alice", model.First().FirstName);
        }


        /// <summary>
        /// Ensures Details returns NotFoundResult when an invalid/nonexistent id is provided.
        /// </summary>
        /// <remarks>
        /// Arrange: empty in-memory context.
        /// Act: call <see cref="LicenseesController.Details(int?)"/> with a non-existent id.
        /// Assert: result is <see cref="NotFoundResult"/>.
        /// </remarks>
        [Fact]
        public async Task Details_ReturnsNotFound_WhenInvalidId()
        {
            var context = InMemoryDbHelper.GetDbContext("LicenseeDetailsTest");
            var controller = new LicenseesController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }


        /// <summary>
        /// Verifies that creating a valid licensee redirects to the Index action and persists the entity.
        /// </summary>
        /// <remarks>
        /// Arrange: in-memory context and a valid <see cref="Licensee"/> instance.
        /// Act: call <see cref="LicenseesController.Create(Licensee)"/>.
        /// Assert: result is RedirectToActionResult targeting "Index" and the context contains one licensee.
        /// </remarks>
        [Fact]
        public async Task Create_ValidLicensee_RedirectsToIndex()
        {
            var context = InMemoryDbHelper.GetDbContext("LicenseeCreateTest");
            var controller = new LicenseesController(context);
            var newLicensee = new Licensee
            {
                FirstName = "Bob",
                LastName = "Barker",
                Email = "bob@example.com",
                LicenseNumber = "ABC123",
                IssueDate = DateTime.Today,
                ExpirationDate = DateTime.Today.AddYears(1),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var result = await controller.Create(newLicensee);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Single(context.Licensees);
        }

        /// <summary>
        /// Confirms that when required fields are missing, Create returns the same view with the model and model state errors.
        /// </summary>
        /// <remarks>
        /// Arrange: controller with invalid model state (missing Email).
        /// Act: call Create with an invalid licensee.
        /// Assert: returns ViewResult with the same model and ModelState is invalid.
        /// </remarks>
        [Fact]
        public async Task Create_MissingRequiredFields_ReturnsSameView()
        {
            // Arrange
            var context = InMemoryDbHelper.GetDbContext("CreateInvalidMissingFields");
            var controller = new LicenseesController(context);

            // Licensee missing required fields like Email and LicenseNumber
            var invalidLicensee = new Licensee
            {
                FirstName = "John",
                LastName = "Doe",
                IssueDate = DateTime.Today,
                ExpirationDate = DateTime.Today.AddYears(1)
            };
            controller.ModelState.AddModelError("Email", "Email is required");

            // Act
            var result = await controller.Create(invalidLicensee);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.Equal(invalidLicensee, viewResult.Model);
        }

        /// <summary>
        /// Verifies that creating a licensee with an expired expiration date returns validation errors and the Create view.
        /// </summary>
        /// <remarks>
        /// Arrange: controller with ModelState simulating expiration date error.
        /// Act: call Create with an expired licensee.
        /// Assert: returns ViewResult and ModelState contains the "ExpirationDate" key.
        /// </remarks>
        [Fact]
        public async Task Create_ExpiredLicensee_ReturnsValidationError()
        {
            // Arrange
            var context = InMemoryDbHelper.GetDbContext("CreateExpiredLicensee");
            var controller = new LicenseesController(context);

            var expiredLicensee = new Licensee
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane@example.com",
                LicenseNumber = "XYZ987",
                IssueDate = DateTime.Today.AddYears(-2),
                ExpirationDate = DateTime.Today.AddDays(-1), // expired yesterday
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Simulate model validation failure
            controller.ModelState.AddModelError("ExpirationDate", "Expiration date cannot be before today");

            // Act
            var result = await controller.Create(expiredLicensee);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(controller.ModelState.IsValid);
            Assert.True(controller.ModelState.ContainsKey("ExpirationDate"));
        }

        /// <summary>
        /// Ensures Edit returns NotFound when attempting to edit a licensee that does not exist.
        /// </summary>
        /// <remarks>
        /// Arrange: create a licensee object with a non-existent LicenseeID and call Edit with that id.
        /// Act: call <see cref="LicenseesController.Edit(int, Licensee)"/>.
        /// Assert: result is <see cref="NotFoundResult"/>.
        /// </remarks>
        [Fact]
        public async Task Edit_NonexistentLicensee_ReturnsNotFound()
        {
            // Arrange
            var context = InMemoryDbHelper.GetDbContext("EditNonexistentLicensee");
            var controller = new LicenseesController(context);

            var licensee = new Licensee
            {
                LicenseeID = 999, // nonexistent ID
                FirstName = "Ghost",
                LastName = "User",
                Email = "ghost@example.com",
                LicenseNumber = "NOPE123",
                IssueDate = DateTime.Today.AddYears(-1),
                ExpirationDate = DateTime.Today.AddYears(1),
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Act
            var result = await controller.Edit(999, licensee);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Data-driven test for Create action using test data from <see cref="LicenseeTestData.CreateLicenseeData"/>.
        /// </summary>
        /// <param name="input">The input licensee instance to test.</param>
        /// <param name="expectedSuccess">Whether creation is expected to succeed.</param>
        /// <remarks>
        /// Arrange: create context and controller, optionally add model errors to simulate invalid state.
        /// Act: call Create and assert RedirectToActionResult when expectedSuccess is true, otherwise assert ViewResult and invalid ModelState.
        /// </remarks>
        [Theory]
        [MemberData(nameof(LicenseeTestData.CreateLicenseeData), MemberType = typeof(LicenseeTestData))]
        public async Task Create_LicenseeDataDriven_ReturnsExpectedResult(Licensee input, bool expectedSuccess)
        {
            // Arrange
            var context = InMemoryDbHelper.GetDbContext("LicenseeTheoryTests");
            var controller = new LicenseesController(context);

            // Simulate model validation if invalid input
            if (string.IsNullOrEmpty(input.Email))
                controller.ModelState.AddModelError("Email", "Email is required");

            if (input.ExpirationDate < DateTime.Today)
                controller.ModelState.AddModelError("ExpirationDate", "Cannot be expired");

            // Act
            var result = await controller.Create(input);

            // Assert
            if (expectedSuccess)
            {
                var redirect = Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Index", redirect.ActionName);
            }
            else
            {
                Assert.IsType<ViewResult>(result);
                Assert.False(controller.ModelState.IsValid);
            }
        }

    }
}
