using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRecentViewedPostsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecentViewedPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentViewedPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecentViewedPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecentViewedPosts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewedPosts_PostId",
                table: "RecentViewedPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewedPosts_PostId_ViewedAt",
                table: "RecentViewedPosts",
                columns: new[] { "PostId", "ViewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewedPosts_UserId",
                table: "RecentViewedPosts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewedPosts_UserId_PostId",
                table: "RecentViewedPosts",
                columns: new[] { "UserId", "PostId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewedPosts_UserId_ViewedAt",
                table: "RecentViewedPosts",
                columns: new[] { "UserId", "ViewedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_RecentViewedPosts_ViewedAt",
                table: "RecentViewedPosts",
                column: "ViewedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecentViewedPosts");
        }
    }
}
