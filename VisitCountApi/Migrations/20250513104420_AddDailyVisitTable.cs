using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitCountApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDailyVisitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiryDateTime",
                table: "Visitors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DailyVisits",
                columns: table => new
                {
                    PersianDateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalVisits = table.Column<int>(type: "int", nullable: false),
                    InsertDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyVisits", x => x.PersianDateID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyVisits");

            migrationBuilder.DropColumn(
                name: "ExpiryDateTime",
                table: "Visitors");
        }
    }
}
