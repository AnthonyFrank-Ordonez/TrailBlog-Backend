using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOwnerIdtoUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_OwnerId",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_OwnerId_CreatedAt",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Communities");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_UserId_CreatedAt",
                table: "Communities",
                columns: new[] { "UserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_UserId_CreatedAt",
                table: "Communities");

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Communities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId",
                table: "Communities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId_CreatedAt",
                table: "Communities",
                columns: new[] { "OwnerId", "CreatedAt" });
        }
    }
}
