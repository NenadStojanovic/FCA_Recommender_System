﻿using LumenWorks.Framework.IO.Csv;
using RecommenderSystem.Models;
using SPARQLNET;
using SPARQLNET.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderSystem.Util
{
    public class InputFileManager
    {
        public string Separator { get; set; }
        QueryClient queryClient = new QueryClient("http://dbpedia.org/sparql");

        public List<Movie> ParseFile(string filePath)
        {
            List<Movie> movies = new List<Movie>();
            using (CsvReader csv = new CsvReader(new StreamReader(filePath), false))
            {
                int fieldCount = csv.FieldCount;
                while (csv.ReadNextRecord())
                {
                    movies.Add(new Movie(Int32.Parse(csv[0]), csv[1], csv[2]));
                }
            }

            return movies;
        }


        public void GetMoveisCategories(List<Movie> movies)
        {
            foreach (var item in movies)
            {
                this.GetMovieCategorie(item);
            }
        }

        public void GetMovieCategorie(Movie movie)
        {
            var query = "SELECT * WHERE {<" + movie.DataLink + "> <http://purl.org/dc/terms/subject> ?categories}";
            Table table = queryClient.Query(query);
            movie.Categories = new HashSet<string>(table.RowsToStringList());
        }

    }
}
