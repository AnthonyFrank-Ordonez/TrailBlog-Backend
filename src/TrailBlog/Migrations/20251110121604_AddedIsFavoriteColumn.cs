using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddedIsFavoriteColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFavorite",
                table: "UserCommunities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_UserCommunities_CommunityId_IsFavorite",
                table: "UserCommunities",
                columns: new[] { "CommunityId", "IsFavorite" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCommunities_IsFavorite",
                table: "UserCommunities",
                column: "IsFavorite");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommunities_UserId_IsFavorite",
                table: "UserCommunities",
                columns: new[] { "UserId", "IsFavorite" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserCommunities_CommunityId_IsFavorite",
                table: "UserCommunities");

            migrationBuilder.DropIndex(
                name: "IX_UserCommunities_IsFavorite",
                table: "UserCommunities");

            migrationBuilder.DropIndex(
                name: "IX_UserCommunities_UserId_IsFavorite",
                table: "UserCommunities");

            migrationBuilder.DropColumn(
                name: "IsFavorite",
                table: "UserCommunities");
        }
    }
}
