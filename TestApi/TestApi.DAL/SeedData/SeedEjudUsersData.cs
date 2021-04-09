using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Migrations;
using TestApi.DAL.Helpers;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.SeedData
{
    public class SeedEjudUsersData
    {
        private const int MAX_AUTOMATION_USERS = 25;
        private const int MAX_MANUAL_USERS = 10;
        private const string AUTOMATION_FIRSTNAME = "Auto";
        private const string MANUAL_FIRSTNAME = "Manual";
        private const string DOMAIN = "@judiciarystaging.onmicrosoft.com";
        private readonly object[,] _userAutomationRowData;
        private readonly object[,] _userManualRowData;
        private readonly object[] _allocationRowData;
        private readonly List<User> _automationUsers;
        private readonly List<User> _manualUsers;

        private static string[] _userColumns =
        {
            "Id", "Username", "ContactEmail", "FirstName", "LastName", "DisplayName", "Number", "TestType", "UserType",
            "Application", "IsProdUser", "CreatedDate"
        };

        public SeedEjudUsersData()
        {
            _automationUsers = CreateUsers(MAX_AUTOMATION_USERS, AUTOMATION_FIRSTNAME, TestType.Automated);
            _manualUsers = CreateUsers(MAX_MANUAL_USERS, MANUAL_FIRSTNAME, TestType.Manual);

            _userAutomationRowData = CreateUserRowData(_automationUsers);
            _userManualRowData = CreateUserRowData(_manualUsers);

            var allocationAutomationRowData = CreateAllocationRowData(_automationUsers);
            var allocationManualRowData = CreateAllocationRowData(_manualUsers);
            _allocationRowData = allocationAutomationRowData.Concat(allocationManualRowData).ToArray();
        }

        private static List<User> CreateUsers(int amountOfUsers, string firstName, TestType testType)
        {
            var users = new List<User>();

            for (var i = 0; i < amountOfUsers; i++)
            {
                var number = i + 1;
                var lastName = $"Judge {number}";
                var displayName = $"{firstName} {lastName}";
                var username = $"{firstName}_{TextHelpers.ReplaceSpacesWithUnderscores(lastName)}{DOMAIN}";
                var contactEmail = $"{firstName}_{TextHelpers.ReplaceSpacesWithUnderscores(lastName)}{DOMAIN}";

                users.Add(new User()
                {
                    Application = Application.Any,
                    ContactEmail = contactEmail,
                    CreatedDate = DateTime.UtcNow,
                    DisplayName = displayName,
                    FirstName = firstName,
                    IsProdUser = false,
                    LastName = lastName,
                    Number = number,
                    TestType = testType,
                    Username = username,
                    UserType = UserType.Judge
                });
            }

            return users;
        }

        private static object[,] CreateUserRowData(IReadOnlyList<User> users)
        {
            var rows = new object[users.Count, _userColumns.Length];
            
            for (var i = 0; i < users.Count; i++)
            {
                rows[i, 0] = users[i].Id;
                rows[i, 1] = users[i].Username;
                rows[i, 2] = users[i].ContactEmail;
                rows[i, 3] = users[i].FirstName;
                rows[i, 4] = users[i].LastName;
                rows[i, 5] = users[i].DisplayName;
                rows[i, 6] = users[i].Number;
                rows[i, 7] = (int) users[i].TestType;
                rows[i, 8] = (int) users[i].UserType;
                rows[i, 9] = (int) users[i].Application;
                rows[i, 10] = users[i].IsProdUser;
                rows[i, 11] = users[i].CreatedDate;

                //{ users[i].Id, users[i].Username, users[i].ContactEmail, users[i].FirstName, users[i].LastName, users[i].DisplayName, users[i].Number, (int) users[i].TestType, (int) users[i].UserType, (int) users[i].Application, users[i].IsProdUser, users[i].CreatedDate };
            }

            return rows;
        }

        private static IEnumerable<object> CreateAllocationRowData(IReadOnlyList<User> users)
        {
            var allocations = new object[users.Count];

            for (var i = 0; i < users.Count; i++)
            {
                allocations[i] = new object[] { Guid.NewGuid(), users[i].Id, users[i].Username, null, false, null };
            }

            return allocations;
        }

        public void Up(MigrationBuilder migrationBuilder)
        {
            AddUsers(migrationBuilder);
            AddAllocations(migrationBuilder);
        }

        private void AddUsers(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(User),
                columns: new[] { "Id", "Username", "ContactEmail", "FirstName", "LastName", "DisplayName", "Number", "TestType", "UserType", "Application", "IsProdUser", "CreatedDate" },
                values: _userAutomationRowData
            );

            migrationBuilder.InsertData(
                table: nameof(User),
                columns: new[] { "Id", "Username", "ContactEmail", "FirstName", "LastName", "DisplayName", "Number", "TestType", "UserType", "Application", "IsProdUser", "CreatedDate" },
                values: _userManualRowData
            );
        }

        public void AddAllocations(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(Allocation),
                columns: new[] { "Id", "UserId", "Username", "ExpiresAt", "Allocated", "AllocatedBy" },
                values: _allocationRowData
            );
        }

        public void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"delete from {nameof(User)}");
            migrationBuilder.Sql($"delete from {nameof(Allocation)}");
            migrationBuilder.Sql($"dbcc checkident('{nameof(User)}',reseed,0)");
            migrationBuilder.Sql($"dbcc checkident('{nameof(Allocation)}',reseed,0)");
        }
    }
}
