// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 11:08:54
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 12:05:18
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// Sacrificial love: the greatest love of all. "Greater love has no one than this: to lay down
    /// one's life for one's friends." (John 15:13). It is <see cref="Agape"/> brought to its
    /// fullness — the love Christ showed at the cross — so it inherits the complete character of
    /// 1 Corinthians 13:4-8 and the whole agape repertoire.
    ///
    /// <para>
    /// Its behavior tree adds one branch above agape's: when a friend's life is at stake, it lays
    /// down its own life (John 15:13). For every other situation it falls through to agape's
    /// repertoire — so it still forgives, meets needs, and mourns as agape does.
    /// </para>
    /// </summary>
    public class SacrificialLove : Agape
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SacrificialLove"/> class conformed to the
        /// full Biblical standard of love.
        /// </summary>
        public SacrificialLove() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SacrificialLove"/> class directed from one
        /// party toward another, conformed to the full Biblical standard of love.
        /// </summary>
        /// <param name="lover">The one who loves.</param>
        /// <param name="beloved">The one (or thing) who is loved.</param>
        public SacrificialLove(string lover, string beloved) : base(lover, beloved)
        {
        }

        /// <summary>
        /// Builds sacrificial love's repertoire: lay down one's life when a friend's life is at stake
        /// (John 15:13), otherwise fall through to agape's full behavior tree.
        /// </summary>
        /// <returns>The root of sacrificial love's decision tree.</returns>
        protected override BehaviorNode BuildBehavior()
        {
            return new Selector(
                new Sequence(Condition.Fact("friendsLifeAtStake"), new Deed(LayDownLife())),
                base.BuildBehavior());
        }

        private static LoveAction LayDownLife()
        {
            return new LoveAction(
                true,
                "Lay down your life for your friends — hold nothing back, not even your own life.",
                "sacrificial; the greatest love",
                "John 15:13",
                "Love each other as I have loved you.");
        }
    }
}
