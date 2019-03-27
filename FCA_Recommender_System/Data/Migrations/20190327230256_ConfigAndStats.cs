using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FCA_Recommender_System.Data.Migrations
{
    public partial class ConfigAndStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigurationAndStatistics",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AttributesCount = table.Column<int>(nullable: false),
                    ConceptsCount = table.Column<int>(nullable: false),
                    LatticeCalculationTime = table.Column<DateTime>(nullable: false),
                    LatticeHeight = table.Column<int>(nullable: false),
                    Neo4jConnectionString = table.Column<string>(nullable: true),
                    Neo4jPass = table.Column<string>(nullable: true),
                    Neo4jUsername = table.Column<string>(nullable: true),
                    ObjectsCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigurationAndStatistics", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigurationAndStatistics");
        }
    }
}
