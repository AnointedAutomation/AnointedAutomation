// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    // Further sins the broader canon names, each with its Scripture, the facets it offends, and its
    // gravity.

    /// <summary>Idolatry: worship turned from God to dead idols. "You shall have no other gods before me." (Exodus 20:3)</summary>
    public sealed class Idolatry : MoralConcept
    {
        public override string Name => "Idolatry";
        public override string Scripture => "Exodus 20:3";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Adultery: betrayal of the marriage covenant. "You shall not commit adultery." (Exodus 20:14)</summary>
    public sealed class Adultery : MoralConcept
    {
        public override string Name => "Adultery";
        public override string Scripture => "Exodus 20:14";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) =>
            facet is LoveFacet || facet is Faithfulness || facet is Justice;
    }

    /// <summary>Pride: self-exaltation against God and neighbour. "Pride goes before destruction." (Proverbs 16:18)</summary>
    public sealed class Pride : MoralConcept
    {
        public override string Name => "Pride";
        public override string Scripture => "Proverbs 16:18";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Deceit: lying and false witness. "The LORD detests lying lips." (Proverbs 12:22; Exodus 20:16)</summary>
    public sealed class Deceit : MoralConcept
    {
        public override string Name => "Deceit";
        public override string Scripture => "Proverbs 12:22";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;

        // Context-sensitive: a deception to shield the innocent from those who would murder them is
        // not the lie God condemns. Rahab hid the spies (Joshua 2; commended in Hebrews 11:31), and
        // the Hebrew midwives deceived Pharaoh and God dealt well with them (Exodus 1:19-21).
        public override bool Violates(DivineAttribute facet, Act act)
        {
            if (act.InContextOf(new Circumstance("protectingInnocentLife")))
            {
                return false;
            }

            return Violates(facet);
        }
    }

    /// <summary>Blasphemy: reviling the holy name of God. "Anyone who blasphemes the name of the LORD." (Leviticus 24:16)</summary>
    public sealed class Blasphemy : MoralConcept
    {
        public override string Name => "Blasphemy";
        public override string Scripture => "Leviticus 24:16";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Greed: the love of money that grasps at the expense of others. (1 Timothy 6:10)</summary>
    public sealed class Greed : MoralConcept
    {
        public override string Name => "Greed";
        public override string Scripture => "1 Timothy 6:10";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is LoveFacet || facet is Justice;
    }
}
