using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace RAZOR_PAGE9_ENTITY.Migrations
{
    public partial class SeedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 1; i<=150; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    new[]
                    {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnabled",
                        "AccessFailedCount",
                        "HomeAddress"
                    },
                    new object[]
                    {
                        Guid.NewGuid().ToString(),
                        "User-"+i.ToString("D3"),
                        $"email{i.ToString("D3")}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        false,
                        0,
                        "...@#%..."
                    }
                    );
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
