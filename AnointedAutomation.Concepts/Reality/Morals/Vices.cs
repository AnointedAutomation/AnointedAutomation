// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    // Moral concepts that offend facets of God's character. Each declares its own Scripture, which
    // facets of God it violates, and how heinous it is (its Gravity, which drives the disorder it
    // unleashes). A concept that offends more than one facet reads as a graver wrong; a capital
    // concept unleashes the most destruction.

    /// <summary>Murder: the unjust taking of a life. "You shall not murder." (Exodus 20:13)</summary>
    public sealed class Murder : MoralConcept
    {
        public override string Name => "Murder";
        public override string Scripture => "Exodus 20:13";
        public override Gravity Gravity => Gravity.Capital;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is LoveFacet;
    }

    /// <summary>Defilement: the violation of a person, equated with murder. (Deuteronomy 22:25-27)</summary>
    public sealed class Defilement : MoralConcept
    {
        public override string Name => "Defilement";
        public override string Scripture => "Deuteronomy 22:25-27";
        public override Gravity Gravity => Gravity.Capital;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is LoveFacet;
    }

    /// <summary>Wronging a neighbour generally: harm done to another. "Love does no harm to a neighbor." (Romans 13:10)</summary>
    public sealed class Wrongdoing : MoralConcept
    {
        public override string Name => "Wrongdoing";
        public override string Scripture => "Romans 13:10";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is LoveFacet;
    }

    /// <summary>Theft: taking what is not due. "You shall not steal." (Exodus 20:15)</summary>
    public sealed class Theft : MoralConcept
    {
        public override string Name => "Theft";
        public override string Scripture => "Exodus 20:15";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Justice;

        // Context-sensitive: eating from a neighbour's field as you pass through, in real need, is a
        // provision of the Law, not theft (Deuteronomy 23:24-25; the disciples plucking grain, Matthew 12:1).
        public override bool Violates(DivineAttribute facet, Act act)
        {
            if (act.InContextOf(new Circumstance("direNeed")))
            {
                return false;
            }

            return Violates(facet);
        }
    }

    /// <summary>Withholding what is due: defrauding the worker of wages. (James 5:4)</summary>
    public sealed class WithholdingWhatIsDue : MoralConcept
    {
        public override string Name => "WithholdingWhatIsDue";
        public override string Scripture => "James 5:4";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Justice;
    }

    /// <summary>Bloodshed: shedding innocent blood. "Hands that shed innocent blood." (Proverbs 6:17)</summary>
    public sealed class Bloodshed : MoralConcept
    {
        public override string Name => "Bloodshed";
        public override string Scripture => "Proverbs 6:17";
        public override Gravity Gravity => Gravity.Capital;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is LoveFacet;
    }

    /// <summary>Oppression: crushing the weak and the poor. "Do not mistreat the widow or the fatherless." (Exodus 22:22)</summary>
    public sealed class Oppression : MoralConcept
    {
        public override string Name => "Oppression";
        public override string Scripture => "Exodus 22:22";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is LoveFacet;
    }

    /// <summary>Child sacrifice: the detestable practice of the nations. "Do not give your children to Molek." (Leviticus 18:21)</summary>
    public sealed class ChildSacrifice : MoralConcept
    {
        public override string Name => "ChildSacrifice";
        public override string Scripture => "Leviticus 18:21";
        public override Gravity Gravity => Gravity.Capital;
        public override bool Violates(DivineAttribute facet) =>
            facet is Justice || facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Self-seeking: "Love ... is not self-seeking." (1 Corinthians 13:5)</summary>
    public sealed class SelfSeeking : MoralConcept
    {
        public override string Name => "SelfSeeking";
        public override string Scripture => "1 Corinthians 13:5";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>A grudge: keeping a record of wrongs. "Love ... keeps no record of wrongs." (1 Corinthians 13:5)</summary>
    public sealed class Grudge : MoralConcept
    {
        public override string Name => "Grudge";
        public override string Scripture => "1 Corinthians 13:5";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Boasting: "Love ... does not boast." (1 Corinthians 13:4)</summary>
    public sealed class Boasting : MoralConcept
    {
        public override string Name => "Boasting";
        public override string Scripture => "1 Corinthians 13:4";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Envy: "Love does not envy." (1 Corinthians 13:4)</summary>
    public sealed class Envy : MoralConcept
    {
        public override string Name => "Envy";
        public override string Scripture => "1 Corinthians 13:4";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Rudeness: dishonoring others. "Love ... does not dishonor others." (1 Corinthians 13:5)</summary>
    public sealed class Rudeness : MoralConcept
    {
        public override string Name => "Rudeness";
        public override string Scripture => "1 Corinthians 13:5";
        public override Gravity Gravity => Gravity.Minor;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Delighting in evil: "Love does not delight in evil." (1 Corinthians 13:6)</summary>
    public sealed class DelightingInEvil : MoralConcept
    {
        public override string Name => "DelightingInEvil";
        public override string Scripture => "1 Corinthians 13:6";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Condemnation: crushing the broken and contrite. "A bruised reed he will not break." (Matthew 12:20)</summary>
    public sealed class Condemnation : MoralConcept
    {
        public override string Name => "Condemnation";
        public override string Scripture => "Matthew 12:20";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Mercy;
    }

    /// <summary>Indifference: withholding compassion from one in need. "If anyone ... has no pity." (1 John 3:17)</summary>
    public sealed class Indifference : MoralConcept
    {
        public override string Name => "Indifference";
        public override string Scripture => "1 John 3:17";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Mercy || facet is LoveFacet;
    }

    /// <summary>Unforgiveness: refusing the mercy one has received. (Matthew 18:35)</summary>
    public sealed class Unforgiveness : MoralConcept
    {
        public override string Name => "Unforgiveness";
        public override string Scripture => "Matthew 18:35";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Mercy;
    }

    /// <summary>Disobedience to God: doing what He forbade. "She took ... and ate." (Genesis 3:6)</summary>
    public sealed class Disobedience : MoralConcept
    {
        public override string Name => "Disobedience";
        public override string Scripture => "Genesis 3:6";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Rebellion against God. "Rebellion is like the sin of divination." (1 Samuel 15:23)</summary>
    public sealed class Rebellion : MoralConcept
    {
        public override string Name => "Rebellion";
        public override string Scripture => "1 Samuel 15:23";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Treachery: breaking faith, betraying a covenant. "Do not be unfaithful." (Malachi 2:10)</summary>
    public sealed class Treachery : MoralConcept
    {
        public override string Name => "Treachery";
        public override string Scripture => "Malachi 2:10";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Chaos: sowing disorder. "For God is not a God of disorder but of peace." (1 Corinthians 14:33)</summary>
    public sealed class Chaos : MoralConcept
    {
        public override string Name => "Chaos";
        public override string Scripture => "1 Corinthians 14:33";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Lawlessness: "Sin is lawlessness." (1 John 3:4)</summary>
    public sealed class Lawlessness : MoralConcept
    {
        public override string Name => "Lawlessness";
        public override string Scripture => "1 John 3:4";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }
}
