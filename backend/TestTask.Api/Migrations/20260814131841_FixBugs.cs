using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestTask.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixBugs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinMetricVAlue",
                table: "Results",
                newName: "MinMetricValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MinMetricValue",
                table: "Results",
                newName: "MinMetricVAlue");
        }
    }
}
