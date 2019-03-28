using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FCAA.Data.Lattice
{
    public class ConceptLattice
    {
        public FormalContext FormalContext { get; set; }
        public IEnumerable<FormalConcept> FormalConcepts { get; private set; }

        private LatticeFormalConcept[] latticeFormalConcepts;
        public IEnumerable<LatticeFormalConcept> LatticeFormalConcepts => latticeFormalConcepts;
        public LatticeFormalConcept LatticeCommonSubconcept { get; private set; }
        public LatticeFormalConcept LatticeCommonSuperconcept { get; private set; }
        public int Height { get; private set; }

        public ConceptLattice(IEnumerable<FormalConcept> formalConcepts, FormalContext formalContext)
        {
            FormalConcepts = formalConcepts ?? throw new ArgumentNullException(nameof(formalConcepts));
            FormalContext = formalContext ?? throw new ArgumentNullException(nameof(formalContext));
            MakeLatticeFormalConcepts();
            ComputeLatticeFormalConceptsLevels();
        }

        private void MakeLatticeFormalConcepts()
        {
            latticeFormalConcepts = new LatticeFormalConcept[FormalConcepts.Count()];
            for (int i = 0; i < FormalConcepts.Count(); i++)
            {
                var latticeFormalConcept = new LatticeFormalConcept(FormalConcepts.ElementAt(i));
                latticeFormalConcept.Id = latticeFormalConcept.GetHashCode().ToString();
                latticeFormalConcepts[i] = latticeFormalConcept;
            }

            var sortedLFCs = latticeFormalConcepts.OrderByDescending(lfc => lfc.Attributes.Count()).ToArray();
            foreach (var currentLFC in sortedLFCs)
            {
                var potentialSubConcepts = sortedLFCs.Where(lfc => lfc.AttributeCount < currentLFC.AttributeCount);
                foreach (var pLFC in potentialSubConcepts)
                {
                    var clfcas = new HashSet<Attribute>(currentLFC.Attributes);
                    var plfcas = new HashSet<Attribute>(pLFC.Attributes);
                    if (plfcas.IsSubsetOf(clfcas))
                    {
                        var plfcSc = pLFC.Superconcepts.Where(c => c.AttributeCount > currentLFC.AttributeCount).ToList();
                        foreach (var superLFC in plfcSc)
                        {
                            var suplfcas = new HashSet<Attribute>(superLFC.Attributes);
                            if (clfcas.IsSubsetOf(suplfcas))
                            {
                                superLFC.Subconcepts.Remove(pLFC);
                                pLFC.Superconcepts.Remove(superLFC);
                            }
                        }
                        currentLFC.Subconcepts.Add(pLFC);
                        pLFC.Superconcepts.Add(currentLFC);
                    }
                }

            }
            LatticeCommonSuperconcept = sortedLFCs.First();
            LatticeCommonSubconcept = sortedLFCs.Last();

        }
        private void ComputeLatticeFormalConceptsLevels()
        {
            var inQueue = new HashSet<LatticeFormalConcept>();
            var queue = new Queue<LatticeFormalConcept>();
            LatticeCommonSuperconcept.Level = 0;
            queue.Enqueue(LatticeCommonSuperconcept);
            inQueue.Add(LatticeCommonSuperconcept);
            while (queue.Any())
            {
                var node = queue.Dequeue();
                foreach(var subnode in node.Subconcepts)
                {
                    if(subnode.Level <= node.Level){
                        subnode.Level = node.Level + 1;
                    }
                    if (!inQueue.Contains(subnode))
                    {
                        queue.Enqueue(subnode);
                    }
                }
                inQueue.Add(node);
            }
            Height = 0;
            if(latticeFormalConcepts.Any())
                Height = LatticeFormalConcepts.Max(fc => fc.Level);
        }
    }
}
