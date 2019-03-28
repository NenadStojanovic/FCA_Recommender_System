using FCAA.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Object = FCAA.Data.Object;
using Attribute = FCAA.Data.Attribute;
using FCAA.DataImport;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace FCAA.DataImport.ContextFileImporters
{
    public class DefaultContextFileImporter : IContextFileImporter
    {
        public string Description => "Default Json file reader [{object: #string, attributes : [#string, ...], ...][...]";

        public FormalContext ImportContext(string filePath)
        {
            var json = ReadJsonData(filePath);
            dynamic datas = JsonConvert.DeserializeObject(json);

            var attribute_names = new HashSet<string>();
            /// Objects dictionary
            var objects_d = new Dictionary<string, Object>();
            /// Attributes dictionary
            var attributes_d = new Dictionary<string, Attribute>();

            var objectHaveAttributes = new Dictionary<string, HashSet<string>>();
            var attributHaveObjects = new Dictionary<string, HashSet<string>>();
            foreach (var data in datas)
            {
                if (data.attributes.Count <= 0)
                    continue;

                // Getting objects, datas are objects
                string object_name = data.GetType().GetProperty("object").GetValue(data, null);
                if (!objects_d.ContainsKey(object_name))
                    objects_d[object_name] = new Object(object_name);

                objectHaveAttributes[object_name] = new HashSet<string>();

                // Getting attributes
                foreach (string attribute in data.attributes)
                {
                    if (!attribute_names.Contains(attribute))
                    {
                        attribute_names.Add(attribute);
                        attributHaveObjects[attribute] = new HashSet<string>();
                    }

                    attributHaveObjects[attribute].Add(object_name);
                    objectHaveAttributes[object_name].Add(attribute);
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

        public Task<FormalContext> ImportContextAsync(string filePath)
        {
            var fcacontext = ImportContext(filePath);
            return Task.FromResult(fcacontext);
        }

        public ICollection<Attribute> ImportContextAttributes(string filePath)
        {
            var attribute_s = new HashSet<string>();
            var json = ReadJsonData(filePath);
            dynamic datas = JsonConvert.DeserializeObject(json);
            foreach (var data in datas)
            {
                foreach (string attribute in data.attributes)
                {
                    if (!attribute_s.Contains(attribute))
                    {
                        attribute_s.Add(attribute);
                    }
                }
            }
            var attributes = new List<Attribute>();
            attribute_s.ToList().ForEach(a => attributes.Add(new Attribute(a)));
            return attributes;
        }

        public ICollection<Object> ImportContextObjects(string filePath)
        {
            var objects_s = new HashSet<string>();
            var json = ReadJsonData(filePath);
            dynamic datas = JsonConvert.DeserializeObject(json);
            foreach (var data in datas)
            {
                string object_name = data.GetType().GetProperty("object").GetValue(data, null);
                objects_s.Add(object_name);
            }
            var objects = new List<Object>();
            objects_s.ToList().ForEach(o => objects.Add(new Object(o)));
            return objects;
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
