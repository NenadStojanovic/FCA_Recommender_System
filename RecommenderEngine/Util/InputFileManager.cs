using LumenWorks.Framework.IO.Csv;
using RecommenderEngine.Models;
using SPARQLNET;
using SPARQLNET.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecommenderEngine.Util
{
    public class InputFileManager
    {
        public string Separator { get; set; }
        QueryClient queryClient = new QueryClient("http://dbpedia.org/sparql");

        public List<Movie> ParseFile(string filePath)
        {
            List<Movie> movies = new List<Movie>();
            using (CsvReader csv = new CsvReader(new StreamReader(filePath), false,'\t'))
            {
                int fieldCount = csv.FieldCount;
                while (csv.ReadNextRecord())
                {
                    movies.Add(new Movie(Int32.Parse(csv[0]), csv[1], csv[2]));
                }
            }

            return movies;
        }


        public void GetMoveisData(List<Movie> movies)
        {
            foreach (var item in movies)
            {
                this.GetMovieCategorie(item);
                this.GetMovieDirector(item);
                this.GetMovieAbstract(item);
            }
        }

        public void GetMovieCategorie(Movie movie)
        {
            var query = "SELECT * WHERE {<" + movie.DataLink + "> <http://purl.org/dc/terms/subject> ?categories}";
            Table table = queryClient.Query(query);
            movie.Categories = new HashSet<string>(table.RowsToStringList());
        }

        public void GetMovieDirector(Movie movie)
        {
            var query = "SELECT * WHERE {<" + movie.DataLink + "> <http://dbpedia.org/ontology/director>   ?director}";
            Table table = queryClient.Query(query);
            if(table.Rows.Count>0)
            {
                movie.Director = table.Rows[0].FirstOrDefault();
                movie.Director = movie.Director.Split("/resource/")[1].Replace('_', ' ');
            }

        }


        public void GetMovieAbstract(Movie movie)
        {
            var query = "SELECT * WHERE {<" + movie.DataLink + "> <http://dbpedia.org/ontology/abstract> ?abstract FILTER langMatches(lang(?abstract),'en')}";
            Table table = queryClient.Query(query);
            if(table.Rows.Count>0)
            {
                movie.Abstract = table.Rows[0].FirstOrDefault();
            }

        }

    }
}
