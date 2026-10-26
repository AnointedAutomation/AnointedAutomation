// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11 Jesus is King ✝️
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 06: Joshua.
    public class JoshuaOracleTests
    {
        [Fact]
        public void GodKeepsThePromiseOfTheLand_IsADivineAct()
        {
            // "Not one of all the LORD's good promises ... failed." (Joshua 21:45)
            OracleHarness.DivineAct(OracleHarness.Witness("the promise of the land kept",
                new CovenantFaithfulness(), new PromiseFulfilled()));
        }

        [Fact]
        public void RahabHidesTheSpiesByFaith_IsRighteous()
        {
            // Rahab shelters the messengers, trusting Israel's God (Joshua 2; Hebrews 11:31).
            OracleHarness.Righteous(OracleHarness.Witness("Rahab hides the spies",
                new CourageousFaith(), new Protection()));
        }

        [Fact]
        public void AchansTreachery_IsSin()
        {
            // Achan takes what was devoted and hides it (Joshua 7).
            OracleHarness.Sin(OracleHarness.Witness("Achan's theft", new Theft(), new Disobedience()));
        }
    }
}
