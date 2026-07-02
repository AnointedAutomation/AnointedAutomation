// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 33-44: the Twelve (Hosea through Malachi).
    public class MinorProphetsOracleTests
    {
        [Fact]
        public void Hosea_IDesireMercyNotSacrifice_IsADivineAct()
        {
            // "I desire mercy, not sacrifice." (Hosea 6:6) God's redeeming love for the unfaithful.
            OracleHarness.DivineAct(OracleHarness.Witness("I desire mercy", new Compassion(), new CovenantFaithfulness()));
        }

        [Fact]
        public void Amos_TramplingThePoor_IsSin()
        {
            // "They trample on the heads of the poor." (Amos 2:7)
            OracleHarness.Sin(OracleHarness.Witness("trampling the poor", new Oppression(), new Greed()));
        }

        [Fact]
        public void Amos_LetJusticeRollOn_IsRighteous()
        {
            // "Let justice roll on like a river." (Amos 5:24)
            OracleHarness.Righteous(OracleHarness.Witness("let justice roll", new JustRecompense(), new DefendingTheWeak()));
        }

        [Fact]
        public void Micah_ActJustlyLoveMercyWalkHumbly_IsRighteous()
        {
            // "Act justly, love mercy, walk humbly with your God." (Micah 6:8) The whole of God's
            // character in one calling.
            OracleHarness.Righteous(OracleHarness.Witness("act justly, love mercy, walk humbly",
                new JustRecompense(), new Compassion(), new Humility()));
        }

        [Fact]
        public void Joel_ReturnToTheGraciousGod_IsRighteous()
        {
            // "Return to me ... for he is gracious and compassionate." (Joel 2:13)
            OracleHarness.Righteous(OracleHarness.Witness("return to the LORD", new Repentance(), new Humility()));
        }

        [Fact]
        public void Obadiah_GloatingOverABrothersFall_IsSin()
        {
            // Edom gloats and joins in the violence against Jacob (Obadiah 10-14).
            OracleHarness.Sin(OracleHarness.Witness("Edom gloats over Jacob", new DelightingInEvil(), new Wrongdoing()));
        }

        [Fact]
        public void Jonah_GodsMercyToTheRepentantEnemyCity_IsADivineAct()
        {
            // God spares Nineveh when it repents, to Jonah's displeasure (Jonah 3-4).
            OracleHarness.DivineAct(OracleHarness.Witness("God spares Nineveh", new Compassion(), new Pardon()));
        }

        [Fact]
        public void Nahum_NinevehsBloodshed_IsGraveSin()
        {
            // "Woe to the city of blood." (Nahum 3:1)
            Resolution r = OracleHarness.Witness("the city of blood", new Bloodshed(), new Oppression());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void Habakkuk_TheRighteousLiveByFaith_IsRighteous()
        {
            // "The righteous person will live by his faithfulness." (Habakkuk 2:4)
            OracleHarness.Righteous(OracleHarness.Witness("the righteous live by faith", new Trust()));
        }

        [Fact]
        public void Zephaniah_GodRejoicesOverHisPeople_IsADivineAct()
        {
            // "He will rejoice over you with singing." (Zephaniah 3:17)
            OracleHarness.DivineAct(OracleHarness.Witness("God rejoices over His people", new Compassion(), new CovenantFaithfulness()));
        }

        [Fact]
        public void Haggai_RebuildTheHousePutGodFirst_IsRighteous()
        {
            // "Give careful thought to your ways ... build the house." (Haggai 1:7-8)
            OracleHarness.Righteous(OracleHarness.Witness("rebuild the LORD's house", new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void Zechariah_TrueJusticeAndMercy_IsRighteous()
        {
            // "Administer true justice; show mercy and compassion to one another." (Zechariah 7:9)
            OracleHarness.Righteous(OracleHarness.Witness("true justice and mercy", new JustRecompense(), new Compassion()));
        }

        [Fact]
        public void Malachi_BreakingFaithWithTheWifeOfYouth_IsSin()
        {
            // "Do not break faith ... I hate divorce." (Malachi 2:14-16) Covenant treachery.
            OracleHarness.Sin(OracleHarness.Witness("breaking faith with the wife of youth", new Treachery(), new Adultery()));
        }
    }
}
