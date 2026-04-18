using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaApplication.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class syncwiththeater : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "CinemaId",
            //    table: "MovieTheaters");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CinemaId",
                table: "MovieTheaters",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
