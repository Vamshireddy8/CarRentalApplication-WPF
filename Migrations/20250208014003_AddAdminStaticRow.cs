using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRentalApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminStaticRow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO Users (FullName, Email, Password, Role)
                VALUES ('Admin', 'admin@carrental.com', 'admin123', 'Admin')
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Users WHERE Email = 'admin@carrental.com'");
        }
    }
}
