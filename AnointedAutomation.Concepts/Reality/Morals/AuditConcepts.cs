// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Concepts
{
    // Concepts the per-book agents asked for that were genuinely moral (not events or sacraments,
    // which are modelled as SacredMystery, and not synonyms of existing concepts). Added in the final
    // completeness audit so nothing the canon names is left unrepresented.

    /// <summary>Wisdom: skill in living by the fear of the LORD. "The fear of the LORD is the beginning of wisdom." (Proverbs 9:10)</summary>
    public sealed class Wisdom : MoralConcept
    {
        public override string Name => "Wisdom";
        public override string Scripture => "Proverbs 9:10";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Integrity: wholeness and uprightness of heart. "May integrity and uprightness protect me." (Psalm 25:21)</summary>
    public sealed class Integrity : MoralConcept
    {
        public override string Name => "Integrity";
        public override string Scripture => "Psalm 25:21";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is Justice;
    }

    /// <summary>Reverence: the fear of the LORD. "The fear of the LORD is the beginning of knowledge." (Proverbs 1:7)</summary>
    public sealed class Reverence : MoralConcept
    {
        public override string Name => "Reverence";
        public override string Scripture => "Proverbs 1:7";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Thankfulness: gratitude to God in all things. "Give thanks in all circumstances." (1 Thessalonians 5:18)</summary>
    public sealed class Thankfulness : MoralConcept
    {
        public override string Name => "Thankfulness";
        public override string Scripture => "1 Thessalonians 5:18";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Contentment: peace with what God provides. "Godliness with contentment is great gain." (1 Timothy 6:6)</summary>
    public sealed class Contentment : MoralConcept
    {
        public override string Name => "Contentment";
        public override string Scripture => "1 Timothy 6:6";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Diligence: faithful, hardworking stewardship. "Diligent hands will rule." (Proverbs 12:24)</summary>
    public sealed class Diligence : MoralConcept
    {
        public override string Name => "Diligence";
        public override string Scripture => "Proverbs 12:24";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Discernment: the trained sense to distinguish good from evil. (Hebrews 5:14)</summary>
    public sealed class Discernment : MoralConcept
    {
        public override string Name => "Discernment";
        public override string Scripture => "Hebrews 5:14";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Friendship: loyal love between companions. "A friend loves at all times." (Proverbs 17:17)</summary>
    public sealed class Friendship : MoralConcept
    {
        public override string Name => "Friendship";
        public override string Scripture => "Proverbs 17:17";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Grace: undeserved favor and kindness shown. "By grace you have been saved." (Ephesians 2:8)</summary>
    public sealed class Grace : MoralConcept
    {
        public override string Name => "Grace";
        public override string Scripture => "Ephesians 2:8";
        public override double Restoration => 0.75;
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Provision: meeting the needs of the dependent. "Your heavenly Father feeds them." (Matthew 6:26)</summary>
    public sealed class Provision : MoralConcept
    {
        public override string Name => "Provision";
        public override string Scripture => "Matthew 6:26";
        public override bool Upholds(DivineAttribute facet) => facet is Mercy || facet is LoveFacet;
    }

    /// <summary>Comfort: consoling the afflicted. "The God of all comfort." (2 Corinthians 1:3-4)</summary>
    public sealed class Comfort : MoralConcept
    {
        public override string Name => "Comfort";
        public override string Scripture => "2 Corinthians 1:3-4";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Cruelty: hardhearted infliction of suffering. "A cruel man brings trouble on himself." (Proverbs 11:17)</summary>
    public sealed class Cruelty : MoralConcept
    {
        public override string Name => "Cruelty";
        public override string Scripture => "Proverbs 11:17";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) =>
            facet is LoveFacet || facet is Mercy || facet is Justice;
    }

    /// <summary>Tyranny: oppressive, unjust rule over the powerless. "When the wicked rule, the people groan." (Proverbs 29:2)</summary>
    public sealed class Tyranny : MoralConcept
    {
        public override string Name => "Tyranny";
        public override string Scripture => "Proverbs 29:2";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Justice || facet is LoveFacet;
    }

    /// <summary>Plunder: violent seizing of others' goods. "Woe to you who have plundered." (Isaiah 33:1)</summary>
    public sealed class Plunder : MoralConcept
    {
        public override string Name => "Plunder";
        public override string Scripture => "Isaiah 33:1";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Justice;
    }

    /// <summary>Sacrilege: profaning what is holy to God. "You who abhor idols, do you rob temples?" (Romans 2:22; Daniel 5)</summary>
    public sealed class Sacrilege : MoralConcept
    {
        public override string Name => "Sacrilege";
        public override string Scripture => "Romans 2:22";
        public override Gravity Gravity => Gravity.Grave;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Hypocrisy: an outward form of godliness denying its power. "Inside you are full of hypocrisy." (Matthew 23:28)</summary>
    public sealed class Hypocrisy : MoralConcept
    {
        public override string Name => "Hypocrisy";
        public override string Scripture => "Matthew 23:28";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Simony: trafficking in the holy things of God for gain. "May your money perish with you!" (Acts 8:20)</summary>
    public sealed class Simony : MoralConcept
    {
        public override string Name => "Simony";
        public override string Scripture => "Acts 8:20";
        public override Gravity Gravity => Gravity.Serious;
        public override bool Violates(DivineAttribute facet) => facet is Faithfulness || facet is Justice;
    }
}
