using FCAA.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Object = FCAA.Data.Object;
using PreprocessingAttribute = FCAA.Data.PreprocessingAttribute;
using Attribute = FCAA.Data.Attribute;

namespace FCAA.DataImport.ContextFileImporters
{
    public class FCAFileReaderWithPreprocessing : IContextFileImporter
    {
        public string Description => "Json file reader(PREPROCESSED) [{name: #string, tags : [#string, ...], ...][...]";

        public ICollection<Attribute> ImportContextAttributes(string filePath)
        {
            throw new NotImplementedException();
        }
        public ICollection<Object> ImportContextObjects(string filePath)
        {
            throw new NotImplementedException();
        }

        public FormalContext ImportContext(string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task<FormalContext> ImportContextAsync(string filePath)
        {
            FilePreprocessingManager preprocessingManager = new FilePreprocessingManager() { Treshold = 0.2 };

            HashSet<string> allPreprocessingAttributes = new HashSet<string>();
            List<Object> objects = new List<Object>();
            List<Attribute> attributes = new List<Attribute>();
            var objectPreprocessingAttributes = new Dictionary<Object, HashSet<Attribute>>();
            var attributeObjects = new Dictionary<Attribute, HashSet<Object>>();

            List<Document> documents = await preprocessingManager.PreprocessFileAsync(filePath);

            foreach (var document in documents)
            {
                if (document.tags.Count == 0) continue;
                Object obj = new Object(document.name);
                objects.Add(obj);

                objectPreprocessingAttributes.Add(obj, new HashSet<Attribute>());
                allPreprocessingAttributes.UnionWith(document.tags);
                foreach (var tag in document.tags)
                {
                    PreprocessingAttribute attr = new PreprocessingAttribute(tag);

                    if (!(attributes.Any(a => a.Name == tag)))
                    {
                        attributes.Add(attr);
                        attributeObjects.Add(attr, new HashSet<Object>());
                    }

                    attributeObjects[attr].Add(obj);
                    objectPreprocessingAttributes[obj].Add(attr);
                }
            }
 
            return new FormalContext(objects, attributes, objectPreprocessingAttributes, attributeObjects);
        }
    }
}
