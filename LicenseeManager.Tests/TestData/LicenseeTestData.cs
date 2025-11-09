using System;
using System.Collections.Generic;
using LicenseeManager.Models;

namespace LicenseeManager.Tests.TestData
{
    public static class LicenseeTestData
    {
        public static IEnumerable<object[]> CreateLicenseeData =>
            new List<object[]>
            {
                //Valid licensee
                new object[]
                {
                    new Licensee
                    {
                        FirstName = "Alice",
                        LastName = "Anderson",
                        Email = "alice@example.com",
                        LicenseNumber = "A12345",
                        IssueDate = DateTime.Today.AddYears(-1),
                        ExpirationDate = DateTime.Today.AddYears(1),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    true // expected success
                },

                //Expired licensee
                new object[]
                {
                    new Licensee
                    {
                        FirstName = "Bob",
                        LastName = "Expired",
                        Email = "bob@example.com",
                        LicenseNumber = "B54321",
                        IssueDate = DateTime.Today.AddYears(-2),
                        ExpirationDate = DateTime.Today.AddDays(-3),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    false // expected failure
                },

                //Missing email
                new object[]
                {
                    new Licensee
                    {
                        FirstName = "Charlie",
                        LastName = "NoEmail",
                        LicenseNumber = "C33333",
                        IssueDate = DateTime.Today.AddYears(-1),
                        ExpirationDate = DateTime.Today.AddYears(1),
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    },
                    false // expected failure
                }
            };
    }
}
