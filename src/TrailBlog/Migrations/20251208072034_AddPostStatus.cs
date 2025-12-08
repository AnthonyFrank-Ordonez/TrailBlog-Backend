using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddPostStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Posts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Status",
                table: "Posts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Status_CreatedAt",
                table: "Posts",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId_Status",
                table: "Posts",
                columns: new[] { "UserId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Posts_Status",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_Status_CreatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserId_Status",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Posts");
        }
    }
}
