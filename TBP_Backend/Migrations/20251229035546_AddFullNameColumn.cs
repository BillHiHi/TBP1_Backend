using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TBP_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddFullNameColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "AspNetUsers",
                type: "nvarchar(100)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("FirstName", "AspNetUsers");
            migrationBuilder.DropColumn("LastName", "AspNetUsers");
        }

    }
}
