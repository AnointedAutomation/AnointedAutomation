// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

namespace AnointedAutomation.Objects.Concepts
{
    /// <summary>
    /// A sacred mystery: a redemptive act of God or a sacrament of the Church, not a moral virtue or
    /// sin but a holy work in which the whole character of God is present. The engine previously
    /// could only model the moral character of an act; the per-book agents kept reaching for the
    /// Incarnation, the Resurrection, the Eucharist, and could only approximate them with adjacent
    /// virtues. A mystery is its own kind of concept: it embodies every facet of God's character at
    /// once, so it reads as a fully coherent divine act that destabilizes nothing.
    /// </summary>
    public abstract class SacredMystery : MoralConcept
    {
        /// <summary>A holy mystery embodies the whole character of God; it offends no facet.</summary>
        /// <param name="facet">A facet of God's character.</param>
        /// <returns>Always <c>true</c>: the mystery upholds every facet.</returns>
        public override bool Upholds(DivineAttribute facet)
        {
            return true;
        }
    }

    /// <summary>The Incarnation: "the Word became flesh and made his dwelling among us." (John 1:14)</summary>
    public sealed class Incarnation : SacredMystery
    {
        public override string Name => "Incarnation";
        public override string Scripture => "John 1:14";
    }

    /// <summary>The Resurrection: "he was raised on the third day." (1 Corinthians 15:4)</summary>
    public sealed class Resurrection : SacredMystery
    {
        public override string Name => "Resurrection";
        public override string Scripture => "1 Corinthians 15:4";
    }

    /// <summary>The Transfiguration: "his face shone like the sun." (Matthew 17:2)</summary>
    public sealed class Transfiguration : SacredMystery
    {
        public override string Name => "Transfiguration";
        public override string Scripture => "Matthew 17:2";
    }

    /// <summary>The Ascension: "he was taken up before their very eyes." (Acts 1:9)</summary>
    public sealed class Ascension : SacredMystery
    {
        public override string Name => "Ascension";
        public override string Scripture => "Acts 1:9";
    }

    /// <summary>Pentecost: "all of them were filled with the Holy Spirit." (Acts 2:4)</summary>
    public sealed class Pentecost : SacredMystery
    {
        public override string Name => "Pentecost";
        public override string Scripture => "Acts 2:4";
    }

    /// <summary>The Eucharist: "This is my body given for you; do this in remembrance of me." (Luke 22:19)</summary>
    public sealed class Eucharist : SacredMystery
    {
        public override string Name => "Eucharist";
        public override string Scripture => "Luke 22:19";
    }

    /// <summary>Baptism: "baptizing them in the name of the Father and of the Son and of the Holy Spirit." (Matthew 28:19)</summary>
    public sealed class Baptism : SacredMystery
    {
        public override string Name => "Baptism";
        public override string Scripture => "Matthew 28:19";
    }

    /// <summary>Ordination: the laying on of hands to set apart for ministry. (Acts 6:6; 1 Timothy 4:14)</summary>
    public sealed class Ordination : SacredMystery
    {
        public override string Name => "Ordination";
        public override string Scripture => "Acts 6:6";
    }

    /// <summary>Divine revelation: "God ... has spoken to us by his Son." (Hebrews 1:1-2)</summary>
    public sealed class DivineRevelation : SacredMystery
    {
        public override string Name => "DivineRevelation";
        public override string Scripture => "Hebrews 1:1-2";
    }

    /// <summary>The new creation: "I am making everything new." (Revelation 21:5)</summary>
    public sealed class NewCreation : SacredMystery
    {
        public override string Name => "NewCreation";
        public override string Scripture => "Revelation 21:5";
    }
}
