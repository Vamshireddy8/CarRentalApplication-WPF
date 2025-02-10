using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalApplication.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserFullNameAndVehicleName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
               name: "UserFullName",
               table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VehicleName",
                table: "Bookings");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserFullName",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleName",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
