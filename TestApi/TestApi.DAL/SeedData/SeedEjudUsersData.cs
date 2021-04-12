using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using TestApi.Common.Data;
using TestApi.Domain;
using TestApi.Domain.Enums;

namespace TestApi.DAL.SeedData
{
    public class SeedEjudUsersData
    {
        private const int MAX_AUTOMATION_USERS = 25;
        private const int MAX_MANUAL_USERS = 10;
        private readonly object[,] _userAutomationRowData;
        private readonly object[,] _userManualRowData;
        private readonly object[,] _allocationAutomationData;
        private readonly object[,] _allocationManualData;

        private static readonly string[] _userColumns =
        {
            "Id", "Username", "ContactEmail", "FirstName", "LastName", "DisplayName", "Number", "TestType", "UserType",
            "Application", "IsProdUser", "CreatedDate"
        };

        private static readonly string[] _allocationColumns = {"Id", "UserId", "Username", "ExpiresAt", "Allocated", "AllocatedBy"};

        private static string DOMAIN => new ConfigurationBuilder()
            .AddUserSecrets("04df59fe-66aa-4fb2-8ac5-b87656f7675a")
            .Build()
            .GetValue<string>("EjudUsernameStem");

        public SeedEjudUsersData()
        {
            var automationUsers = CreateUsers(MAX_AUTOMATION_USERS, EjudUserData.AUTOMATED_FIRST_NAME_PREFIX, TestType.Automated);
            var manualUsers = CreateUsers(MAX_MANUAL_USERS, EjudUserData.MANUAL_FIRST_NAME_PREFIX, TestType.Manual);

            _userAutomationRowData = CreateUserRowData(automationUsers);
            _userManualRowData = CreateUserRowData(manualUsers);

            _allocationAutomationData = CreateAllocationRowData(automationUsers);
            _allocationManualData = CreateAllocationRowData(manualUsers);
        }

        private static List<User> CreateUsers(int amountOfUsers, string firstName, TestType testType)
        {
            var users = new List<User>();

            for (var i = 0; i < amountOfUsers; i++)
            {
                var number = i + 1;
                var lastName = EjudUserData.LAST_NAME(number);
                var displayName = EjudUserData.DISPLAY_NAME(firstName, lastName);
                var username = EjudUserData.USERNAME(firstName, lastName, DOMAIN);
                var contactEmail = EjudUserData.CONTACT_EMAIL(firstName, lastName, DOMAIN);

                users.Add(new User
                {
                    Application = Application.Ejud,
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
            }

            return rows;
        }

        private static object[,] CreateAllocationRowData(IReadOnlyList<User> users)
        {
            var rows = new object[users.Count, _allocationColumns.Length];

            for (var i = 0; i < users.Count; i++)
            {
                rows[i, 0] = Guid.NewGuid();
                rows[i, 1] = users[i].Id;
                rows[i, 2] = users[i].Username;
                rows[i, 3] = null;
                rows[i, 4] = false;
                rows[i, 5] = null;
            }

            return rows;
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
                columns: _userColumns,
                values: _userAutomationRowData
            );

            migrationBuilder.InsertData(
                table: nameof(User),
                columns: _userColumns,
                values: _userManualRowData
            );
        }

        public void AddAllocations(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: nameof(Allocation),
                columns: _allocationColumns,
                values: _allocationAutomationData
            );

            migrationBuilder.InsertData(
                table: nameof(Allocation),
                columns: _allocationColumns,
                values: _allocationManualData
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
