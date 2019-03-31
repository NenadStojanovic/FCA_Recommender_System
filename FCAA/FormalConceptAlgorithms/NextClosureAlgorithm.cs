using FCAA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Attribute = FCAA.Data.Attribute;
using Object = FCAA.Data.Object;

namespace FCAA.FormalConceptAlgorithms
{
    public class NextClosureAlgorithm : IFormalConceptAlgorithm
    {
        public string Name => "Next clousure";
        /// <summary>
        /// Fromal context
        /// </summary>
        public FormalContext FormalContext { get; private set; }

        /// <summary>
        /// Objects lexical positions used for next closure lexical ordering.
        /// </summary>
        public Dictionary<Object, int> ObjectsLexicalPosition { get; private set; }
        /// <summary>
        /// Attributes lexical positions used for next closure lexical ordering.
        /// </summary>
        public Dictionary<Attribute, int> AttributesLexicalPosition { get; private set; }

        #region Optimization for speeding up formal concept computation

        /// <summary>
        /// Hash dictionary that store intents for computed extents.
        /// Used to speed up formal concept generation after finding all closed extents.
        /// </summary>
        public Dictionary<IEnumerable<Object>, IEnumerable<Attribute>> ExtentIntetnts { get; private set; }
        /// <summary>
        /// Hash dictionary that store extents for computed intents.
        /// Used to speed up formal concept genration after finding all closed intents.
        /// </summary>
        public Dictionary<IEnumerable<Attribute>, IEnumerable<Object>> IntetntExtents { get; private set; }

        #endregion

        public NextClosureAlgorithm(FormalContext formalContext)
        {
            FormalContext = formalContext ?? throw new ArgumentNullException(nameof(formalContext));

            /// Object lexical positions are object positions in formal contex objects array
            ObjectsLexicalPosition = new Dictionary<Object, int>();
            var objects = FormalContext.ObjectsArray;
            for (int i = 0; i < objects.Length; i++)
                ObjectsLexicalPosition[objects[i]] = i;

            /// Attribute lexical positions are attribute positions in formal contex attribute array
            AttributesLexicalPosition = new Dictionary<Attribute, int>();
            var attributes = FormalContext.AttributesArray;
            for (int i = 0; i < attributes.Length; i++)
                AttributesLexicalPosition[attributes[i]] = i;

            ExtentIntetnts = new Dictionary<IEnumerable<Object>, IEnumerable<Attribute>>();
            IntetntExtents = new Dictionary<IEnumerable<Attribute>, IEnumerable<Object>>();
        }

        #region Operations on attributes and objects

        /// <summary>
        /// Extent of specified attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public IEnumerable<Object> Extent(IEnumerable<Attribute> attributes)
        {
            var attributeSubSet = attributes as HashSet<Attribute> ?? new HashSet<Attribute>(attributes);

            // Extent of empty attribute set is the whole object set
            if (!attributeSubSet.Any())
                return FormalContext.ObjectsSet;

            // Computing extents of single attributes
            if (attributeSubSet.Count == 1)
            {
                var attributeObjects = FormalContext.AttributeObjects[attributeSubSet.First()];
                return new HashSet<Object>(attributeObjects);
            }

            var extent = new HashSet<Object>();
            // Computing extents when passed attributes set have more than one attribute
            foreach (var obj in FormalContext.ObjectsSet)
            {
                var objectAttributes = FormalContext.ObjectAttributes[obj];
                if (attributeSubSet.IsSubsetOf(objectAttributes))
                    extent.Add(obj);
            }

            return extent;
        }
        /// <summary>
        /// Intent of specified objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        public IEnumerable<Attribute> Intent(IEnumerable<Object> objects)
        {
            // Used for hash set operations
            var objectSubSet = objects as HashSet<Object> ?? new HashSet<Object>(objects);

            // Intent of empty object set is the whole attribute set
            if (!objectSubSet.Any())
                return new HashSet<Attribute>(FormalContext.AttributesSet);

            // Computing intents of single objects
            if (objectSubSet.Count == 1)
            {
                var objectAttributes = FormalContext.ObjectAttributes[objectSubSet.First()];
                return new HashSet<Attribute>(objectAttributes);
            }

            var intent = new HashSet<Attribute>();
            // Computing intents when passed objects set have more than one object
            foreach (var attribute in FormalContext.AttributesSet)
            {
                var attributeObjects = FormalContext.AttributeObjects[attribute];
                if (objectSubSet.IsSubsetOf(attributeObjects))
                    intent.Add(attribute);
            }
            return intent;
        }

        /// <summary>
        /// Closures of specified objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <returns></returns>
        public IEnumerable<Object> Closure(IEnumerable<Object> objects)
        {
            var intent = Intent(objects);
            var closure = Extent(intent);

            // Cache extent intents
            ExtentIntetnts[closure] = intent;
            return closure;
        }
        /// <summary>
        /// Closures of specified attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public IEnumerable<Attribute> Closure(IEnumerable<Attribute> attributes)
        {
            var extent = Extent(attributes);
            var closure = Intent(extent);

            // Cache intent extents
            IntetntExtents[closure] = extent;
            return closure;
        }

        #endregion

        #region Algorithm 

        /// <summary>
        /// Compute formal concept intents with Next Closure algorithm.
        /// </summary>
        /// <remarks>
        /// ALGORITHM PSEUDO CODE:
        /// 
        /// G - Set of all attributes
        ///     Extents(G)
        ///         B := FirstClosure()
        ///         while(B != null)
        ///             output B
        ///             B := NextExtent(B,G)
        ///             
        ///     FirstClosure()
        ///         return ∅"
        ///         
        ///     NextExtent(B,G)
        ///         for all g ∈ G in reverse order do
        ///             if g ∈ B then
        ///                 B := B \ {g}
        ///             else
        ///                 A := (B ∪ {g})"
        ///                 if A\B contains no element < g then
        ///                     return A
        ///          return null
        /// </remarks>
        /// <returns></returns>
        public IEnumerable<IEnumerable<Object>> NextExtent()
        {
            /// Set G is set of all attributes
            var setG = FormalContext.ObjectsSet;

            // B = FirstClousure(") = ∅"
            var setB = FirstExtent();
            var extents = new List<IEnumerable<Object>>();
            while (setB != null)
            {
                // Output B
                extents.Add(setB);
                setB = NextExtent(setB, setG);
            }
            return extents;
        }
        /// <summary>
        /// Find first closed set that is closure of the empty attribute set.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Object> FirstExtent()
        {
            return Closure(new HashSet<Object>());
        }
        /// <summary>
        /// Find next smallest closed set of objects larger than given set B ⊂ G.
        /// </summary>
        /// <param name="B">Set B that is subs set of G.</param>
        /// <param name="M">Set of all formal context objects M.</param>
        /// <returns></returns>
        private IEnumerable<Object> NextExtent(IEnumerable<Object> B, IEnumerable<Object> G)
        {
            // Coping set B for modification
            var _B = new HashSet<Object>(B);

            /// Forech loop provides lecticlly ordered iteration through reverse ordered set G.
            /// Set is reverse ordered to provide iteration from lecticlly largest to smallest object.
            /// For each iteration gi is largest element of remaing set G.
            foreach (var g in G.Reverse())
            {
                /// This part in foreach loop calculates B ⊕ gi = ((B ∩ {g1, . . . ,gi−1}) ∪ {gi})"
                /// and finds the next smallest closed set larger than given set B ⊂ G with respect
                /// to the lectic order, with gi being the largest element of G.

                /// if g ∈ B
                if (_B.Contains(g))
                {
                    /// Provides the (B ∩ {g1, . . . ,gi−1}) part of set B for the given gi
                    /// by removing all elements grater than gi-1 from set A.

                    /// B = B\{g}
                    _B.Remove(g);
                }
                else
                {
                    /// Part that calculates union with gi and form (G ∩ { g1, . . . ,gi−1}) ∪ { gi})
                    var BUg = new HashSet<Object>(_B);
                    BUg.Add(g);

                    /// A = B ⊕ gi = ((B ∩ {g1, . . . ,gi−1}) ∪ {gi})"
                    var setA = Closure(BUg);

                    // A\B
                    var AeB = setA.Except(_B);

                    // Part where we check is A next smallest closure B <i B ⊕ mi = A

                    // When A\B contains no elements < g, that is true when min elment is not < g, set A is next smallest closure.
                    // When A\B is empty set, set A is not next smallest closure.
                    if (AeB.Any() && !(AeB.Min(o => ObjectsLexicalPosition[o]) < ObjectsLexicalPosition[g]))
                        return setA;
                    else
                        ExtentIntetnts.Remove(setA);
                }
            }
            /// Null indicates that alghoritham is finished
            return null;
        }

        /// <summary>
        /// Compute formal concept intents with Next Closure algorithm.
        /// </summary>
        /// <remarks>
        /// ALGORITHM PSEUDO CODE:
        /// 
        /// M - Set of all objects
        ///     Intents(M)
        ///         A := FirstIntent()
        ///         while(A != null)
        ///             output A
        ///             A := NextIntent(A,M)
        ///             
        ///     FirstIntent()
        ///         return ∅"
        ///         
        ///     NextExtent(A,M)
        ///         for all m ∈ M in reverse order do
        ///             if m ∈ A then
        ///                 A := A \ {m}
        ///             else
        ///                 B := (A ∪ {m})"
        ///                 if B\A contains no element < m then
        ///                     return B
        ///          return null
        /// </remarks>
        /// <returns></returns>
        public IEnumerable<IEnumerable<Attribute>> NextIntent()
        {
            /// Set M is set of all attributes
            var setM = FormalContext.AttributesSet;

            // A = FirstClousure(") = ∅"
            var setA = FirstIntent();
            var intents = new List<IEnumerable<Attribute>>();
            while (setA != null)
            {
                // Output A
                intents.Add(setA);
                setA = NextIntent(setA, setM);
            }
            return intents;
        }
        /// <summary>
        /// Find first closed set that is closure of the empty object set.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<Attribute> FirstIntent()
        {
            return Closure(new HashSet<Attribute>());
        }
        /// <summary>
        /// Finds next smallest closed set of attributes larger than given set A ⊂ M 
        /// </summary>
        /// <param name="A">Set A that is sub set of M.</param>
        /// <param name="M">Set of all formal context objects M.</param>
        /// <returns>Null when algoritham is finished.</returns>
        private IEnumerable<Attribute> NextIntent(IEnumerable<Attribute> A, IEnumerable<Attribute> M)
        {
            // Coping set A for modification
            var _A = new HashSet<Attribute>(A);

            /// Forech loop provides lecticlly ordered iteration through reverse ordered set M.
            /// Set is reverse ordered to provide iteration from lecticlly largest to smallest attribute.
            /// For each iteration mi is largest element of remaing set M. 
            foreach (var m in M.Reverse())
            {
                /// This part in foreach loop calculates A ⊕ mi = ((A ∩ {m1, . . . ,mi−1}) ∪ {mi})"
                /// and finds the next smallest closed set larger than given set A ⊂ M with respect
                /// to the lectic order, with mi being the largest element of M.

                /// if m ∈ A
                if (_A.Contains(m))
                {
                    /// Provides the (A ∩ {m1, . . . ,mi−1}) part of set A for the given mi
                    /// by removing all elements grater than mi-1 from set A.

                    // A = A\{m}
                    _A.Remove(m);
                }
                else
                {
                    /// Part that calculates union with mi and form (A ∩ { m1, . . . ,mi−1}) ∪ { mi})
                    var AUm = new HashSet<Attribute>(_A);
                    AUm.Add(m);

                    /// B = A ⊕ mi = ((A ∩ {m1, . . . ,mi−1}) ∪ {mi})"
                    var setB = Closure(AUm);

                    // B\A
                    var BeA = setB.Except(_A);

                    // Part where we check is B next smallest closure A <i A ⊕ mi = B

                    // When B\A contains no elements < m, that is true when min elment is not < m, set B is next smallest closure.
                    // When B\A is empty set, set B is not next smallest closure.
                    if (BeA.Any() && !(BeA.Min(a => AttributesLexicalPosition[a]) < AttributesLexicalPosition[m]))
                        return setB;
                    else
                        IntetntExtents.Remove(setB);
                }
            }
            /// Null indicates that alghoritham is finished
            return null;
        }

        #endregion

        /// <summary>
        /// Compute formal concepts.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<FormalConcept> FormalConcepts()
        {
            /// Formal concepts are genereated from attribute or object set perspectiv based on their count 
            /// Reason is less combinations and faster formal concepts generation.
            if (FormalContext.AttributesSet.Count < FormalContext.ObjectsSet.Count)
                return FormalConceptsFromIntents();
            return FormalConceptsFromExtents();
        }

        public IEnumerable<FormalConcept> FormalConceptsFromIntents()
        {
            var intents = NextIntent();
            var formalConcepts = new HashSet<FormalConcept>();
            foreach (var intent in intents)
            {
                var extent = IntetntExtents[intent];
                var formalConcept = new FormalConcept(extent, intent, FormalContext);
                formalConcepts.Add(formalConcept);
            }
            IntetntExtents.Clear();
            return formalConcepts;
        }

        public IEnumerable<FormalConcept> FormalConceptsFromExtents()
        {
            var extents = NextExtent();
            var formalConcepts = new HashSet<FormalConcept>();
            foreach (var extent in extents)
            {
                var intent = ExtentIntetnts[extent];
                var formalConcept = new FormalConcept(extent, intent, FormalContext);
                formalConcepts.Add(formalConcept);
            }
            ExtentIntetnts.Clear();
            return formalConcepts;
        }
    }
}
