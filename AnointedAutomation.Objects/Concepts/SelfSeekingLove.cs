// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 10:42:10
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A self-seeking love — the priest and the Levite of the parable, who saw the wounded man and
    /// "passed by on the other side" (Luke 10:31-32). It faces the same situations as
    /// <see cref="Agape"/>, but because it is self-seeking (1 Corinthians 13:5) its repertoire holds
    /// only one response, whatever the situation: do nothing.
    ///
    /// <para>
    /// This concrete love exists to show why <see cref="Love"/> is abstract: the same
    /// <see cref="Situation"/> produces a different deed depending on the love that meets it.
    /// </para>
    /// </summary>
    public class SelfSeekingLove : Love
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelfSeekingLove"/> class.
        /// </summary>
        public SelfSeekingLove() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelfSeekingLove"/> class directed from one
        /// party toward another.
        /// </summary>
        /// <param name="lover">The one who loves.</param>
        /// <param name="beloved">The one (or thing) who is loved.</param>
        public SelfSeekingLove(string lover, string beloved) : base(lover, beloved)
        {
            selfSeeking = true;
        }

        /// <summary>
        /// Builds self-seeking love's repertoire: a single deed that applies to every situation —
        /// pass by on the other side (Luke 10:31-32).
        /// </summary>
        /// <returns>The root of self-seeking love's decision tree.</returns>
        protected override BehaviorNode BuildBehavior()
        {
            return new Deed(PassesBy());
        }

        private static LoveAction PassesBy()
        {
            return new LoveAction(
                false,
                "Passes by on the other side.",
                "self-seeking (1 Corinthians 13:5) — the absence of love",
                "Luke 10:31-32",
                null);
        }
    }
}
