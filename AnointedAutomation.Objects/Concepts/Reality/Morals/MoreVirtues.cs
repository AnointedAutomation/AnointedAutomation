// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    // Further virtues the broader canon calls for, each first-class code with its own Scripture and
    // stance toward God's character.

    /// <summary>Humility: lowliness of heart before God and others. "Walk humbly with your God." (Micah 6:8)</summary>
    public sealed class Humility : MoralConcept
    {
        public override string Name => "Humility";
        public override string Scripture => "Micah 6:8";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Generosity: open-handed giving to the needy. "Whoever is kind to the poor lends to the LORD." (Proverbs 19:17)</summary>
    public sealed class Generosity : MoralConcept
    {
        public override string Name => "Generosity";
        public override string Scripture => "Proverbs 19:17";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Mercy;
    }

    /// <summary>Repentance: turning back from sin to God. "Return to me ... for he is gracious and compassionate." (Joel 2:13)</summary>
    public sealed class Repentance : MoralConcept
    {
        public override string Name => "Repentance";
        public override string Scripture => "Joel 2:13";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is Mercy;
    }

    /// <summary>Trust: faith in God's faithfulness. "The righteous shall live by his faith." (Habakkuk 2:4; Genesis 15:6)</summary>
    public sealed class Trust : MoralConcept
    {
        public override string Name => "Trust";
        public override string Scripture => "Habakkuk 2:4";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }

    /// <summary>Worship: right reverence and praise of God. "Worship the LORD in the splendor of his holiness." (Psalm 29:2)</summary>
    public sealed class Worship : MoralConcept
    {
        public override string Name => "Worship";
        public override string Scripture => "Psalm 29:2";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness || facet is LoveFacet;
    }

    /// <summary>Peacemaking: making peace between the estranged. "Blessed are the peacemakers." (Matthew 5:9)</summary>
    public sealed class Peacemaking : MoralConcept
    {
        public override string Name => "Peacemaking";
        public override string Scripture => "Matthew 5:9";
        public override bool Upholds(DivineAttribute facet) => facet is LoveFacet || facet is Faithfulness;
    }

    /// <summary>Courageous faithfulness: standing for God under threat. "Be strong and courageous." (Joshua 1:9; Daniel 3)</summary>
    public sealed class CourageousFaith : MoralConcept
    {
        public override string Name => "CourageousFaith";
        public override string Scripture => "Joshua 1:9";
        public override bool Upholds(DivineAttribute facet) => facet is Faithfulness;
    }
}
