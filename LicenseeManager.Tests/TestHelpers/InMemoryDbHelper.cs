using Licensee_Manager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace LicenseeManager.Tests.TestHelpers
{
    public static class InMemoryDbHelper
    {
        public static AppDbContext GetDbContext(string name)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(name)
                .Options;

            return new AppDbContext(options);
        }
    }
}
