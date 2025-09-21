using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRevokedFieldToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRevoked",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRevoked",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "Users");
        }
    }
}
