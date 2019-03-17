using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace FCA_Recommender_System.Data.Migrations
{
    public partial class LikedMoviesUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movies_LikedMovies_LikedMoviesID",
                table: "Movies");

            migrationBuilder.DropIndex(
                name: "IX_Movies_LikedMoviesID",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "LikedMoviesID",
                table: "Movies");

            migrationBuilder.AddColumn<int>(
                name: "MovieID",
                table: "LikedMovies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LikedMovies_MovieID",
                table: "LikedMovies",
                column: "MovieID");

            migrationBuilder.AddForeignKey(
                name: "FK_LikedMovies_Movies_MovieID",
                table: "LikedMovies",
                column: "MovieID",
                principalTable: "Movies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LikedMovies_Movies_MovieID",
                table: "LikedMovies");

            migrationBuilder.DropIndex(
                name: "IX_LikedMovies_MovieID",
                table: "LikedMovies");

            migrationBuilder.DropColumn(
                name: "MovieID",
                table: "LikedMovies");

            migrationBuilder.AddColumn<int>(
                name: "LikedMoviesID",
                table: "Movies",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_LikedMoviesID",
                table: "Movies",
                column: "LikedMoviesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Movies_LikedMovies_LikedMoviesID",
                table: "Movies",
                column: "LikedMoviesID",
                principalTable: "LikedMovies",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
