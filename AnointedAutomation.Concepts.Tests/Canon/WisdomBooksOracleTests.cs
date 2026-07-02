// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 25, 26, 27, 28: Tegsats (Reproof), Metsihafe Tibeb (Wisdom of Solomon), Ecclesiastes,
    // Song of Songs.
    public class WisdomBooksOracleTests
    {
        [Fact]
        public void ReceivingReproofInHumility_IsRighteous()
        {
            // The book of Reproof: "whoever heeds correction gains understanding." (Proverbs 15:32)
            OracleHarness.Righteous(OracleHarness.Witness("heeding reproof", new Repentance(), new Humility()));
        }

        [Fact]
        public void TheUngodlyOppressTheRighteous_IsSin()
        {
            // "Let us oppress the righteous poor man." (Wisdom of Solomon 2:10)
            OracleHarness.Sin(OracleHarness.Witness("the ungodly oppress the righteous", new Oppression(), new Wrongdoing()));
        }

        [Fact]
        public void TheSoulsOfTheRighteousAreInGodsHand_IsADivineAct()
        {
            // "The souls of the righteous are in the hand of God." (Wisdom of Solomon 3:1)
            OracleHarness.DivineAct(OracleHarness.Witness("the righteous in God's hand", new Righteousness(), new Compassion()));
        }

        [Fact]
        public void FearGodAndKeepHisCommandments_IsRighteous()
        {
            // "Fear God and keep his commandments ... God will bring every deed into judgment." (Ecc 12:13-14)
            OracleHarness.Righteous(OracleHarness.Witness("fear God and keep His commandments",
                new ObedienceToGod(), new Humility()));
        }

        [Fact]
        public void CovenantLoveOfTheBride_IsRighteous()
        {
            // "Love is as strong as death." (Song of Songs 8:6) Faithful covenant love.
            OracleHarness.Righteous(OracleHarness.Witness("the love of the bride and bridegroom",
                new CovenantFaithfulness(), new Compassion()));
        }
    }
}
