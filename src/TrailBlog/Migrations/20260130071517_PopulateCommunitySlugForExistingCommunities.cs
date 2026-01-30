using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrailBlog.Api.Migrations
{
    /// <inheritdoc />
    public partial class PopulateCommunitySlugForExistingCommunities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Communities""
                SET ""CommunitySlug"" = LOWER(REPLACE(""Name"", ' ', '-'))
                WHERE ""CommunitySlug"" IS NULL OR ""CommunitySlug"" = ''
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Communities""
                SET ""CommunitySlug"" = NULL
                WHERE ""CommunitySlug"" != ''
            ");
        }
    }
}
