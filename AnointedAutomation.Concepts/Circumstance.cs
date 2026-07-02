// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A state of the world that love responds to: hunger, enmity, grief, a friend in mortal peril.
    /// A circumstance carries no moral weight of its own (unlike a <see cref="MoralConcept"/>); it is
    /// simply the case. Well-known circumstances are their own types (<see cref="Hunger"/>,
    /// <see cref="Enmity"/>, ...), but because the world is open-ended, any circumstance can be named
    /// directly, including ones Scripture never described. Two circumstances are the same when they
    /// share a name.
    /// </summary>
    public class Circumstance : Concept
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Circumstance"/> class. Use a named subclass
        /// for a well-known circumstance, or this constructor directly for a novel one.
        /// </summary>
        /// <param name="name">The name of the circumstance (for example, "hungry").</param>
        public Circumstance(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new System.ArgumentNullException(nameof(name));
            }

            this.name = name;
        }

        private readonly string name;

        /// <inheritdoc />
        public override string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Two circumstances are equal when they share a name, so a novel circumstance and a named
        /// subclass for the same state match.
        /// </summary>
        /// <param name="obj">The object to compare with.</param>
        /// <returns><c>true</c> if the other is a circumstance with the same name.</returns>
        public override bool Equals(object obj)
        {
            Circumstance other = obj as Circumstance;
            return other != null && Name.Equals(other.Name);
        }

        /// <summary>
        /// A hash code consistent with name-based equality.
        /// </summary>
        /// <returns>The hash of the circumstance's name.</returns>
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
