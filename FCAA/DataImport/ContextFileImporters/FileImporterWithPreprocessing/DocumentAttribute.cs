using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCAA.DataImport.ContextFileImporters
{
    class DocumentAttribute
    {
        public string AttributeName { get; set; }
        public Dictionary<string,double> AttributeMeasures { get; set; }
        public double MedianValue { get; set; }
        public int NumOfSimilarAttributes { get; set; }

        private static readonly Random RandomGenerator = new Random();

        public DocumentAttribute(string attributeName)
        {
            AttributeName = attributeName;
            this.AttributeMeasures = new Dictionary<string, double>();
            this.MedianValue = 0;
            this.NumOfSimilarAttributes = 0;
        }

        /// <summary>
        /// Prolazi kroz sve atribute i setuje sve vrednosti konkretnog atributa
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="treshold"></param>
        public void CalculateSimilarityForAllAttributes(HashSet<string> attributes, double treshold)
        {
            //TODO: ovde je potrebno izmeniti da kada se dobije servis koji ce da vrati slicnost za niz atributa da se s njim u nastavku radi
            double medianSum = 0;
            foreach (var item in attributes)
            {
                var similarity = CalulateSimilarity(item);
                this.AttributeMeasures.Add(item, similarity);
                medianSum += similarity;
                if (similarity >= treshold)
                    this.NumOfSimilarAttributes++;
            }
            this.MedianValue = medianSum / attributes.Count;
        }

        public double CalulateSimilarity(string attr)
        {
            var res = RandomGenerator.NextDouble();
            return res;
        }

        public override bool Equals(object obj)
        {
            var attribute = obj as DocumentAttribute;
            return attribute != null &&
                   NumOfSimilarAttributes == attribute.NumOfSimilarAttributes;
        }

        public override int GetHashCode()
        {
            return -529510658 + NumOfSimilarAttributes.GetHashCode();
        }
    }
}
