using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VisitCountApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyVisits",
                columns: table => new
                {
                    PersianDateID = table.Column<int>(type: "int", nullable: false),
                    TotalVisits = table.Column<int>(type: "int", nullable: false),
                    InsertDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyVisits", x => x.PersianDateID);
                });

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    VisitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstertDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpiryDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VisitCount = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.VisitId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyVisits");

            migrationBuilder.DropTable(
                name: "Visitors");
        }
    }
}
