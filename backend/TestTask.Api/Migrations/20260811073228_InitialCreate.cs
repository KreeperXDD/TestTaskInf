using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestTask.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    FirstOperationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TimeDeltaSeconds = table.Column<double>(type: "double precision", nullable: false),
                    AvarageExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    AvarageMetricValue = table.Column<double>(type: "double precision", nullable: false),
                    MedianMetricValue = table.Column<double>(type: "double precision", nullable: false),
                    MaxMetricValue = table.Column<double>(type: "double precision", nullable: false),
                    MinMetricVAlue = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Values",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExecutionTime = table.Column<double>(type: "double precision", nullable: false),
                    MetricValue = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Values", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_FileName",
                table: "Results",
                column: "FileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Values_FileName_Date",
                table: "Values",
                columns: new[] { "FileName", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "Values");
        }
    }
}
