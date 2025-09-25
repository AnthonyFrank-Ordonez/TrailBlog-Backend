using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddCommentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CommentedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastUpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PostId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedAt",
                table: "Users",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsRevoked",
                table: "Users",
                column: "IsRevoked");

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsRevoked_CreatedAt",
                table: "Users",
                columns: new[] { "IsRevoked", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsRevoked_UpdatedAt",
                table: "Users",
                columns: new[] { "IsRevoked", "UpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_RefreshToken",
                table: "Users",
                column: "RefreshToken");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedAt",
                table: "Users",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_AssignedAt",
                table: "UserRoles",
                column: "AssignedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId_AssignedAt",
                table: "UserRoles",
                columns: new[] { "RoleId", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId_AssignedAt",
                table: "UserRoles",
                columns: new[] { "UserId", "AssignedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_UserCommunities_JoinedDate",
                table: "UserCommunities",
                column: "JoinedDate");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommunities_UserId",
                table: "UserCommunities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CommunityId_CreatedAt",
                table: "Posts",
                columns: new[] { "CommunityId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedAt",
                table: "Posts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UpdatedAt",
                table: "Posts",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_UserId_CreatedAt",
                table: "Posts",
                columns: new[] { "UserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Communities_CreatedAt",
                table: "Communities",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId",
                table: "Communities",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Communities_OwnerId_CreatedAt",
                table: "Communities",
                columns: new[] { "OwnerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_CommentedAt",
                table: "Comments",
                column: "CommentedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_IsDeleted",
                table: "Comments",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LastUpdatedAt",
                table: "Comments",
                column: "LastUpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId",
                table: "Comments",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_CommentedAt",
                table: "Comments",
                columns: new[] { "PostId", "CommentedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_IsDeleted",
                table: "Comments",
                columns: new[] { "PostId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PostId_LastUpdatedAt",
                table: "Comments",
                columns: new[] { "PostId", "LastUpdatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId_CommentedAt",
                table: "Comments",
                columns: new[] { "UserId", "CommentedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId_LastUpdatedAt",
                table: "Comments",
                columns: new[] { "UserId", "LastUpdatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsRevoked",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsRevoked_CreatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_IsRevoked_UpdatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_RefreshToken",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UpdatedAt",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_AssignedAt",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_RoleId_AssignedAt",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserRoles_UserId_AssignedAt",
                table: "UserRoles");

            migrationBuilder.DropIndex(
                name: "IX_UserCommunities_JoinedDate",
                table: "UserCommunities");

            migrationBuilder.DropIndex(
                name: "IX_UserCommunities_UserId",
                table: "UserCommunities");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CommunityId_CreatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UpdatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_UserId_CreatedAt",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Communities_CreatedAt",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_OwnerId",
                table: "Communities");

            migrationBuilder.DropIndex(
                name: "IX_Communities_OwnerId_CreatedAt",
                table: "Communities");
        }
    }
}
