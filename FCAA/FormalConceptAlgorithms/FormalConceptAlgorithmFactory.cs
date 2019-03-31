using FCAA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCAA.FormalConceptAlgorithms
{
    public class FormalConceptAlgorithmFactory
    {
        public static IFormalConceptAlgorithm ProduceAlgorithm(FormalConceptAlgorithms algorithm, FormalContext context)
        {
            switch (algorithm)
            {
                case FormalConceptAlgorithms.NextClosure:
                    return new NextClosureAlgorithm(context);
                default:
                    throw new Exception("Algorithm not supported.");
            }
        }
    }

    public enum FormalConceptAlgorithms
    {
        NextClosure
    }
}
