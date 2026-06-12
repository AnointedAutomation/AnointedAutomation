// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 73: Revelation.
    public class RevelationOracleTests
    {
        [Fact]
        public void TheLambWhoWasSlain_IsADivineAct()
        {
            // "Worthy is the Lamb, who was slain." (Revelation 5:12)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lamb who was slain", new SelfSacrifice(), new Atonement()));
        }

        [Fact]
        public void TrueAndJustAreHisJudgments_IsADivineAct()
        {
            // "True and just are his judgments." (Revelation 19:2)
            OracleHarness.DivineAct(OracleHarness.Witness("true and just judgments", new Righteousness(), new JustRecompense()));
        }

        [Fact]
        public void EveryDeadJudgedFromTheBooks_IsADivineAct()
        {
            // "The dead were judged ... according to what they had done as recorded in the books."
            // (Revelation 20:12) The heavenly record, the engine's HeavenlyTablets.
            OracleHarness.DivineAct(OracleHarness.Witness("judged from the books", new JustRecompense()));
        }

        [Fact]
        public void TheNewCreation_OrderRestored_IsADivineAct()
        {
            // "I am making everything new ... no more death or mourning." (Revelation 21:4-5)
            OracleHarness.DivineAct(OracleHarness.Witness("the new creation", new CreationOrder(), new RestoringLife()));
        }
    }
}
