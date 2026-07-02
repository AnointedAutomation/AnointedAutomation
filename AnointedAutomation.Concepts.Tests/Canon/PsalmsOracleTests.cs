// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 23: Psalms.
    public class PsalmsOracleTests
    {
        [Fact]
        public void GodIsRefugeAndSteadfastLove_IsADivineAct()
        {
            // "The LORD is my refuge ... his love endures forever." (Psalm 18; 136)
            OracleHarness.DivineAct(OracleHarness.Witness("God my refuge", new Compassion(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheContriteHeart_IsRighteous()
        {
            // "Create in me a pure heart ... a broken and contrite heart you will not despise." (Psalm 51)
            OracleHarness.Righteous(OracleHarness.Witness("a contrite heart", new Repentance(), new Humility()));
        }

        [Fact]
        public void RighteousnessAndPeaceKiss_IsADivineAct()
        {
            // "Righteousness and peace kiss each other." (Psalm 85:10) Justice and mercy meet, as at
            // the cross.
            OracleHarness.DivineAct(OracleHarness.Witness("righteousness and peace meet",
                new Righteousness(), new Pardon()));
        }

        [Fact]
        public void TheWayOfTheWicked_IsSin()
        {
            // "Not so the wicked ... the way of the wicked leads to destruction." (Psalm 1)
            OracleHarness.Sin(OracleHarness.Witness("the way of the wicked", new Wrongdoing(), new Pride()));
        }
    }
}
