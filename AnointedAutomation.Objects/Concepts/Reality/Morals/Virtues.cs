// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    // Moral concepts that embody facets of God's character. Each is first-class code, not a string:
    // it declares its own Scripture and which facets of God it upholds. Grouped here by kind for
    // navigation; each remains its own concept with its own logic.

    /// <summary>Compassion: mercy and love to the afflicted. "Be merciful, even as your Father is merciful." (Luke 6:36)</summary>
    public sealed class Compassion : MoralConcept
    {
        public override string Name => "Compassion";
        public override string Scripture => "Luke 6:36";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Protection: love that always protects. "Love ... always protects." (1 Corinthians 13:7)</summary>
    public sealed class Protection : MoralConcept
    {
        public override string Name => "Protection";
        public override string Scripture => "1 Corinthians 13:7";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Kindness: "Love is kind." (1 Corinthians 13:4)</summary>
    public sealed class Kindness : MoralConcept
    {
        public override string Name => "Kindness";
        public override string Scripture => "1 Corinthians 13:4";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Rejoicing in the truth: "Love ... rejoices with the truth." (1 Corinthians 13:6)</summary>
    public sealed class RejoicingInTruth : MoralConcept
    {
        public override string Name => "RejoicingInTruth";
        public override string Scripture => "1 Corinthians 13:6";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Patience: "Love is patient." (1 Corinthians 13:4)</summary>
    public sealed class Patience : MoralConcept
    {
        public override string Name => "Patience";
        public override string Scripture => "1 Corinthians 13:4";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Self-sacrifice: laying down one's life. "Greater love has no one than this." (John 15:13)</summary>
    public sealed class SelfSacrifice : MoralConcept
    {
        public override string Name => "SelfSacrifice";
        public override string Scripture => "John 15:13";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Forgiveness: "Forgive as the Lord forgave you." (Colossians 3:13)</summary>
    public sealed class Forgiveness : MoralConcept
    {
        public override string Name => "Forgiveness";
        public override string Scripture => "Colossians 3:13";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Healing the sick. "Jesus ... healing every disease and sickness." (Matthew 9:35)</summary>
    public sealed class Healing : MoralConcept
    {
        public override string Name => "Healing";
        public override string Scripture => "Matthew 9:35";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Hospitality: welcoming the stranger. "Love the foreigner." (Deuteronomy 10:19)</summary>
    public sealed class Hospitality : MoralConcept
    {
        public override string Name => "Hospitality";
        public override string Scripture => "Deuteronomy 10:19";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Honoring the vulnerable: the widow, the orphan, the child. (Exodus 22:22)</summary>
    public sealed class HonoringTheVulnerable : MoralConcept
    {
        public override string Name => "HonoringTheVulnerable";
        public override string Scripture => "Exodus 22:22";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Justice;
    }

    /// <summary>Teaching the truth. "The truth will set you free." (John 8:32)</summary>
    public sealed class TeachingTruth : MoralConcept
    {
        public override string Name => "TeachingTruth";
        public override string Scripture => "John 8:32";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet;
    }

    /// <summary>Restoring life: raising the dead, reversing death itself. "Lazarus, come out!" (John 11:43)</summary>
    public sealed class RestoringLife : MoralConcept
    {
        public override string Name => "RestoringLife";
        public override string Scripture => "John 11:43";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Atonement: paying the debt that justice requires. "A sacrifice of atonement." (Romans 3:25)</summary>
    public sealed class Atonement : MoralConcept
    {
        public override string Name => "Atonement";
        public override string Scripture => "Romans 3:25";
        public override bool Upholds(DivineAttribute facet) => facet is Justice;
    }

    /// <summary>Just recompense: rendering to each what is due. "Render to all what is due." (Romans 13:7)</summary>
    public sealed class JustRecompense : MoralConcept
    {
        public override string Name => "JustRecompense";
        public override string Scripture => "Romans 13:7";
        public override bool Upholds(DivineAttribute facet) => facet is Justice;
    }

    /// <summary>Deliverance: rescuing the oppressed. "I have come down to rescue them." (Exodus 3:8)</summary>
    public sealed class Deliverance : MoralConcept
    {
        public override string Name => "Deliverance";
        public override string Scripture => "Exodus 3:8";
        public override bool Upholds(DivineAttribute facet) => facet is Justice || facet is Mercy;
    }

    /// <summary>Defending the weak. "Defend the weak and the fatherless." (Psalm 82:3)</summary>
    public sealed class DefendingTheWeak : MoralConcept
    {
        public override string Name => "DefendingTheWeak";
        public override string Scripture => "Psalm 82:3";
        public override bool Upholds(DivineAttribute facet) => facet is Justice;
    }

    /// <summary>Righteousness: "The LORD is righteous, he loves justice." (Psalm 11:7)</summary>
    public sealed class Righteousness : MoralConcept
    {
        public override string Name => "Righteousness";
        public override string Scripture => "Psalm 11:7";
        public override bool Upholds(DivineAttribute facet) => facet is Justice;
    }

    /// <summary>Pardon: sparing the guilty. "While we were still sinners, Christ died for us." (Romans 5:8)</summary>
    public sealed class Pardon : MoralConcept
    {
        public override string Name => "Pardon";
        public override string Scripture => "Romans 5:8";
        public override bool Upholds(DivineAttribute facet) => facet is Mercy;
    }

    /// <summary>Covenant faithfulness: keeping one's promise. "Great is your faithfulness." (Lamentations 3:23)</summary>
    public sealed class CovenantFaithfulness : MoralConcept
    {
        public override string Name => "CovenantFaithfulness";
        public override string Scripture => "Lamentations 3:23";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Upholding the order of creation. "In him all things hold together." (Colossians 1:17)</summary>
    public sealed class CreationOrder : MoralConcept
    {
        public override string Name => "CreationOrder";
        public override string Scripture => "Colossians 1:17";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Obedience to God: alignment with His word. "If you fully obey the LORD." (Deuteronomy 28:1)</summary>
    public sealed class ObedienceToGod : MoralConcept
    {
        public override string Name => "ObedienceToGod";
        public override string Scripture => "Deuteronomy 28:1";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>The promise fulfilled: God keeping His word across history. "It must be fulfilled." (Luke 24:44)</summary>
    public sealed class PromiseFulfilled : MoralConcept
    {
        public override string Name => "PromiseFulfilled";
        public override string Scripture => "Luke 24:44";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Trustworthiness: a faithful keeper of confidence and covenant. (Proverbs 11:13)</summary>
    public sealed class Trustworthiness : MoralConcept
    {
        public override string Name => "Trustworthiness";
        public override string Scripture => "Proverbs 11:13";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }
}
