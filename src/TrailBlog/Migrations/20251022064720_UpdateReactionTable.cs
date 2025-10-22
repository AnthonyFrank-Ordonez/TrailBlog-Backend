using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReactionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId_PostId",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "IsDislike",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "IsLike",
                table: "Reactions");

            migrationBuilder.AddColumn<int>(
                name: "ReactionId",
                table: "Reactions",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_PostId_ReactionId",
                table: "Reactions",
                columns: new[] { "UserId", "PostId", "ReactionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reactions_UserId_PostId_ReactionId",
                table: "Reactions");

            migrationBuilder.DropColumn(
                name: "ReactionId",
                table: "Reactions");

            migrationBuilder.AddColumn<bool>(
                name: "IsDislike",
                table: "Reactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsLike",
                table: "Reactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Reactions_UserId_PostId",
                table: "Reactions",
                columns: new[] { "UserId", "PostId" },
                unique: true);
        }
    }
}
