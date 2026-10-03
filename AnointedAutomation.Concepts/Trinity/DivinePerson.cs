// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// One Person of the Trinity. Each Person is distinct from the others by relation of origin alone
    /// (the Father unbegotten, the Son begotten, the Spirit proceeding), and each is wholly the one
    /// God: every Person holds the same single <see cref="Essence"/>, not a share or a copy of it.
    /// This guards against both errors at once. Against modalism, the Persons are three objects that
    /// exist together and are never equal to one another (the Son prays to the Father, the Father
    /// sends the Spirit, John 14:16). Against tritheism, there is only one essence instance among
    /// them ("the LORD our God, the LORD is one", Deuteronomy 6:4).
    /// </summary>
    public sealed class DivinePerson
    {
        internal DivinePerson(
            DivinePersonKind kind,
            string name,
            string origin,
            string appropriation,
            string scripture,
            DivineCharacter essence)
        {
            if (essence == null)
            {
                throw new System.ArgumentNullException(nameof(essence));
            }

            Kind = kind;
            Name = name;
            Origin = origin;
            Appropriation = appropriation;
            Scripture = scripture;
            Essence = essence;
        }

        /// <summary>Gets which Person this is.</summary>
        public DivinePersonKind Kind
        {
            get;
        }

        /// <summary>Gets the name of this Person.</summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets this Person's relation of origin, as confessed in the Nicene Creed. This is the only
        /// thing that distinguishes one Person from another.
        /// </summary>
        public string Origin
        {
            get;
        }

        /// <summary>
        /// Gets the part of the one undivided work that Scripture appropriates to this Person: the
        /// Father grounds, the Son conforms (all things made through the Word), the Spirit empowers
        /// and completes. Appropriation is not division; every Person is at work in every work.
        /// </summary>
        public string Appropriation
        {
            get;
        }

        /// <summary>Gets the Scripture this Person is confessed from.</summary>
        public string Scripture
        {
            get;
        }

        /// <summary>
        /// Gets the one divine essence this Person is. The same instance for all three Persons.
        /// </summary>
        public DivineCharacter Essence
        {
            get;
        }

        /// <summary>
        /// Gets the attributes of God this Person possesses: all of them, because they belong to the
        /// one essence, never to one Person apart from the others.
        /// </summary>
        public System.Collections.Generic.IReadOnlyList<DivineAttribute> Attributes
        {
            get
            {
                return Essence.Facets;
            }
        }

        /// <summary>
        /// Whether this Person and another are the one God, sharing the very same essence.
        /// </summary>
        /// <param name="other">Another Person.</param>
        /// <returns><c>true</c> when both are the same single essence.</returns>
        public bool IsSameEssenceAs(DivinePerson other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            return ReferenceEquals(Essence, other.Essence);
        }

        /// <summary>
        /// Whether this is the same Person as another. The Father is not the Son, and the Son is not
        /// the Spirit, even though each is wholly God.
        /// </summary>
        /// <param name="other">Another Person.</param>
        /// <returns><c>true</c> only for the same Person.</returns>
        public bool IsSamePersonAs(DivinePerson other)
        {
            if (other == null)
            {
                throw new System.ArgumentNullException(nameof(other));
            }

            return Kind == other.Kind && IsSameEssenceAs(other);
        }
    }
}
