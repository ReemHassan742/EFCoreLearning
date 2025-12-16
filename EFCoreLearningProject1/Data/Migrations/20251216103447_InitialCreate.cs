using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EFCoreLearningProject1.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Biography = table.Column<string>(type: "TEXT", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Country = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    ISBN = table.Column<string>(type: "TEXT", nullable: false),
                    PublicationYear = table.Column<int>(type: "INTEGER", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    IsAvailable = table.Column<bool>(type: "INTEGER", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthDate", "Country", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "English novelist and essayist.", new DateTime(1903, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "United Kingdom", "George", "Orwell" },
                    { 2, "American science fiction writer and professor of biochemistry.", new DateTime(1920, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "United States", "Isaac", "Asimov" },
                    { 3, "English writer known for her detective novels.", new DateTime(1890, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "United Kingdom", "Agatha", "Christie" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Fictional Literature", "Fiction" },
                    { 2, "Sci-Fi books", "Science Fiction" },
                    { 3, "Mystery and Thriller", "Mystery" },
                    { 4, "Biographical works", "Biography" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AddedDate", "AuthorId", "GenreId", "ISBN", "IsAvailable", "Price", "PublicationYear", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 12, 16, 10, 34, 39, 830, DateTimeKind.Utc).AddTicks(6461), 1, 1, "978-0451524935", true, 9.99m, 1949, "1984" },
                    { 2, new DateTime(2025, 12, 16, 10, 34, 39, 833, DateTimeKind.Utc).AddTicks(765), 1, 1, "978-0451526342", true, 7.99m, 1945, "Animal Farm" },
                    { 3, new DateTime(2025, 12, 16, 10, 34, 39, 833, DateTimeKind.Utc).AddTicks(783), 2, 2, "978-0553293357", true, 8.99m, 1951, "Foundation" },
                    { 4, new DateTime(2025, 12, 16, 10, 34, 39, 833, DateTimeKind.Utc).AddTicks(786), 2, 2, "978-0553382563", true, 6.99m, 1950, "I, Robot" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_FirstName_LastName",
                table: "Authors",
                columns: new[] { "FirstName", "LastName" });

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_GenreId",
                table: "Books",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_ISBN",
                table: "Books",
                column: "ISBN",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
