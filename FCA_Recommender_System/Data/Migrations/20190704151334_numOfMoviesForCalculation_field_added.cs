using Microsoft.EntityFrameworkCore.Migrations;

namespace FCA_Recommender_System.Data.Migrations
{
    public partial class numOfMoviesForCalculation_field_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumOfMoviesForCalculation",
                table: "ConfigurationAndStatistics",
                nullable: false,
                defaultValue: 500);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumOfMoviesForCalculation",
                table: "ConfigurationAndStatistics");
        }
    }
}
