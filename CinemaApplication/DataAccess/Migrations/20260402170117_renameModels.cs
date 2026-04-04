using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaApplication.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class renameModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CinemaApplicationMovies");

            migrationBuilder.DropTable(
                name: "CinemaApplicationHalls");

            migrationBuilder.CreateTable(
                name: "CinemaHalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaHalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MovieTheaters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CinemaId = table.Column<int>(type: "int", nullable: false),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    CinemaHallId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieTheaters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieTheaters_CinemaHalls_CinemaHallId",
                        column: x => x.CinemaHallId,
                        principalTable: "CinemaHalls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MovieTheaters_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovieTheaters_CinemaHallId",
                table: "MovieTheaters",
                column: "CinemaHallId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieTheaters_MovieId",
                table: "MovieTheaters",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovieTheaters");

            migrationBuilder.DropTable(
                name: "CinemaHalls");

            migrationBuilder.CreateTable(
                name: "CinemaApplicationHalls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaApplicationHalls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CinemaApplicationMovies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CinemaApplicationHallId = table.Column<int>(type: "int", nullable: true),
                    MovieId = table.Column<int>(type: "int", nullable: false),
                    CinemaApplicationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CinemaApplicationMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CinemaApplicationMovies_CinemaApplicationHalls_CinemaApplicationHallId",
                        column: x => x.CinemaApplicationHallId,
                        principalTable: "CinemaApplicationHalls",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CinemaApplicationMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CinemaApplicationMovies_CinemaApplicationHallId",
                table: "CinemaApplicationMovies",
                column: "CinemaApplicationHallId");

            migrationBuilder.CreateIndex(
                name: "IX_CinemaApplicationMovies_MovieId",
                table: "CinemaApplicationMovies",
                column: "MovieId");
        }
    }
}
