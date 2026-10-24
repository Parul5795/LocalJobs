using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalJobs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CompleteLocalJobsMvp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_WorkerProfiles_CategoryId_CityId_IsFresher",
                table: "WorkerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CityId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Ads_AdCategoryId_CityId_IsActive",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_CityId",
                table: "Ads");

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "WorkerProfiles",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactCount",
                table: "WorkerProfiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "WorkerProfiles",
                type: "character varying(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Jobs",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Area",
                table: "Ads",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string[]>(
                name: "ImageUrls",
                table: "Ads",
                type: "text[]",
                nullable: false,
                defaultValueSql: "ARRAY[]::text[]");

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmployerUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Method = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_Users_EmployerUserId",
                        column: x => x.EmployerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_Users_WorkerUserId",
                        column: x => x.WorkerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfiles_CategoryId_CityId_Area_IsFresher",
                table: "WorkerProfiles",
                columns: new[] { "CategoryId", "CityId", "Area", "IsFresher" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CityId_Area_IsActive",
                table: "Jobs",
                columns: new[] { "CityId", "Area", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CityId_CategoryId_IsActive_CreatedAt",
                table: "Jobs",
                columns: new[] { "CityId", "CategoryId", "IsActive", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_AdCategoryId",
                table: "Ads",
                column: "AdCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_CityId_AdCategoryId_IsActive_CreatedAt",
                table: "Ads",
                columns: new[] { "CityId", "AdCategoryId", "IsActive", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_CityId_Area_IsActive",
                table: "Ads",
                columns: new[] { "CityId", "Area", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_EmployerUserId_CreatedAt",
                table: "Contacts",
                columns: new[] { "EmployerUserId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_WorkerUserId_CreatedAt",
                table: "Contacts",
                columns: new[] { "WorkerUserId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropIndex(
                name: "IX_WorkerProfiles_CategoryId_CityId_Area_IsFresher",
                table: "WorkerProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CityId_Area_IsActive",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CityId_CategoryId_IsActive_CreatedAt",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Ads_AdCategoryId",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_CityId_AdCategoryId_IsActive_CreatedAt",
                table: "Ads");

            migrationBuilder.DropIndex(
                name: "IX_Ads_CityId_Area_IsActive",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "ContactCount",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "WorkerProfiles");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Area",
                table: "Ads");

            migrationBuilder.DropColumn(
                name: "ImageUrls",
                table: "Ads");

            migrationBuilder.CreateIndex(
                name: "IX_WorkerProfiles_CategoryId_CityId_IsFresher",
                table: "WorkerProfiles",
                columns: new[] { "CategoryId", "CityId", "IsFresher" });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CityId",
                table: "Jobs",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Ads_AdCategoryId_CityId_IsActive",
                table: "Ads",
                columns: new[] { "AdCategoryId", "CityId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Ads_CityId",
                table: "Ads",
                column: "CityId");
        }
    }
}
