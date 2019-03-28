using System;
using System.Collections.Generic;
using System.Linq;

namespace FCAA.Data.Lattice
{
    public class LatticeFormalConcept
    {
        public string Id { get; set; }
        public int Level { get; set; }
        public FormalConcept FormalConcept { get; private set; }

        public int AttributeCount => Attributes.Count();
        public IEnumerable<Attribute> Attributes => FormalConcept.Attributes;
        public int ObjectsCount => Objects.Count();
        public IEnumerable<Object> Objects => FormalConcept.Objects;

        public ICollection<LatticeFormalConcept> Superconcepts { get; set; }
        public ICollection<LatticeFormalConcept> Subconcepts { get; set; }

        public LatticeFormalConcept(FormalConcept formalConcept)
        {
            FormalConcept = formalConcept ?? throw new ArgumentNullException(nameof(formalConcept));
            Superconcepts = new HashSet<LatticeFormalConcept>();
            Subconcepts = new HashSet<LatticeFormalConcept>();
        }
    }
}
