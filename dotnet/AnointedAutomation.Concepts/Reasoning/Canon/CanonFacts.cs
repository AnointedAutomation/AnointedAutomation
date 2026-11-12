// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.Threading;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// The immutable, thread-safe index of canon facts the <see cref="CanonReasoner"/> reasons over.
    /// <see cref="Canon"/> is built once from <see cref="SinCatalog.All"/>, every public moral concept
    /// type (virtues, vices, canon concepts, mysteries) and the facets of God's revealed character.
    /// Every lookup is a hash lookup by (subject, relation) or by whole assertion; no query scans the index.
    /// </summary>
    public sealed class CanonFacts
    {
        /// <summary>The <see cref="ConceptRef.Kind"/> of a facet of God's character.</summary>
        public const string AttributeKind = "DivineAttribute";

        /// <summary>The <see cref="ConceptRef.Kind"/> of a <see cref="SinCategory"/>.</summary>
        public const string CategoryKind = "SinCategory";

        /// <summary>The <see cref="ConceptRef.Kind"/> of a coarse <see cref="Sin"/>.</summary>
        public const string CoarseSinKind = "CoarseSin";

        /// <summary>The <see cref="ConceptRef.Kind"/> of a Scripture reference.</summary>
        public const string ScriptureKind = "Scripture";

        /// <summary>The <see cref="ConceptRef.Kind"/> of God's whole character.</summary>
        public const string CharacterKind = "DivineCharacter";

        private static readonly Lazy<CanonFacts> CanonIndex =
            new Lazy<CanonFacts>(BuildCanon, LazyThreadSafetyMode.ExecutionAndPublication);

        private static readonly IReadOnlyList<CanonFact> NoFacts = Array.AsReadOnly(new CanonFact[0]);

        private readonly IReadOnlyList<CanonFact> facts;
        private readonly Dictionary<Assertion, CanonFact> byAssertion;
        private readonly Dictionary<(ConceptRef Subject, RelationKind Relation), IReadOnlyList<CanonFact>> bySubject;
        private readonly Dictionary<ConceptRef, MoralConcept> concepts;
        private readonly Dictionary<ConceptRef, DivineAttribute> attributes;

        /// <summary>
        /// Initializes a new index. Facts with the same assertion merge into one, keeping every distinct reference.
        /// </summary>
        /// <param name="facts">The facts, in a stable order.</param>
        /// <param name="concepts">The concepts the facts speak of, by reference.</param>
        /// <param name="attributes">The facets of God's character, by reference.</param>
        private CanonFacts(
            IEnumerable<CanonFact> facts,
            Dictionary<ConceptRef, MoralConcept> concepts,
            Dictionary<ConceptRef, DivineAttribute> attributes)
        {
            List<Assertion> order = new List<Assertion>();
            Dictionary<Assertion, List<ScriptureReference>> merged = new Dictionary<Assertion, List<ScriptureReference>>();
            foreach (CanonFact fact in facts)
            {
                ReasoningGuard.NotNull(fact, nameof(facts));
                if (!merged.TryGetValue(fact.Assertion, out List<ScriptureReference> references))
                {
                    references = new List<ScriptureReference>();
                    merged.Add(fact.Assertion, references);
                    order.Add(fact.Assertion);
                }

                foreach (ScriptureReference reference in fact.Scripture)
                {
                    if (!references.Contains(reference))
                    {
                        references.Add(reference);
                    }
                }
            }

            List<CanonFact> all = new List<CanonFact>(order.Count);
            byAssertion = new Dictionary<Assertion, CanonFact>(order.Count);
            Dictionary<(ConceptRef, RelationKind), List<CanonFact>> grouped = new Dictionary<(ConceptRef, RelationKind), List<CanonFact>>();
            foreach (Assertion assertion in order)
            {
                CanonFact fact = new CanonFact(assertion, merged[assertion]);
                all.Add(fact);
                byAssertion.Add(assertion, fact);
                (ConceptRef, RelationKind) key = (assertion.Subject, assertion.Relation);
                if (!grouped.TryGetValue(key, out List<CanonFact> list))
                {
                    list = new List<CanonFact>();
                    grouped.Add(key, list);
                }

                list.Add(fact);
            }

            bySubject = new Dictionary<(ConceptRef Subject, RelationKind Relation), IReadOnlyList<CanonFact>>(grouped.Count);
            foreach (KeyValuePair<(ConceptRef, RelationKind), List<CanonFact>> pair in grouped)
            {
                bySubject.Add(pair.Key, pair.Value.AsReadOnly());
            }

            this.facts = all.AsReadOnly();
            this.concepts = concepts;
            this.attributes = attributes;
        }

        /// <summary>
        /// Gets the canon index, built once on first use.
        /// </summary>
        public static CanonFacts Canon => CanonIndex.Value;

        /// <summary>
        /// Gets the reference to God's whole revealed character, the object of a witness query
        /// ("does this concept uphold God's character?").
        /// </summary>
        public static ConceptRef RevealedCharacter { get; } = new ConceptRef(CharacterKind, "Revealed");

        /// <summary>
        /// Gets every fact, in build order.
        /// </summary>
        public IReadOnlyList<CanonFact> All => facts;

        /// <summary>
        /// Gets the reference to a facet of God's character.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <returns>The reference.</returns>
        public static ConceptRef Attribute(DivineAttribute facet)
        {
            return new ConceptRef(AttributeKind, ReasoningGuard.NotNull(facet, nameof(facet)).Name);
        }

        /// <summary>
        /// Gets the reference to a sin category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The reference.</returns>
        public static ConceptRef Category(SinCategory category)
        {
            return new ConceptRef(CategoryKind, ReasoningGuard.Defined(category, nameof(category)).ToString());
        }

        /// <summary>
        /// Gets the reference to a coarse sin.
        /// </summary>
        /// <param name="sin">The coarse sin.</param>
        /// <returns>The reference.</returns>
        public static ConceptRef CoarseSin(Sin sin)
        {
            return new ConceptRef(CoarseSinKind, ReasoningGuard.Defined(sin, nameof(sin)).ToString());
        }

        /// <summary>
        /// Gets the facts about a subject under one relation (empty when there are none).
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="relation">The relation.</param>
        /// <returns>The facts, in build order.</returns>
        public IReadOnlyList<CanonFact> About(ConceptRef subject, RelationKind relation)
        {
            ReasoningGuard.NotNull(subject, nameof(subject));
            return bySubject.TryGetValue((subject, relation), out IReadOnlyList<CanonFact> found) ? found : NoFacts;
        }

        /// <summary>
        /// Finds the fact stating exactly this assertion.
        /// </summary>
        /// <param name="assertion">The assertion.</param>
        /// <param name="fact">The fact, when found.</param>
        /// <returns>True when the canon states the assertion.</returns>
        public bool TryGetFact(Assertion assertion, out CanonFact fact)
        {
            return byAssertion.TryGetValue(ReasoningGuard.NotNull(assertion, nameof(assertion)), out fact);
        }

        /// <summary>
        /// Finds the moral concept (or catalogued sin) a reference names.
        /// </summary>
        /// <param name="subject">The reference.</param>
        /// <param name="concept">The concept, when found.</param>
        /// <returns>True when the canon knows the concept.</returns>
        public bool TryGetConcept(ConceptRef subject, out MoralConcept concept)
        {
            return concepts.TryGetValue(ReasoningGuard.NotNull(subject, nameof(subject)), out concept);
        }

        /// <summary>
        /// Finds the facet of God's character a reference names.
        /// </summary>
        /// <param name="reference">The reference.</param>
        /// <param name="attribute">The facet, when found.</param>
        /// <returns>True when the reference names a facet.</returns>
        public bool TryGetAttribute(ConceptRef reference, out DivineAttribute attribute)
        {
            return attributes.TryGetValue(ReasoningGuard.NotNull(reference, nameof(reference)), out attribute);
        }

        /// <summary>
        /// Returns a new index holding these facts plus <paramref name="extra"/>. This index is unchanged.
        /// </summary>
        /// <param name="extra">The facts to add.</param>
        /// <returns>The extended index.</returns>
        public CanonFacts With(IEnumerable<CanonFact> extra)
        {
            ReasoningGuard.NotNull(extra, nameof(extra));
            List<CanonFact> combined = new List<CanonFact>(facts);
            combined.AddRange(extra);
            return new CanonFacts(combined, concepts, attributes);
        }

        /// <summary>
        /// Builds the canon index from the sin catalog, the moral concept types and God's revealed character.
        /// </summary>
        /// <returns>The index.</returns>
        private static CanonFacts BuildCanon()
        {
            IReadOnlyList<DivineAttribute> facets = Reality.Revealed().Character.Facets;
            Dictionary<ConceptRef, DivineAttribute> attributes = new Dictionary<ConceptRef, DivineAttribute>();
            foreach (DivineAttribute facet in facets)
            {
                attributes.Add(Attribute(facet), facet);
            }

            List<CanonFact> facts = new List<CanonFact>();
            Dictionary<ConceptRef, MoralConcept> concepts = new Dictionary<ConceptRef, MoralConcept>();
            foreach (MoralConcept concept in MoralConceptTypes())
            {
                ConceptRef subject = ConceptRef.Of(concept);
                concepts.Add(subject, concept);
                ScriptureReference[] grounds = { ScriptureReference.Parse(concept.Scripture) };
                AddGrounding(facts, subject, grounds);
                AddFacetFacts(facts, subject, concept, facets, grounds);
            }

            foreach (SinEntry entry in SinCatalog.All)
            {
                if (entry.IsOmitted)
                {
                    continue;
                }

                CataloguedSin sin = SinCatalog.Concept(entry.Id);
                ConceptRef subject = ConceptRef.Of(entry.Id);
                concepts.Add(subject, sin);
                ScriptureReference[] grounds = new List<ScriptureReference>(entry.References).ToArray();
                AddGrounding(facts, subject, grounds);
                facts.Add(new CanonFact(new Assertion(subject, RelationKind.InCategory, Category(entry.Category.Value)), grounds));
                if (entry.CoarseSin.HasValue)
                {
                    facts.Add(new CanonFact(new Assertion(subject, RelationKind.IsInstanceOf, CoarseSin(entry.CoarseSin.Value)), grounds));
                }

                facts.Add(new CanonFact(new Assertion(subject, RelationKind.IsInstanceOf, ConceptRef.Of(sin.GroundingVice)), grounds));
                AddFacetFacts(facts, subject, sin, facets, grounds);
            }

            return new CanonFacts(facts, concepts, attributes);
        }

        /// <summary>
        /// Adds one GroundedIn fact per reference, so a concept's Scripture can be looked up by subject.
        /// </summary>
        /// <param name="facts">The facts being built.</param>
        /// <param name="subject">The concept.</param>
        /// <param name="grounds">Its references.</param>
        private static void AddGrounding(List<CanonFact> facts, ConceptRef subject, ScriptureReference[] grounds)
        {
            foreach (ScriptureReference reference in grounds)
            {
                ConceptRef verse = new ConceptRef(ScriptureKind, reference.ToString());
                facts.Add(new CanonFact(new Assertion(subject, RelationKind.GroundedIn, verse), new[] { reference }));
            }
        }

        /// <summary>
        /// Adds the context-free facet facts of a concept: what it upholds (and so what God commands)
        /// and what it violates (and so what God forbids).
        /// </summary>
        /// <param name="facts">The facts being built.</param>
        /// <param name="subject">The concept's reference.</param>
        /// <param name="concept">The concept.</param>
        /// <param name="facets">God's facets.</param>
        /// <param name="grounds">The concept's references.</param>
        private static void AddFacetFacts(
            List<CanonFact> facts,
            ConceptRef subject,
            MoralConcept concept,
            IReadOnlyList<DivineAttribute> facets,
            ScriptureReference[] grounds)
        {
            foreach (DivineAttribute facet in facets)
            {
                ConceptRef attribute = Attribute(facet);
                if (concept.Upholds(facet))
                {
                    facts.Add(new CanonFact(new Assertion(subject, RelationKind.Upholds, attribute), grounds));
                    facts.Add(new CanonFact(new Assertion(attribute, RelationKind.Commands, subject), grounds));
                }

                if (concept.Violates(facet))
                {
                    facts.Add(new CanonFact(new Assertion(subject, RelationKind.Violates, attribute), grounds));
                    facts.Add(new CanonFact(new Assertion(attribute, RelationKind.Forbids, subject), grounds));
                }
            }
        }

        /// <summary>
        /// Creates one instance of every public, concrete moral concept type with a parameterless
        /// constructor (the catalogued sins are indexed from the catalog instead), ordered by type name
        /// so the index is the same on every run.
        /// </summary>
        /// <returns>The concepts.</returns>
        private static List<MoralConcept> MoralConceptTypes()
        {
            List<Type> types = new List<Type>();
            foreach (Type type in typeof(MoralConcept).Assembly.GetExportedTypes())
            {
                if (type.IsClass
                    && !type.IsAbstract
                    && typeof(MoralConcept).IsAssignableFrom(type)
                    && type != typeof(CataloguedSin)
                    && type.GetConstructor(Type.EmptyTypes) != null)
                {
                    types.Add(type);
                }
            }

            types.Sort((left, right) => string.CompareOrdinal(left.FullName, right.FullName));
            List<MoralConcept> concepts = new List<MoralConcept>(types.Count);
            foreach (Type type in types)
            {
                concepts.Add((MoralConcept)Activator.CreateInstance(type));
            }

            return concepts;
        }
    }
}
