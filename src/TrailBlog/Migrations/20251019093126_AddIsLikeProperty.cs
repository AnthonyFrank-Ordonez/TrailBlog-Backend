using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddIsLikeProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLike",
                table: "Likes",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLike",
                table: "Likes");
        }
    }
}
