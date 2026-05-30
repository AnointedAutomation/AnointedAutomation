// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-28 09:52:39
// Edited by Alexander Fields https://www.alexanderfields.me 2026-05-28 10:42:10
//Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// Models the abstract concept of Love as a concrete entity.
    ///
    /// Just as a person is conceptualized as being composed of matter and energy, this entity
    /// decomposes Love into its Biblical constituents. The "matter" of Love is the description
    /// given in 1 Corinthians 13:4-8 (the sixteen characteristics of what love is and is not),
    /// while its "energy" and direction flow from its <see cref="Source"/> — God Himself
    /// ("God is love." 1 John 4:8) — outward from a <see cref="Lover"/> toward a
    /// <see cref="Beloved"/> (Matthew 22:37-39). This is agape: the self-giving, sacrificial love
    /// supremely revealed at the cross (John 3:16; John 15:13).
    ///
    /// <para>
    /// Love is <c>abstract</c> because what love <em>does</em> differs in every situation. The
    /// character of love (the properties below) is shared by all love, but how a given love
    /// <see cref="Decide(Situation)"/>s in a concrete <see cref="Situation"/> is left to each
    /// concrete love, which supplies its own behavior tree via <see cref="BuildBehavior"/> (for
    /// example <see cref="Agape"/>, which meets the need before it as the Good Samaritan did,
    /// versus <see cref="SelfSeekingLove"/>, which passes by on the other side).
    /// </para>
    /// </summary>
    public abstract class Love
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Love"/> class. Virtues default to their
        /// unformed state; concrete loves (such as <see cref="Agape"/>) establish their own
        /// character.
        /// </summary>
        protected Love()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Love"/> class directed from one party
        /// toward another (Matthew 22:39, "Love your neighbor as yourself.").
        /// </summary>
        /// <param name="lover">The one who loves.</param>
        /// <param name="beloved">The one (or thing) who is loved.</param>
        protected Love(string lover, string beloved)
        {
            Lover = lover;
            Beloved = beloved;
        }

        /// <summary>
        /// Gets or sets the one (or thing) who is loved (Matthew 22:39).
        /// </summary>
        public string Beloved
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love boasts. Agape "does not boast" (1 Corinthians 13:4),
        /// so this is <c>false</c> in perfect love.
        /// </summary>
        public bool boastful
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love delights in evil. Agape "does not delight in evil"
        /// (1 Corinthians 13:6), so this is <c>false</c> in perfect love.
        /// </summary>
        public bool delightsInEvil
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love is easily angered. Agape "is not easily angered"
        /// (1 Corinthians 13:5), so this is <c>false</c> in perfect love.
        /// </summary>
        public bool easilyAngered
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love envies. Agape "does not envy" (1 Corinthians 13:4),
        /// so this is <c>false</c> in perfect love.
        /// </summary>
        public bool envious
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the two Great Commandments on which all the Law and the Prophets hang
        /// (Matthew 22:37-40).
        /// </summary>
        public string[] GreatestCommandment { get; set; } = new[]
        {
            "Love the Lord your God with all your heart and with all your soul and with all your mind.",
            "Love your neighbor as yourself."
        };

        /// <summary>
        /// Gets or sets whether this love always hopes (1 Corinthians 13:7), so this is
        /// <c>true</c> in perfect love.
        /// </summary>
        public bool hopeful
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love keeps a record of wrongs. Agape "keeps no record of
        /// wrongs" (1 Corinthians 13:5), so this is <c>false</c> in perfect love.
        /// </summary>
        public bool keepsRecordOfWrongs
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love is kind. "Love is kind" (1 Corinthians 13:4), so this
        /// is <c>true</c> in perfect love.
        /// </summary>
        public bool kind
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the one who loves (Matthew 22:37-39).
        /// </summary>
        public string Lover
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love never fails. "Love never fails" (1 Corinthians 13:8),
        /// so this is <c>true</c> in perfect love.
        /// </summary>
        public bool neverFails
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love is patient. "Love is patient" (1 Corinthians 13:4), so
        /// this is <c>true</c> in perfect love.
        /// </summary>
        public bool patient
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love always perseveres (1 Corinthians 13:7), so this is
        /// <c>true</c> in perfect love.
        /// </summary>
        public bool perseveres
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love always protects (1 Corinthians 13:7), so this is
        /// <c>true</c> in perfect love.
        /// </summary>
        public bool protects
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love is proud. Agape "is not proud" (1 Corinthians 13:4),
        /// so this is <c>false</c> in perfect love.
        /// </summary>
        public bool proud
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the Scripture reference describing this love.
        /// </summary>
        public string Reference { get; set; } = "1 Corinthians 13:4-8";

        /// <summary>
        /// Gets or sets whether this love rejoices with the truth. Agape "rejoices with the truth"
        /// (1 Corinthians 13:6), so this is <c>true</c> in perfect love.
        /// </summary>
        public bool rejoicesWithTruth
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love dishonors others (is rude). Agape "does not dishonor
        /// others" (1 Corinthians 13:5), so this is <c>false</c> in perfect love.
        /// </summary>
        public bool rude
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets whether this love is sacrificial — willing to lay down its life. "Greater
        /// love has no one than this: to lay down one's life for one's friends." (John 15:13; cf.
        /// John 3:16). This is <c>true</c> in perfect love.
        /// </summary>
        public bool sacrificial
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the full text of the love described in this entity (1 Corinthians 13:4-8).
        /// </summary>
        public string Scripture { get; set; } =
            "Love is patient, love is kind. It does not envy, it does not boast, it is not proud. " +
            "It does not dishonor others, it is not self-seeking, it is not easily angered, it keeps " +
            "no record of wrongs. Love does not delight in evil but rejoices with the truth. It always " +
            "protects, always trusts, always hopes, always perseveres. Love never fails.";

        /// <summary>
        /// Gets or sets whether this love is self-seeking. Agape "is not self-seeking"
        /// (1 Corinthians 13:5), so this is <c>false</c> in perfect love.
        /// </summary>
        public bool selfSeeking
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the source of this love. "We love because he first loved us." (1 John 4:19);
        /// "God is love." (1 John 4:8). Defaults to God.
        /// </summary>
        public string Source { get; set; } = "God";

        /// <summary>
        /// Gets or sets whether this love always trusts (1 Corinthians 13:7), so this is
        /// <c>true</c> in perfect love.
        /// </summary>
        public bool trusts
        {
            get; set;
        }

        private BehaviorNode behavior;

        /// <summary>
        /// Decides what this love does in a concrete <see cref="Situation"/> by walking its behavior
        /// tree (its built-in repertoire). Because love acts differently in every situation — and
        /// because different kinds of love act differently in the same situation (Luke 10:31-35) —
        /// each concrete love supplies its own tree via <see cref="BuildBehavior"/>.
        /// </summary>
        /// <param name="situation">The situation to respond to.</param>
        /// <returns>The <see cref="LoveAction"/> this love takes.</returns>
        public LoveAction Decide(Situation situation)
        {
            if (situation == null)
            {
                throw new System.ArgumentNullException(nameof(situation));
            }

            if (behavior == null)
            {
                behavior = BuildBehavior();
            }

            BehaviorResult result = behavior.Tick(situation);
            if (!result.succeeded || result.Action == null)
            {
                throw new System.InvalidOperationException(
                    "This love's behavior tree produced no action; a fallback deed is required.");
            }

            return result.Action;
        }

        /// <summary>
        /// Builds this love's behavior tree — its built-in repertoire of responses, in priority
        /// order (normally a <see cref="Selector"/> of <see cref="Sequence"/>s ending in a fallback
        /// <see cref="Deed"/>). Called once, lazily, on the first <see cref="Decide(Situation)"/>.
        /// </summary>
        /// <returns>The root <see cref="BehaviorNode"/> of this love's decision tree.</returns>
        protected abstract BehaviorNode BuildBehavior();

        /// <summary>
        /// Creates the perfect pattern of love — agape — exactly as described in
        /// 1 Corinthians 13:4-8: every virtue present, every vice absent, sacrificial, and never
        /// failing.
        /// </summary>
        /// <param name="lover">The one who loves.</param>
        /// <param name="beloved">The one (or thing) who is loved.</param>
        /// <returns>An <see cref="Agape"/> conformed to the full Biblical standard.</returns>
        public static Love Agape(string lover = null, string beloved = null)
        {
            return new Agape(lover, beloved);
        }

        /// <summary>
        /// Determines whether this love matches the complete Biblical pattern of agape: every
        /// virtue of 1 Corinthians 13:4-8 present, every vice absent, and sacrificial.
        /// </summary>
        /// <returns><c>true</c> if this love is perfect; otherwise <c>false</c>.</returns>
        public bool IsPerfect()
        {
            return patient
                && kind
                && rejoicesWithTruth
                && protects
                && trusts
                && hopeful
                && perseveres
                && neverFails
                && sacrificial
                && !envious
                && !boastful
                && !proud
                && !rude
                && !selfSeeking
                && !easilyAngered
                && !keepsRecordOfWrongs
                && !delightsInEvil;
        }

        /// <summary>
        /// Counts how many of the seventeen Biblical attributes this love embodies — a measure of
        /// how fully an expression of love is conformed to agape. Perfect love scores the maximum
        /// (<see cref="MaxCompleteness"/>); "if I have not love, I am nothing" (1 Corinthians 13:2).
        /// </summary>
        /// <returns>The number of attributes conformed to agape (0 through 17).</returns>
        public int Completeness()
        {
            int score = 0;
            if (patient) score++;
            if (kind) score++;
            if (rejoicesWithTruth) score++;
            if (protects) score++;
            if (trusts) score++;
            if (hopeful) score++;
            if (perseveres) score++;
            if (neverFails) score++;
            if (sacrificial) score++;
            if (!envious) score++;
            if (!boastful) score++;
            if (!proud) score++;
            if (!rude) score++;
            if (!selfSeeking) score++;
            if (!easilyAngered) score++;
            if (!keepsRecordOfWrongs) score++;
            if (!delightsInEvil) score++;
            return score;
        }

        /// <summary>
        /// The maximum value <see cref="Completeness"/> can return — the count of Biblical
        /// attributes that define perfect love.
        /// </summary>
        public const int MaxCompleteness = 17;

        /// <summary>
        /// Whether this love "bears all things" (1 Corinthians 13:7, KJV) — it always protects.
        /// </summary>
        /// <returns><c>true</c> if this love protects.</returns>
        public bool Bears()
        {
            return protects;
        }

        /// <summary>
        /// Whether this love "believes all things" (1 Corinthians 13:7, KJV) — it always trusts.
        /// </summary>
        /// <returns><c>true</c> if this love trusts.</returns>
        public bool Believes()
        {
            return trusts;
        }

        /// <summary>
        /// Whether this love "hopes all things" (1 Corinthians 13:7, KJV) — it always hopes.
        /// </summary>
        /// <returns><c>true</c> if this love is hopeful.</returns>
        public bool Hopes()
        {
            return hopeful;
        }

        /// <summary>
        /// Whether this love "endures all things" (1 Corinthians 13:7, KJV) — it always perseveres.
        /// </summary>
        /// <returns><c>true</c> if this love perseveres.</returns>
        public bool Endures()
        {
            return perseveres;
        }

        /// <summary>
        /// Whether this love embodies the greatest love of all: "Greater love has no one than this:
        /// to lay down one's life for one's friends." (John 15:13).
        /// </summary>
        /// <returns><c>true</c> if this love is sacrificial.</returns>
        public bool GreaterLove()
        {
            return sacrificial;
        }

        /// <summary>
        /// Whether this love abides. "And now these three remain: faith, hope and love. But the
        /// greatest of these is love." (1 Corinthians 13:13); love never fails (1 Corinthians 13:8).
        /// </summary>
        /// <returns><c>true</c> if this love endures and never fails.</returns>
        public bool Abides()
        {
            return neverFails;
        }

        /// <summary>
        /// Produces a human-readable description of this love, including its source, the parties
        /// involved (when known), and the Scripture that defines it.
        /// </summary>
        /// <returns>A description of this love.</returns>
        public string Describe()
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("Love (");
            builder.Append(Reference);
            builder.Append("), sourced in ");
            builder.Append(Source);
            builder.Append(". ");
            if (!string.IsNullOrEmpty(Lover) && !string.IsNullOrEmpty(Beloved))
            {
                builder.Append(Lover);
                builder.Append(" loves ");
                builder.Append(Beloved);
                builder.Append(". ");
            }
            builder.Append(Scripture);
            return builder.ToString();
        }
    }
}
