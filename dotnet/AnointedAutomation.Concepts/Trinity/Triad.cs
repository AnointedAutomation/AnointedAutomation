// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// The multiplicative rule shared by every triad in this model: a whole that needs three things
    /// is instantiated only when all three are present, and the strength of the whole is the product
    /// of its three factors, so any one factor at zero collapses the whole to zero. This is why love
    /// with no beloved is not love, and why a deed with no ground, no conformity to the Word, or no
    /// empowering Spirit does not cohere. "A cord of three strands is not quickly broken."
    /// (Ecclesiastes 4:12).
    /// </summary>
    public static class Triad
    {
        /// <summary>
        /// Whether all three members of a triad are present. A missing member means the triad is not
        /// instantiated at all; nothing is assumed in its place.
        /// </summary>
        /// <param name="first">The first member.</param>
        /// <param name="second">The second member.</param>
        /// <param name="third">The third member.</param>
        /// <returns><c>true</c> only when none of the three is <c>null</c>.</returns>
        public static bool Exists(object first, object second, object third)
        {
            return first != null && second != null && third != null;
        }

        /// <summary>
        /// The strength of a triad: the product of its three factors, each confined to 0.0 through
        /// 1.0. Any factor at 0.0 makes the whole 0.0; no factor can make up for another.
        /// </summary>
        /// <param name="first">The first factor.</param>
        /// <param name="second">The second factor.</param>
        /// <param name="third">The third factor.</param>
        /// <returns>The product, from 0.0 to 1.0.</returns>
        public static double Product(double first, double second, double third)
        {
            return Unit(first) * Unit(second) * Unit(third);
        }

        private static double Unit(double value)
        {
            if (value < 0.0)
            {
                return 0.0;
            }

            if (value > 1.0)
            {
                return 1.0;
            }

            return value;
        }
    }
}
