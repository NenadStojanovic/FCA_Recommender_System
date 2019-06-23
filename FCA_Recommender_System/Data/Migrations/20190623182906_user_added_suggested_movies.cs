using Microsoft.EntityFrameworkCore.Migrations;

namespace FCA_Recommender_System.Data.Migrations
{
    public partial class user_added_suggested_movies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedMovies_Movies_MovieID",
                table: "LikedMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategories_Movies_CategoryId",
                table: "MovieCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategories_Categories_MovieId",
                table: "MovieCategories");

            migrationBuilder.RenameColumn(
                name: "MovieID",
                table: "LikedMovies",
                newName: "MovieId");

            migrationBuilder.RenameIndex(
                name: "IX_LikedMovies_MovieID",
                table: "LikedMovies",
                newName: "IX_LikedMovies_MovieId");

            migrationBuilder.AlterColumn<int>(
                name: "MovieId",
                table: "LikedMovies",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SuggestedMovies",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_LikedMovies_Movies_MovieId",
                table: "LikedMovies",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategories_Categories_CategoryId",
                table: "MovieCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategories_Movies_MovieId",
                table: "MovieCategories",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedMovies_Movies_MovieId",
                table: "LikedMovies");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategories_Categories_CategoryId",
                table: "MovieCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_MovieCategories_Movies_MovieId",
                table: "MovieCategories");

            migrationBuilder.DropColumn(
                name: "SuggestedMovies",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "MovieId",
                table: "LikedMovies",
                newName: "MovieID");

            migrationBuilder.RenameIndex(
                name: "IX_LikedMovies_MovieId",
                table: "LikedMovies",
                newName: "IX_LikedMovies_MovieID");

            migrationBuilder.AlterColumn<int>(
                name: "MovieID",
                table: "LikedMovies",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_LikedMovies_Movies_MovieID",
                table: "LikedMovies",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategories_Movies_CategoryId",
                table: "MovieCategories",
                column: "CategoryId",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MovieCategories_Categories_MovieId",
                table: "MovieCategories",
                column: "MovieId",
                principalTable: "Categories",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
