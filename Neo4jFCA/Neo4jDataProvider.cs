using FCAA.Data.Lattice;
using Neo4jClient;
using Neo4jClient.Cypher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Neo4jFCA
{
    public class Neo4jDataProvider
    {
        private GraphClient client;
        private Uri db_adres;
        private string user_name;
        private string password;

        public Neo4jDataProvider(string connectionString, string username, string password)
        {
            this.db_adres = new Uri(connectionString);
            this.user_name = username;
            this.password = password;
            client = new GraphClient(this.db_adres, this.user_name, this.password);
            client = new GraphClient(db_adres,
                new HttpClientWrapper(user_name, password,
                new HttpClient() { Timeout = TimeSpan.FromMinutes(20) })
                );
            try
            {
                client.Connect();
            }
            catch (Exception e)
            {
                throw new Exception("Database failed to connect with following message : " + e.Message);
            }
        }

        public void ClearDatabase()
        {
            //var rawClient = (IRawGraphClient)client;
            client.Cypher
                .Match("()-[r]-()")
                .Delete("r")
                .ExecuteWithoutResults();
            //var query = new CypherQuery("MATCH ()-[r]-() DELETE r", null, CypherResultMode.Set);
            //rawClient.ExecuteCypher(query);

            //var query2 = new CypherQuery("MATCH (n) DELETE n", null, CypherResultMode.Set);
            //rawClient.ExecuteCypher(query2);
            client.Cypher
                .Match("(n)")
                .Delete("n")
                .ExecuteWithoutResults();
        }

        public void ImportFCALattice(ConceptLattice lattice)
        {
            var latticeNodes = lattice.LatticeFormalConcepts;
            foreach (var node in latticeNodes)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                dictionary.Add("id", node.Id);
                string attributesAggregate = AttributesCSV(node);
                dictionary.Add("attributes", attributesAggregate);
                string objectsAggregate = ObjectsCSV(node);
                dictionary.Add("objects", objectsAggregate);
                dictionary.Add("level", node.Level);

                CypherQuery query = new CypherQuery("CREATE (node: Concept{ id: {id}, attributes: {attributes}, objects: {objects}, level: {level}})",
                           dictionary, CypherResultMode.Set);

                ((IRawGraphClient)client).ExecuteCypher(query);
            }

            foreach (var node in latticeNodes)
            {
                foreach (var subnode in node.Subconcepts)
                {
                    var dictionary = new Dictionary<string, object>();
                    dictionary.Add("node_id", node.Id);
                    dictionary.Add("subnode_id", subnode.Id);
                    var query = new CypherQuery("MATCH (node:Concept{id: {node_id}}),(subnode:Node{id: {subnode_id}}) CREATE (node)-[relation:intent]->(subnode)",
                               dictionary, CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(query);
                }
                /*foreach (var supernode in node.Superconcepts)
                {
                    var dictionary = new Dictionary<string, object>();
                    dictionary.Add("node_id", node.Id);
                    dictionary.Add("supernode_id", supernode.Id);
                    var query = new CypherQuery("MATCH (node:Node{id: {node_id}}),(supernode:Node{id: {supernode_id}}) CREATE (node)-[relation:extent]->(supernode)",
                               dictionary, CypherResultMode.Set);
                    ((IRawGraphClient)client).ExecuteCypher(query);
                }*/
            }
        }

        /// <summary>
        /// Imports the fca lattice like CSV.
        /// </summary>
        /// <param name="lattice">The lattice.</param>
        public void ImportFCALatticeLikeCSV(ConceptLattice lattice)
        {
            /// Concepts import
            var lattice_conceptsfileName = "lattice_concepts.csv";
            using (var sw = new StreamWriter(File.OpenWrite(lattice_conceptsfileName)))
            {
                // CSV header
                sw.WriteLine("id,objects,attributes,level");
                foreach (var lfc in lattice.LatticeFormalConcepts)
                {
                    sw.WriteLine(string.Format("{0},{1},{2},{3}", lfc.Id, ObjectsCSV(lfc), AttributesCSV(lfc), lfc.Level));
                }
            }
            var conceptsfileInfo = new FileInfo(lattice_conceptsfileName);
            var conceptsFileUri = new Uri("file://////" + conceptsfileInfo.FullName);
            ///When you get "Could not load the external resourses..." exception.
            ///In the Neo4j desktop select the database you are using,
            ///go to the setting and there you will find the solution... 
            ///just comment the "dbms.directories.import=import" line
            client.Cypher
                .LoadCsv(conceptsFileUri, "csvConcept", true)
                .Create("(node:Concept {id:csvConcept.id, attributes:csvConcept.attributes, objects:csvConcept.objects, level:csvConcept.level})")
                .ExecuteWithoutResults();
            conceptsfileInfo.Delete();

            /// Edges import
            var edgesfileName = "edges.csv";
            using (var sw = new StreamWriter(File.OpenWrite(edgesfileName)))
            {
                // CSV header
                sw.WriteLine("node_id,subnode_id");
                foreach (var node in lattice.LatticeFormalConcepts)
                {
                    foreach (var subnode in node.Subconcepts)
                    {
                        sw.WriteLine(string.Format("{0},{1}", node.Id, subnode.Id));
                    }
                }
            }
            var edgesfileInfo = new FileInfo(edgesfileName);
            var edgessFileUri = new Uri("file://////" + edgesfileInfo.FullName);
            client.Cypher
                .LoadCsv(edgessFileUri, "csvEdge", true)
                .Match("(node:Concept{id:csvEdge.node_id}),(subnode:Concept{id:csvEdge.subnode_id})")
                .Create("(node)-[relation:intent]->(subnode)")
                .ExecuteWithoutResults();
            edgesfileInfo.Delete();
        }

        private string ObjectsCSV(LatticeFormalConcept node)
        {
            var objectsAggregate = String.Empty;
            var objectNames = node.Objects.Select(o => CSVNormalize(o.Name)).ToList();
            if (node.Objects.Any())
                objectsAggregate = objectNames.Aggregate((i, j) => i + " " + j);
            return objectsAggregate;
        }
        private string AttributesCSV(LatticeFormalConcept node)
        {
            var attributesAggregate = String.Empty;
            var attributeNames = node.Attributes.Select(a => CSVNormalize(a.Name)).ToList();
            if (node.Attributes.Any())
                attributesAggregate = attributeNames.Aggregate((i, j) => i + " " + j);
            return attributesAggregate;
        }


        public string SearchForObjects(string attribute)
        {
            var res = client.Cypher
                 .Match("(n:Concept)")
                 .Where("n.attributes CONTAINS " + "'" + attribute + "'")
                 .Return(n => n.As<Neo4JNode>().objects)
                 .OrderByDescending("n.level")
                 .Limit(1)
                 .Results;
            return res.FirstOrDefault();
        }

        public IEnumerable<NodeMatch> SearchForObjects(string _object, IEnumerable<string> attributes)
        {
            var res = client.Cypher
                 .Match("(n:Concept)")
                 .Where("n.objects CONTAINS " + "'" + CSVNormalize(_object) + "'")
                 .AndWhere("n.level = '1'")
                 .With("n")
                 .Match("(n)-[r:intent]->(m)")
                 .Return(m => m.As<Neo4JNode>())
                 .Results;
            //return objectNames;
            var nodeMatches = new List<NodeMatch>();
            foreach (var node in res)
            {
                node.Objects = node.objects.Split(' ').Select(s => CSVDenormalize(s));
                node.Attributes = node.attributes.Split(' ').Select(s => CSVDenormalize(s).Replace(" ", "_"));
                var match = new NodeMatch()
                {
                    Matches = node.Attributes.Intersect(attributes).Count(),
                    Node = node
                };
                nodeMatches.Add(match);
            }
            return nodeMatches;
        }

        public IEnumerable<string> SearchForObjects(IEnumerable<AttributeLikes> liked, IEnumerable<string> likedMovies)
        {
            var res = client.Cypher
                 .Match("(n:Concept)")
                 .Return(n => n.As<Neo4JNode>())
                 .Results;

            var matches = new List<NodeMatch>();
            //var maxNode = res.Where(r => r.level == 0).First();
            //var maxPoints = 0;
            foreach (var node in res)
            {
                node.Objects = node.objects?.Split(' ').Select(s => CSVDenormalize(s)) ?? new List<string>();
                node.Attributes = node.attributes?.Split(' ').Select(s => CSVDenormalize(s).Replace(" ", "_")) ?? new List<string>();
                var points = liked.Where(l => node.Attributes.Any(a => a == l.Attribute)).Select(l => l.Likes).DefaultIfEmpty().Sum();
                var nodeMatch = new NodeMatch { Matches = points, Node = node };
                matches.Add(nodeMatch);
                //if (maxPoints < points)
                //{
                //    maxPoints = points;
                //    maxNode = node;
                //}
            }
            matches = matches
                .Where(m => m.Node.Objects.Any() && (m.Node.Objects.Count() == 1 && !m.Node.Objects.Intersect(likedMovies).Any()))
                .OrderByDescending(m => m.Matches).ToList();

            var movies = matches.Take(50)
                .SelectMany(m => m.Node.Objects, (m, s) => s)
                .Distinct();

            return movies;
        }

        public class NodeMatch
        {
            public int Matches { get; set; }
            public Neo4JNode Node { get; set; }
        }

        public class AttributeLikes
        {
            public int Likes { get; set; }
            public string Attribute { get; set; }
        }

        public IEnumerable<string> SearchForObjects(IEnumerable<string> attributes)
        {
            var query = client.Cypher
                     .Match("(n:Concept)")
                     .Where("n.attributes CONTAINS " + "'" + attributes.FirstOrDefault() + "'");
            foreach (var attribute in attributes)
            {
                query = query.AndWhere("n.attributes =~ " + "'.*" + attribute.Replace("'", ".") + ".*'");
            }
            var res = query
                 .Return(n => n.As<Neo4JNode>().objects)
                 .OrderByDescending("n.level")
                 .Limit(10)
                 .Results;
            return res;
        }

        #region Helpers

        private string CSVNormalize(string entry)
        {
            var normalized = entry.Replace(",", "~").Replace(" ", "_").Replace("'", "+");
            return normalized;
        }
        private string CSVDenormalize(string entry)
        {
            var denormalized = entry.Replace("~", ",").Replace("_", " ").Replace("+", "'");
            return denormalized;
        }

        //pomocna klasa za deserijalizaciju rezultata iz upita za pretragu
        public class Neo4JNode
        {
            public string id { get; set; }
            public int level { get; set; }
            public string attributes { get; set; }
            public string objects { get; set; }
            public IEnumerable<string> Objects { get; set; }
            public IEnumerable<string> Attributes { get; set; }
        }
        #endregion
    }
}
