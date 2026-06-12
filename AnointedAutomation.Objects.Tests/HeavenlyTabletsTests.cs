// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests
{
    public class HeavenlyTabletsTests
    {
        [Fact]
        public void NewTablets_AreFullyOrdered()
        {
            // Before any deed is witnessed, reality stands fully ordered (1 Enoch 72-82: the courses
            // of creation keep their decreed law).
            HeavenlyTablets tablets = new HeavenlyTablets();

            Assert.Equal(1.0, tablets.Coherence());
        }

        [Fact]
        public void RecordingDisorder_DegradesCoherence()
        {
            // A deed that introduces disorder warps the order of reality (1 Enoch 80).
            HeavenlyTablets tablets = new HeavenlyTablets();

            tablets.Record(new Resolution(0.5, 0.5));

            Assert.Equal(0.5, tablets.Coherence());
        }

        [Fact]
        public void RecordingFaithfulDeeds_LeavesRealityOrdered()
        {
            // Deeds that fully cohere introduce no disorder, so the courses of reality keep their law.
            HeavenlyTablets tablets = new HeavenlyTablets();

            tablets.Record(new Resolution(1.0, 0.0));
            tablets.Record(new Resolution(1.0, 0.0));

            Assert.Equal(1.0, tablets.Coherence());
        }

        [Fact]
        public void Disorder_Accumulates_AcrossDeeds()
        {
            // As sinners multiply, disorder compounds and reality drifts further from its order
            // (1 Enoch 80).
            HeavenlyTablets tablets = new HeavenlyTablets();

            tablets.Record(new Resolution(0.5, 0.5));
            tablets.Record(new Resolution(0.5, 0.5));

            Assert.Equal(0.25, tablets.Coherence());
        }

        [Fact]
        public void History_ReturnsEveryDeedRecorded_InOrder()
        {
            // The tablets hold "the earlier and the later" (Jubilees): the whole record of what was.
            HeavenlyTablets tablets = new HeavenlyTablets();
            Resolution first = new Resolution(0.9, 0.1);
            Resolution second = new Resolution(0.2, 0.8);

            tablets.Record(first);
            tablets.Record(second);

            Assert.Equal(2, tablets.History().Count);
            Assert.Same(first, tablets.History()[0]);
            Assert.Same(second, tablets.History()[1]);
        }
    }
}
