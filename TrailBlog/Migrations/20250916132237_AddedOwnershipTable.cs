using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Migrations
{
    /// <inheritdoc />
    public partial class AddedOwnershipTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Communities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Communities",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Communities_UserId",
                table: "Communities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Communities_Users_UserId",
                table: "Communities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Communities_Users_UserId",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_UserId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Communities");
        }
    }
}
