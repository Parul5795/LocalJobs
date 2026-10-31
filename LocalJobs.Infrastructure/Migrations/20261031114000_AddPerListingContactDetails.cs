using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LocalJobs.Infrastructure.Migrations;

public partial class AddPerListingContactDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>("ContactPersonName", "Jobs", type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("ContactMobileNumber", "Jobs", type: "character varying(13)", maxLength: 13, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<bool>("AllowPhoneCall", "Jobs", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>("AllowWhatsApp", "Jobs", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<string>("ContactPersonName", "Ads", type: "character varying(100)", maxLength: 100, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<string>("ContactMobileNumber", "Ads", type: "character varying(13)", maxLength: 13, nullable: false, defaultValue: "");
        migrationBuilder.AddColumn<bool>("AllowPhoneCall", "Ads", type: "boolean", nullable: false, defaultValue: false);
        migrationBuilder.AddColumn<bool>("AllowWhatsApp", "Ads", type: "boolean", nullable: false, defaultValue: false);

        migrationBuilder.AlterColumn<string>("ContactPersonName", "Jobs", type: "character varying(100)", maxLength: 100, nullable: false, oldClrType: typeof(string), oldType: "character varying(100)", oldMaxLength: 100, oldDefaultValue: "");
        migrationBuilder.AlterColumn<string>("ContactMobileNumber", "Jobs", type: "character varying(13)", maxLength: 13, nullable: false, oldClrType: typeof(string), oldType: "character varying(13)", oldMaxLength: 13, oldDefaultValue: "");
        migrationBuilder.AlterColumn<bool>("AllowPhoneCall", "Jobs", type: "boolean", nullable: false, oldClrType: typeof(bool), oldType: "boolean", oldDefaultValue: false);
        migrationBuilder.AlterColumn<bool>("AllowWhatsApp", "Jobs", type: "boolean", nullable: false, oldClrType: typeof(bool), oldType: "boolean", oldDefaultValue: false);
        migrationBuilder.AlterColumn<string>("ContactPersonName", "Ads", type: "character varying(100)", maxLength: 100, nullable: false, oldClrType: typeof(string), oldType: "character varying(100)", oldMaxLength: 100, oldDefaultValue: "");
        migrationBuilder.AlterColumn<string>("ContactMobileNumber", "Ads", type: "character varying(13)", maxLength: 13, nullable: false, oldClrType: typeof(string), oldType: "character varying(13)", oldMaxLength: 13, oldDefaultValue: "");
        migrationBuilder.AlterColumn<bool>("AllowPhoneCall", "Ads", type: "boolean", nullable: false, oldClrType: typeof(bool), oldType: "boolean", oldDefaultValue: false);
        migrationBuilder.AlterColumn<bool>("AllowWhatsApp", "Ads", type: "boolean", nullable: false, oldClrType: typeof(bool), oldType: "boolean", oldDefaultValue: false);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn("ContactPersonName", "Jobs");
        migrationBuilder.DropColumn("ContactMobileNumber", "Jobs");
        migrationBuilder.DropColumn("AllowPhoneCall", "Jobs");
        migrationBuilder.DropColumn("AllowWhatsApp", "Jobs");
        migrationBuilder.DropColumn("ContactPersonName", "Ads");
        migrationBuilder.DropColumn("ContactMobileNumber", "Ads");
        migrationBuilder.DropColumn("AllowPhoneCall", "Ads");
        migrationBuilder.DropColumn("AllowWhatsApp", "Ads");
    }
}
