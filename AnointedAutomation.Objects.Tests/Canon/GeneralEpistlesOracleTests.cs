// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 65-72: Hebrews, James, 1-2 Peter, 1-3 John, Jude.
    public class GeneralEpistlesOracleTests
    {
        [Fact]
        public void Hebrews_SustainingAllThingsByHisWord_IsADivineAct()
        {
            // "Sustaining all things by his powerful word." (Hebrews 1:3) The grounding.
            OracleHarness.DivineAct(OracleHarness.Witness("sustaining all things by His word", new CreationOrder()));
        }

        [Fact]
        public void Hebrews_TheOnceForAllSacrifice_IsADivineAct()
        {
            // "He sacrificed for their sins once for all when he offered himself." (Hebrews 7:27)
            OracleHarness.DivineAct(OracleHarness.Witness("the once-for-all sacrifice", new Atonement(), new SelfSacrifice()));
        }

        [Fact]
        public void James_PureReligionCaresForOrphansAndWidows_IsRighteous()
        {
            // "Look after orphans and widows in their distress." (James 1:27)
            OracleHarness.Righteous(OracleHarness.Witness("care for orphans and widows", new HonoringTheVulnerable(), new Generosity()));
        }

        [Fact]
        public void James_TheWithheldWagesCryOut_IsSin()
        {
            // "The wages you failed to pay ... are crying out against you." (James 5:4)
            OracleHarness.Sin(OracleHarness.Witness("the withheld wages cry out", new WithholdingWhatIsDue()));
        }

        [Fact]
        public void FirstPeter_HeBoreOurSinsInHisBody_IsADivineAct()
        {
            // "He himself bore our sins in his body on the cross." (1 Peter 2:24)
            OracleHarness.DivineAct(OracleHarness.Witness("He bore our sins", new Atonement(), new SelfSacrifice()));
        }

        [Fact]
        public void SecondPeter_NotWillingThatAnyShouldPerish_IsADivineAct()
        {
            // "Not wanting anyone to perish, but everyone to come to repentance." (2 Peter 3:9)
            OracleHarness.DivineAct(OracleHarness.Witness("God's patience toward all", new Patience(), new Compassion()));
        }

        [Fact]
        public void FirstJohn_GodIsLove_IsADivineAct()
        {
            // "God is love." (1 John 4:8); "we ought to lay down our lives." (3:16)
            OracleHarness.DivineAct(OracleHarness.Witness("God is love", new Compassion(), new SelfSacrifice()));
        }

        [Fact]
        public void FirstJohn_HavingNoPityOnABrotherInNeed_IsSin()
        {
            // "If anyone ... sees a brother in need but has no pity, how can the love of God be in
            // them?" (1 John 3:17)
            OracleHarness.Sin(OracleHarness.Witness("no pity on a brother in need", new Indifference()));
        }

        [Fact]
        public void SecondJohn_WalkingInLoveAndTruth_IsRighteous()
        {
            // "Love one another ... walking in the truth." (2 John 4-6)
            OracleHarness.Righteous(OracleHarness.Witness("walk in love and truth", new Compassion(), new RejoicingInTruth()));
        }

        [Fact]
        public void ThirdJohn_DiotrephesSelfSeeking_IsSin()
        {
            // Diotrephes "loves to be first" and refuses the brothers (3 John 9-10).
            OracleHarness.Sin(OracleHarness.Witness("Diotrephes loves to be first", new SelfSeeking(), new Pride()));
        }

        [Fact]
        public void Jude_KeepYourselvesInGodsLove_IsRighteous()
        {
            // "Keep yourselves in God's love ... be merciful to those who doubt." (Jude 21-22)
            OracleHarness.Righteous(OracleHarness.Witness("keep yourselves in God's love", new Compassion(), new Trust()));
        }
    }
}
