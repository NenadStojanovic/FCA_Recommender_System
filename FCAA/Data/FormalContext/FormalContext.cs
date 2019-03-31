using System;
using System.Collections.Generic;
using System.Linq;

namespace FCAA.Data
{
    public class FormalContext
    {
        private Object[] _objects { get; set; }
        public Object[] ObjectsArray => _objects;
        public HashSet<Object> ObjectsSet => new HashSet<Object>(_objects);

        private Attribute[] _attributes { get; set; }
        public Attribute[] AttributesArray => _attributes;
        public HashSet<Attribute> AttributesSet => new HashSet<Attribute>(_attributes);

        public Dictionary<Object, HashSet<Attribute>> ObjectAttributes { get; private set; }
        public Dictionary<Attribute, HashSet<Object>> AttributeObjects { get; private set; }

        public bool[,] RelationsMatrix { get; private set; }

        public FormalContext(
            IEnumerable<Object> objects,
            IEnumerable<Attribute> attributes,
            Dictionary<Object, HashSet<Attribute>> objectAttributes,
            Dictionary<Attribute, HashSet<Object>> attributeObjects)
        {
            _objects = objects?.ToArray() ?? throw new ArgumentNullException(nameof(objects));
            _attributes = attributes?.ToArray() ?? throw new ArgumentNullException(nameof(attributes));
            ObjectAttributes = objectAttributes ?? throw new ArgumentNullException(nameof(objectAttributes));
            AttributeObjects = attributeObjects ?? throw new ArgumentNullException(nameof(attributeObjects));
            ComputeMatrixRelations();
        }

        public FormalContext(
            IEnumerable<Object> objects, 
            IEnumerable<Attribute> attributes,
            bool[,] relations)
        {
            _objects = objects?.ToArray() ?? throw new ArgumentNullException(nameof(objects));
            _attributes = attributes?.ToArray() ?? throw new ArgumentNullException(nameof(attributes));
            if (relations.GetLength(0) != _objects.Count()) throw new Exception("Relations matrix height not coresponding to object count.");
            if (relations.GetLength(1) != _attributes.Count()) throw new Exception("Relations matrix width not coresponding to attribute count.");

            RelationsMatrix = relations ?? throw new ArgumentNullException(nameof(relations));
            ComputeRelationsDictionarySets();
        }

        private void ComputeMatrixRelations()
        {
            RelationsMatrix = new bool[_objects.Count(), _attributes.Count()];
            var attributeIndexes = new Dictionary<Attribute, int>();
            for (int i = 0; i < _attributes.Count(); i++)
                attributeIndexes[AttributesArray[i]] = i;
            for(int i = 0; i < _objects.Count(); i++)
            {
                var object_ = ObjectsArray[i];
                var objectAttributes = ObjectAttributes[object_];
                foreach (var attribute in objectAttributes)
                    RelationsMatrix[i, attributeIndexes[attribute]] = true;
            }
        }

        private void ComputeRelationsDictionarySets()
        {
            ObjectAttributes = new Dictionary<Object, HashSet<Attribute>>();
            var height = RelationsMatrix.GetLength(0);
            var length = RelationsMatrix.GetLength(1);
            for (int i = 0; i < height; i++)
            {
                var object_ = ObjectsArray[i];
                var objectAttributes = new HashSet<Attribute>();
                for (int j = 0; j < length; j++)
                    if (RelationsMatrix[i, j])
                    {
                        var attribute = AttributesArray[j];
                        objectAttributes.Add(attribute);
                    }
                ObjectAttributes[object_] = objectAttributes;
            }

            AttributeObjects = new Dictionary<Attribute, HashSet<Object>>();
            for (int j = 0; j < length; j++)
            {
                var attribute = AttributesArray[j];
                var attributeObject = new HashSet<Object>();
                for (int i = 0; i < height; i++)
                    if (RelationsMatrix[i, j])
                    {
                        var object_ = ObjectsArray[i];
                        attributeObject.Add(object_);
                    }
                AttributeObjects[attribute] = attributeObject;
            }
        }
    }
}
