// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using AnointedAutomation.Enums;

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A catalogued sin as a first-class <see cref="MoralConcept"/>, so it can be part of an
    /// <see cref="Act"/> that <see cref="Reality.Witness(Act)"/> reads against God's character. It
    /// answers to the entry's own Scripture and takes its gravity and the facets it offends from the
    /// engine's existing vice it is grounded in: the vice for its coarse <see cref="Sin"/> when it has
    /// one (Murder, Theft, Deceit, ...), otherwise the vice for its <see cref="SinCategory"/>.
    /// Context-sensitive readings (for example Deceit to protect innocent life) carry through.
    /// </summary>
    public sealed class CataloguedSin : MoralConcept
    {
        /// <summary>
        /// Creates the concept for a catalogued, non-omitted entry.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <exception cref="ArgumentNullException">Thrown when the entry is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the entry is the omitted placeholder.</exception>
        public CataloguedSin(SinEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }

            if (entry.IsOmitted)
            {
                throw new ArgumentException("An omitted entry is not a sin to witness.", nameof(entry));
            }

            Entry = entry;
            GroundingVice = entry.CoarseSin.HasValue ? ViceFor(entry.CoarseSin.Value) : ViceFor(entry.Category.Value);
        }

        /// <summary>The catalog entry.</summary>
        public SinEntry Entry { get; }

        /// <summary>The engine's existing vice this sin is grounded in.</summary>
        public MoralConcept GroundingVice { get; }

        /// <summary>The stable id's name.</summary>
        public override string Name => Entry.Id.ToString();

        /// <summary>The entry's first Scripture reference; all of them are on <see cref="SinEntry.References"/>.</summary>
        public override string Scripture => Entry.References[0].ToString();

        /// <summary>The grounding vice's gravity.</summary>
        public override Gravity Gravity => GroundingVice.Gravity;

        /// <summary>
        /// Whether this sin offends a facet of God's character.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <returns>The grounding vice's answer.</returns>
        public override bool Violates(DivineAttribute facet) => GroundingVice.Violates(facet);

        /// <summary>
        /// Whether this sin offends a facet in the act's context.
        /// </summary>
        /// <param name="facet">The facet.</param>
        /// <param name="act">The act and its context.</param>
        /// <returns>The grounding vice's answer.</returns>
        public override bool Violates(DivineAttribute facet, Act act) => GroundingVice.Violates(facet, act);

        /// <summary>
        /// The engine's vice for a coarse moderation sin.
        /// </summary>
        /// <param name="sin">The coarse sin.</param>
        /// <returns>The vice.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for an undefined value.</exception>
        public static MoralConcept ViceFor(Sin sin)
        {
            switch (sin)
            {
                case Sin.Blasphemy: return new Blasphemy();
                case Sin.Idolatry: return new Idolatry();
                case Sin.Sorcery: return new Sorcery();
                case Sin.SexualImmorality: return new SexualImmorality();
                case Sin.Pride: return new Pride();
                case Sin.Greed: return new Greed();
                case Sin.Lust: return new Lust();
                case Sin.Wrath: return new FitsOfRage();
                case Sin.Envy: return new Envy();
                case Sin.Gluttony: return new Gluttony();
                case Sin.Sloth: return new Sloth();
                case Sin.Deceit: return new Deceit();
                case Sin.Theft: return new Theft();
                case Sin.Murder: return new Murder();
                default: throw new ArgumentOutOfRangeException(nameof(sin), sin, "No vice is defined for this Sin.");
            }
        }

        /// <summary>
        /// The engine's vice for a sin category.
        /// </summary>
        /// <param name="category">The category.</param>
        /// <returns>The vice.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for an undefined value.</exception>
        public static MoralConcept ViceFor(SinCategory category)
        {
            switch (category)
            {
                case SinCategory.AgainstGod: return new Disobedience();
                case SinCategory.FalseWorshipAndOccult: return new Idolatry();
                case SinCategory.ScriptureAndDoctrine: return new Disobedience();
                case SinCategory.Speech: return new Slander();
                case SinCategory.AgainstOthers: return new Heartlessness();
                case SinCategory.AngerAndViolence: return new Hatred();
                case SinCategory.Sexual: return new Impurity();
                case SinCategory.GreedAndMoney: return new Covetousness();
                case SinCategory.DishonestyAndTheft: return new Deceit();
                case SinCategory.Pride: return new Pride();
                case SinCategory.HeartAndMind: return new WickedScheming();
                case SinCategory.BodyAndExcess: return new Debauchery();
                case SinCategory.FamilyAndMarriage: return new DishonoringParents();
                case SinCategory.AuthorityAndWork: return new Rebellion();
                case SinCategory.Omission: return new Indifference();
                case SinCategory.Worldliness: return new SelfSeeking();
                case SinCategory.Leadership: return new Hypocrisy();
                case SinCategory.AgainstBelievers: return new Malice();
                default: throw new ArgumentOutOfRangeException(nameof(category), category, "No vice is defined for this category.");
            }
        }
    }
}
