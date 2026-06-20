// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Psalms narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class PsalmsNarrativeTests
    {
        [Fact]
        public void TheBlessedManDelightsInTheLaw_IsRighteous()
        {
            // "Blessed is the man... his delight is in the law of the LORD." (Psalm 1:1-2)
            OracleHarness.Righteous(OracleHarness.Witness("the righteous one meditates on God's law day and night", new ObedienceToGod(), new Righteousness()));
        }

        [Fact]
        public void TheLordReignsAndSetsHisKing_IsADivineAct()
        {
            // "I have set my king on Zion, my holy hill." (Psalm 2:6) God establishes His ordained order.
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD enthrones His Anointed One", new CreationOrder(), new Righteousness()));
        }

        [Fact]
        public void TheLordIsTheShepherdWhoLeadsAndProvides_IsADivineAct()
        {
            // "The LORD is my shepherd; I shall not want." (Psalm 23) God guides, feeds, and protects.
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD shepherds His people", new Protection(), new Compassion(), new Goodness()));
        }

        [Fact]
        public void TheKingOfGloryAndCreatorOfTheEarth_IsADivineAct()
        {
            // "The earth is the LORD's, and the fulness thereof." (Psalm 24:1) God's ownership of creation's order.
            OracleHarness.DivineAct(OracleHarness.Witness("the King of Glory who founded the earth on the seas", new CreationOrder()));
        }

        [Fact]
        public void DavidsConfessionAfterBathsheba_IsRighteous()
        {
            // "Have mercy upon me, O God... wash me throughly from mine iniquity." (Psalm 51) The penitent turns back.
            OracleHarness.Righteous(OracleHarness.Witness("David confesses his sin and pleads for a clean heart", new Repentance(), new Humility(), new Purity()));
        }

        [Fact]
        public void DavidsAdulteryAndMurderOfUriah_IsSin()
        {
            // The very sin Psalm 51 laments: David's adultery with Bathsheba and killing of Uriah (Psalm 51 title; 2 Sam 11).
            Resolution r = OracleHarness.Witness("David takes Bathsheba and has Uriah killed", new Adultery(), new Murder(), new Deceit());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void GodForgivesTransgressionAndCoversSin_IsADivineAct()
        {
            // "Blessed is he whose transgression is forgiven, whose sin is covered." (Psalm 32:1)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD forgives and does not impute iniquity", new Forgiveness(), new Pardon(), new Compassion()));
        }

        [Fact]
        public void TheFoolSaysThereIsNoGodAndDoesCorruptly_IsSin()
        {
            // "The fool hath said in his heart, There is no God. They are corrupt..." (Psalm 14:1)
            OracleHarness.Sin(OracleHarness.Witness("the fool denies God and works corruption", new Blasphemy(), new Wrongdoing()));
        }

        [Fact]
        public void TheLordExecutesJusticeForTheOppressed_IsADivineAct()
        {
            // "The LORD executeth righteousness and judgment for all that are oppressed." (Psalm 103:6)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD vindicates the oppressed and defends the weak", new Righteousness(), new JustRecompense(), new DefendingTheWeak()));
        }

        [Fact]
        public void TheLordKeepsCovenantSteadfastLove_IsADivineAct()
        {
            // "For his mercy endureth for ever." (Psalm 136) God's enduring covenant faithfulness.
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD's steadfast love endures forever", new CovenantFaithfulness(), new Fidelity(), new Goodness()));
        }

        [Fact]
        public void TheLordLiftsTheNeedyFromTheDust_IsADivineAct()
        {
            // "He raiseth up the poor out of the dust... he giveth the barren woman a house." (Psalm 113:7-9)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD lifts the needy and seats them with princes", new Compassion(), new HonoringTheVulnerable(), new Deliverance()));
        }

        [Fact]
        public void TheWickedPlotAgainstTheRighteousWithDeceit_IsSin()
        {
            // "Under his tongue is mischief and vanity... he lieth in wait to murder the innocent." (Psalm 10:7-8)
            Resolution r = OracleHarness.Witness("the wicked lie in wait with deceit to slay the innocent", new WickedScheming(), new Deceit(), new Bloodshed(), new Oppression());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void ThePsalmistVowsWholeheartedPraise_IsRighteous()
        {
            // "I will praise thee, O LORD, with my whole heart." (Psalm 9:1; 150) The proper response of the creature.
            OracleHarness.Righteous(OracleHarness.Witness("the psalmist worships God with thanksgiving and joy", new Worship(), new Joy(), new Trust()));
        }

        [Fact]
        public void IdolsOfSilverAndGoldAreTrusted_IsSin()
        {
            // "Their idols are silver and gold... they that make them are like unto them." (Psalm 115:4-8) Grounding in an idol.
            OracleHarness.Sin(OracleHarness.Grounded("the nations trust in idols of silver and gold", Grounding.InIdol("idols of silver and gold"), new Idolatry()));
        }
    }
}
