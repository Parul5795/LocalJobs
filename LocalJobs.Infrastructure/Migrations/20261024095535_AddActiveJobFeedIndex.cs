using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalJobs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActiveJobFeedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Jobs_IsActive_CreatedAt",
                table: "Jobs",
                columns: new[] { "IsActive", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Jobs_IsActive_CreatedAt",
                table: "Jobs");
        }
    }
}
