using FCAA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Attribute = FCAA.Data.Attribute;

namespace FCAA.DataImport.Preprocess
{
    public class NLPFormalContextAttributesReduction
    {
        public string NLPServiceUrl { get; set; } = "http://160.99.9.216/Description/api/description";
        public double Treshold { get; set; } = 0.2;

        //public FormalContext AttributeReduction(FormalContext context)
        //{
        //    var attributes_d = new Dictionary<string, Attribute>();
        //    context.AttributesSet.ToList().ForEach(a => attributes_d[a.Name] = a);
        //    var attributes = new HashSet<string>(context.AttributesSet.Select(a => a.Name));
        //    //ovde se smestaju atributi koji se proglase istim tako da se oni ne obradjuju kad se opet naidje na njih
        //    var excessAttributes = new HashSet<string>();
        //    foreach (var selectedAttr in attributes)
        //    {
        //        if (excessAttributes.Contains(selectedAttr))
        //            continue;
        //        //TODO: Ovde ubaciti izmenu kada se servis za slicnost izmeni da prima niz atributa
        //        foreach (var attr in attributes)
        //        {
        //            //TODO: Ovo mora da se uradi jer je svaki atribut isti sa samim sobom, razmisliti o potencijalnoj optimizaciji ovog dela jer ipak mogu da se javi istri atributi u tagovima
        //            if (excessAttributes.Contains(attr) || selectedAttr.Equals(attr))
        //                continue;
        //            bool result = AreAttributesNLPEqual(attr, selectedAttr);
        //            if (result)
        //            {
        //                excessAttributes.Add(attr);
        //                RefactorDocumentTags(documents, selectedAttr, attr);
        //            }
        //        }
        //    }

        //    FormalConcept formalConcept
        //    return documents;
        //}

        //void RefactorDocumentTags(List<Document> documents, string mainAttr, string replacingAttr)
        //{
        //    for (int i = 0; i < documents.Count; i++)
        //    {
        //        if (documents[i].tags.Contains(replacingAttr))
        //        {
        //            documents[i].tags.Remove(replacingAttr);
        //            documents[i].tags.Add(mainAttr);
        //        }

        //    }
        //}

        private bool AreAttributesNLPEqual(Attribute left, Attribute right)
        {
            //TODO: Ovde je potrebno pozvati pravi servis koji leksicki uporedjuje dve recenice
            //TODO: Dodatno, od tagova koji se sastoje vise reci (odvojene _ ili - ) potrebno je napraviti recenice pa tako leksicki uporedjivati, odnosno cilj je da ova
            //metoda uvek uporedjuje dve recenice cak i kad se one sastoje samo do po jedne reci

            double similarity = 0;
            var firstSentence = left.Name.Replace("-", " ");
            var secondSentence = right.Name.Replace("-", " ");
            string myJson = "{ 'IRI1': '1', 'DESC1' : '" + firstSentence + "','IRI2': '2','DESC2' : '" + secondSentence + "' }";
            using (var client = new HttpClient())
            {
                var postTask = client.PostAsync(
                    NLPServiceUrl,
                     new StringContent(myJson, Encoding.UTF8, "application/json"));
                var result = postTask.Result;
                similarity = double.Parse(result.Content.ToString());
            }
            return similarity > Treshold;
        }
    }
}
