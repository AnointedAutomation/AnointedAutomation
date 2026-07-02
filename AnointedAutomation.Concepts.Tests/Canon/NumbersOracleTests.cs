// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 04: Numbers.
    public class NumbersOracleTests
    {
        [Fact]
        public void KorahsRebellion_IsSin()
        {
            // Korah and his company rise against the LORD's appointed order (Numbers 16).
            OracleHarness.Sin(OracleHarness.Witness("Korah's rebellion", new Rebellion(), new Pride()));
        }

        [Fact]
        public void TheWildernessGrumbling_IsSin()
        {
            // Refusing to trust God at the border of the land (Numbers 14).
            OracleHarness.Sin(OracleHarness.Witness("the wilderness rebellion", new Disobedience(), new Rebellion()));
        }

        [Fact]
        public void TheBronzeSerpentHealing_IsADivineAct()
        {
            // God provides healing for those who look in faith (Numbers 21:8-9; cf. John 3:14).
            OracleHarness.DivineAct(OracleHarness.Witness("the bronze serpent", new Healing(), new Compassion()));
        }

        [Fact]
        public void TheDaughtersOfZelophehad_InheritInJustice()
        {
            // No sons remained, so the inheritance passes to the daughters: a startling justice for
            // women in antiquity (Numbers 27:1-11).
            OracleHarness.Righteous(OracleHarness.Witness("the daughters of Zelophehad inherit",
                new JustRecompense(), new HonoringTheVulnerable()));
        }
    }
}
