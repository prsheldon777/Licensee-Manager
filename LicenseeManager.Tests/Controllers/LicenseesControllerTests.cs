using Licensee_Manager.Models;
using LicenseeManager.Controllers;
using LicenseeManager.Models;
using LicenseeManager.Tests.TestData;
using LicenseeManager.Tests.TestHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace LicenseeManager.Tests.Controllers
{
    public class LicenseesControllerTests
    {
        // Test for Index action
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


        // Test for Details action with invalid ID
        [Fact]
        public async Task Details_ReturnsNotFound_WhenInvalidId()
        {
            var context = InMemoryDbHelper.GetDbContext("LicenseeDetailsTest");
            var controller = new LicenseesController(context);

            var result = await controller.Details(999);

            Assert.IsType<NotFoundResult>(result);
        }


        // Test for Create action with valid licensee
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

        // Test for Create action with invalid licensee (missing required fields)
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

        // Test for Create action with expired licensee
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

        // Test for Edit action with nonexistent licensee
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

        // Data-driven test for Create action
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
