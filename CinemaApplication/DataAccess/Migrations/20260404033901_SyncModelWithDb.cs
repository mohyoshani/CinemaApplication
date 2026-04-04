using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaApplication.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelWithDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. حذف الـ Foreign Key القديم مؤقتاً
            migrationBuilder.DropForeignKey(
                name: "FK_MovieTheaters_CinemaHalls_CinemaHallId",
                table: "MovieTheaters");

            migrationBuilder.Sql("UPDATE MovieTheaters SET CinemaHallId = (SELECT TOP 1 Id FROM CinemaHalls)");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaHallId",
                table: "MovieTheaters",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

           
            migrationBuilder.DropColumn(
                name: "CinemaId",
                table: "MovieTheaters");

            // 5. إعادة ربط الـ Foreign Key
            migrationBuilder.AddForeignKey(
                name: "FK_MovieTheaters_CinemaHalls_CinemaHallId",
                table: "MovieTheaters",
                column: "CinemaHallId",
                principalTable: "CinemaHalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovieTheaters_CinemaHalls_CinemaHallId",
                table: "MovieTheaters");

            migrationBuilder.AlterColumn<int>(
                name: "CinemaHallId",
                table: "MovieTheaters",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_MovieTheaters_CinemaHalls_CinemaHallId",
                table: "MovieTheaters",
                column: "CinemaHallId",
                principalTable: "CinemaHalls",
                principalColumn: "Id");
        }
    }
}
