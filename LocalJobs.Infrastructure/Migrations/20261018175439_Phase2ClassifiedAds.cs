using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LocalJobs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase2ClassifiedAds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkerProfiles_CategoryId",
                table: "WorkerProfiles");

            migrationBuilder.CreateTable(
                name: "AdCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "character varying(3000)", maxLength: 3000, nullable: false),
                    AdCategoryId = table.Column<int>(type: "integer", nullable: false),
                    CityId = table.Column<int>(type: "integer", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ads_AdCategories_AdCategoryId",
                        column: x => x.AdCategoryId,
                        principalTable: "AdCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ads_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AdCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Jobs/Work" },
                    { 2, "Services" },
                    { 3, "Business/Shop" },
                    { 4, "Products for Sale" },
                    { 5, "Tuition" },
                    { 6, "House/Room" },
                    { 7, "Vehicle" },
                    { 8, "Local Announcement" },
                    { 9, "Other" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfiles_CategoryId_CityId_IsFresher",
                table: "WorkerProfiles",
                columns: new[] { "CategoryId", "CityId", "IsFresher" });

            migrationBuilder.CreateIndex(
                name: "IX_AdCategories_Name",
                table: "AdCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ads_AdCategoryId_CityId_IsActive",
                table: "Ads",
                columns: new[] { "AdCategoryId", "CityId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_CityId",
                table: "Ads",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_IsActive_CreatedAt",
                table: "Ads",
                columns: new[] { "IsActive", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_UserId",
                table: "Ads",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ads");

            migrationBuilder.DropTable(
                name: "AdCategories");

            migrationBuilder.DropIndex(
                name: "IX_WorkerProfiles_CategoryId_CityId_IsFresher",
                table: "WorkerProfiles");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfiles_CategoryId",
                table: "WorkerProfiles",
                column: "CategoryId");
        }
    }
}
