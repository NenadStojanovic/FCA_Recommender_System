using FCAA.Data;
using System.Collections.Generic;

namespace FCAA.FormalConceptAlgorithms
{
    public interface IFormalConceptAlgorithm
    {
        string Name { get; }
        IEnumerable<FormalConcept> FormalConcepts();
    }
}
