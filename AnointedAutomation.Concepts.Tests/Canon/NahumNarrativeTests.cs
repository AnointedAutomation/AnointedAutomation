// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Nahum narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class NahumNarrativeTests
    {
        [Fact]
        public void TheLordIsAvengingAndSlowToAngerYetGreatInPower_IsADivineAct()
        {
            // The LORD is a jealous and avenging God; the LORD is slow to anger and great in power, and will not acquit the guilty (Nahum 1:2-3).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD avenges, yet is slow to anger and great in power, and by no means clears the guilty",
                new JustRecompense(), new Righteousness(), new Patience()));
        }

        [Fact]
        public void TheLordRulesTheStormAndTheTremblingCreation_IsADivineAct()
        {
            // His way is in whirlwind and storm; He rebukes the sea, the mountains quake and the hills melt before Him (Nahum 1:3-6).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD marches in whirlwind and storm and the whole creation trembles in order before its Maker",
                new CreationOrder(), new Righteousness()));
        }

        [Fact]
        public void TheLordIsAGoodStrongholdToThoseWhoTrustHim_IsADivineAct()
        {
            // The LORD is good, a stronghold in the day of trouble; He knows those who take refuge in Him (Nahum 1:7).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD is good, a stronghold in the day of trouble, and He knows all who take refuge in Him",
                new Goodness(), new Protection(), new Trustworthiness()));
        }

        [Fact]
        public void TheLordMakesAFullEndOfHisEnemies_IsADivineAct()
        {
            // With an overflowing flood He will make a complete end of Nineveh, and pursue His enemies into darkness (Nahum 1:8-9).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD makes a complete end of His enemies and delivers His afflicted people from their grip",
                new JustRecompense(), new Deliverance(), new Righteousness()));
        }

        [Fact]
        public void TheWickedCounselorOfNinevehPlotsEvilAgainstTheLord_IsSin()
        {
            // From you, O Nineveh, came one who plotted evil against the LORD, a worthless counselor (Nahum 1:11).
            OracleHarness.Sin(OracleHarness.Witness("the worthless counselor of Nineveh devises evil schemes against the LORD",
                new WickedScheming(), new Rebellion(), new Pride()));
        }

        [Fact]
        public void TheLordBreaksTheYokeOffHisAfflictedPeople_IsADivineAct()
        {
            // Now I will break his yoke from off you and burst your bonds apart (Nahum 1:13).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD breaks the yoke of the oppressor from off His people and bursts their bonds apart",
                new Deliverance(), new Protection(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GoodNewsOfPeaceIsHeraldedOverTheMountains_IsRighteous()
        {
            // Behold, on the mountains the feet of him who brings good news, who publishes peace! Keep your feasts, O Judah (Nahum 1:15).
            OracleHarness.Righteous(OracleHarness.Witness("the herald upon the mountains publishes good news of peace and Judah keeps her feasts and vows to the LORD",
                new Peacemaking(), new Joy(), new Worship(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheLordIsAgainstNinevehAndBurnsHerChariots_IsADivineAct()
        {
            // Behold, I am against you, declares the LORD of hosts, and I will burn your chariots in smoke (Nahum 2:13).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD of hosts sets Himself against Nineveh and burns her chariots in smoke in just judgment",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void NinevehTheBloodyCityFullOfLiesAndPlunder_IsSin()
        {
            // Woe to the bloody city, all full of lies and plunder, no end to the prey! (Nahum 3:1).
            Resolution r = OracleHarness.Witness("Nineveh, the bloody city, is full of lies and plunder with no end to her prey",
                new Bloodshed(), new Deceit(), new Theft());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void NinevehTheHarlotEnslavesNationsBySorcery_IsSin()
        {
            // Because of the countless harlotries of the prostitute, who betrays nations with her whorings and peoples with her sorcery (Nahum 3:4).
            OracleHarness.Sin(OracleHarness.Witness("Nineveh the seductive harlot betrays nations by her whoredom and enslaves peoples by her sorcery",
                new SexualImmorality(), new Sorcery(), new Deceit()));
        }

        [Fact]
        public void TheLordIsAgainstTheHarlotCityAndExposesHerShame_IsADivineAct()
        {
            // Behold, I am against you, declares the LORD of hosts, and will lift up your skirts over your face (Nahum 3:5-7).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD of hosts judges the harlot city, uncovers her shame, and recompenses her evil rightly",
                new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void TheNationsRejoiceAtTheFallOfNinevehsEndlessCruelty_IsADivineAct()
        {
            // All who hear the news of you clap their hands, for upon whom has not come your unceasing evil? (Nahum 3:19).
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD brings Nineveh's wound past healing as the just recompense for her unceasing cruelty against all peoples",
                new JustRecompense(), new Righteousness(), new DefendingTheWeak()));
        }
    }
}
