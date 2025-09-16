using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Migrations
{
    /// <inheritdoc />
    public partial class RenameTableForConsistency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communinity_CommunityId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommunity_Communinity_CommunityId",
                table: "UserCommunity");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommunity_Users_UserId",
                table: "UserCommunity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCommunity",
                table: "UserCommunity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Communinity",
                table: "Communinity");

            migrationBuilder.RenameTable(
                name: "UserCommunity",
                newName: "UserCommunities");

            migrationBuilder.RenameTable(
                name: "Communinity",
                newName: "Communities");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommunity_CommunityId",
                table: "UserCommunities",
                newName: "IX_UserCommunities_CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Communinity_Name",
                table: "Communities",
                newName: "IX_Communities_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCommunities",
                table: "UserCommunities",
                columns: new[] { "UserId", "CommunityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Communities",
                table: "Communities",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                table: "Posts",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommunities_Communities_CommunityId",
                table: "UserCommunities",
                column: "CommunityId",
                principalTable: "Communities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommunities_Users_UserId",
                table: "UserCommunities",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_Communities_CommunityId",
                table: "Posts");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommunities_Communities_CommunityId",
                table: "UserCommunities");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCommunities_Users_UserId",
                table: "UserCommunities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserCommunities",
                table: "UserCommunities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Communities",
                table: "Communities");

            migrationBuilder.RenameTable(
                name: "UserCommunities",
                newName: "UserCommunity");

            migrationBuilder.RenameTable(
                name: "Communities",
                newName: "Communinity");

            migrationBuilder.RenameIndex(
                name: "IX_UserCommunities_CommunityId",
                table: "UserCommunity",
                newName: "IX_UserCommunity_CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Communities_Name",
                table: "Communinity",
                newName: "IX_Communinity_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserCommunity",
                table: "UserCommunity",
                columns: new[] { "UserId", "CommunityId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Communinity",
                table: "Communinity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_Communinity_CommunityId",
                table: "Posts",
                column: "CommunityId",
                principalTable: "Communinity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommunity_Communinity_CommunityId",
                table: "UserCommunity",
                column: "CommunityId",
                principalTable: "Communinity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCommunity_Users_UserId",
                table: "UserCommunity",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
