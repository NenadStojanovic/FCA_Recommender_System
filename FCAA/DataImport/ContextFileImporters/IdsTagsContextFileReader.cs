using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FCAA.DataImport;

namespace FCAA.Data
{
    /// <summary>
    /// FCA file reader that read formal concept data from json file that keeps data with documents and tags
    /// Format: [{id: #string, tags : [#string, ...], category : [#string, ...]}, ...][...]
    /// WARNING: Files contains arrays of arrays that are not separated and we replace "][" with "," to get single array
    /// </summary>
    /// <seealso cref="NextClosureAlgorithm.Util.IFCAFileReader" />
    public class IdsTagsContextFileReader : IContextFileImporter
    {
        public string Description { get; } = "Json file reader [{id: #string, tags : [#string, ...], ...][...]";

        public FormalContext ImportContext(string filePath)
        {
            var json = ReadJsonData(filePath);
            dynamic documents = JsonConvert.DeserializeObject(json);

            // Tags forms context attributes
            var attribute_names = new HashSet<string>();
            /// Objects dictionary
            var objects_d = new Dictionary<string, Object>();
            /// Attributes dictionary
            var attributes_d = new Dictionary<string, Attribute>();

            var objectHaveAttributes = new Dictionary<string, HashSet<string>>();
            var attributHaveObjects = new Dictionary<string, HashSet<string>>();
            foreach (var document in documents)
            {
                // Ignoring objects that don't have tags as attributes
                if (document.tags.Count <= 0)
                    continue;

                // Getting objects, documents are objects
                string object_name = document.id;
                if (!objects_d.ContainsKey(object_name))
                    objects_d[object_name] = new Object(object_name);

                objectHaveAttributes[object_name] = new HashSet<string>();

                // Getting attributes
                foreach (string tag in document.tags)
                {
                    if (!attribute_names.Contains(tag))
                    {
                        attribute_names.Add(tag);
                        attributHaveObjects[tag] = new HashSet<string>();
                    }

                    attributHaveObjects[tag].Add(object_name);
                    objectHaveAttributes[object_name].Add(tag);
                }
            }
            // Objects list
            var objects = new List<Object>();
            objects = objects_d.Values.ToList();

            // Attributes list
            var attributes = new List<Attribute>();
            foreach (var attribute_name in attribute_names)
            {
                var attribute = new Attribute(attribute_name);
                attributes.Add(attribute);
                attributes_d[attribute_name] = attribute;
            }

            var attributeObjects = new Dictionary<Attribute, HashSet<Object>>();
            foreach (var pair in attributHaveObjects)
                attributeObjects[attributes_d[pair.Key]] = new HashSet<Object>(pair.Value.Select(o => objects_d[o]));

            var objectAttributes = new Dictionary<Object, HashSet<Attribute>>();
            foreach (var pair in objectHaveAttributes)
                objectAttributes[objects_d[pair.Key]] = new HashSet<Attribute>(pair.Value.Select(a => attributes_d[a]));

            var fcacontext = new FormalContext(objects, attributes, objectAttributes, attributeObjects);
            return fcacontext;
        }
        public ICollection<Attribute> ImportContextAttributes(string filePath)
        {
            var attribute_tags = new HashSet<string>();
            var json = ReadJsonData(filePath);
            dynamic documents = JsonConvert.DeserializeObject(json);
            foreach (var document in documents)
            {
                foreach (string tag in document.tags)
                {
                    if (!attribute_tags.Contains(tag))
                    {
                        attribute_tags.Add(tag);
                    }
                }
            }
            var attributes = new List<Attribute>();
            attribute_tags.ToList().ForEach(a => attributes.Add(new Attribute(a)));
            return attributes;
        }
        public ICollection<Object> ImportContextObjects(string filePath)
        {
            var objects_s = new HashSet<string>();
            var json = ReadJsonData(filePath);
            dynamic documents = JsonConvert.DeserializeObject(json);
            foreach (var document in documents)
            {
                string object_name = document.id;
                objects_s.Add(object_name);
            }
            var objects = new List<Object>();
            objects_s.ToList().ForEach(o => objects.Add(new Object(o)));
            return objects;
        }

        public Task<FormalContext> ImportContextAsync(string filePath)
        {
            var fcacontext = ImportContext(filePath);
            return Task.FromResult(fcacontext);
        }

        private string ReadJsonData(string filePath)
        {
            var streamReader = new StreamReader(filePath);
            var json = streamReader.ReadToEnd();
            streamReader.Close();
            // Fixing broken json arrays
            if (json.IndexOf("][") > 0)
            {
                json = json.Replace("][", ",");
            }
            return json;
        }
    }
}
