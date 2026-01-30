using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedCommunitySlug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommunitySlug",
                table: "Communities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CommunitySlug",
                table: "Communities",
                column: "CommunitySlug");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Communities_CommunitySlug",
                table: "Communities");

            migrationBuilder.DropColumn(
                name: "CommunitySlug",
                table: "Communities");
        }
    }
}
