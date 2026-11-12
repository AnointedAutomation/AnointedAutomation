// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// A stable, serializable reference to a concept the engines reason about: a kind plus an id
    /// within that kind. Two references are equal when kind and id match ordinally.
    /// </summary>
    public sealed record ConceptRef
    {
        /// <summary>
        /// The kind used for catalogued sins (<see cref="SinId"/>).
        /// </summary>
        public const string SinKind = "Sin";

        /// <summary>
        /// The kind used for every other <see cref="MoralConcept"/>.
        /// </summary>
        public const string MoralConceptKind = "MoralConcept";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptRef"/> record.
        /// </summary>
        /// <param name="kind">The concept kind, e.g. <see cref="SinKind"/>.</param>
        /// <param name="id">The id within that kind.</param>
        public ConceptRef(string kind, string id)
        {
            Kind = ReasoningGuard.NotBlank(kind, nameof(kind));
            Id = ReasoningGuard.NotBlank(id, nameof(id));
        }

        /// <summary>
        /// The concept kind.
        /// </summary>
        public string Kind { get; }

        /// <summary>
        /// The id within <see cref="Kind"/>.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// References a catalogued sin by its stable id.
        /// </summary>
        /// <param name="sin">The sin id.</param>
        /// <returns>A reference of kind <see cref="SinKind"/>.</returns>
        public static ConceptRef Of(SinId sin)
        {
            return new ConceptRef(SinKind, ReasoningGuard.Defined(sin, nameof(sin)).ToString());
        }

        /// <summary>
        /// References a moral concept. A <see cref="CataloguedSin"/> resolves to the same reference
        /// as <see cref="Of(SinId)"/> for its id, so both paths name one concept.
        /// </summary>
        /// <param name="concept">The moral concept.</param>
        /// <returns>The reference.</returns>
        public static ConceptRef Of(MoralConcept concept)
        {
            ReasoningGuard.NotNull(concept, nameof(concept));
            CataloguedSin sin = concept as CataloguedSin;
            if (sin != null)
            {
                return Of(sin.Entry.Id);
            }

            return new ConceptRef(MoralConceptKind, concept.Name);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Kind + ":" + Id;
        }
    }
}
