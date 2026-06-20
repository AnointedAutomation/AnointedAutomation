// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
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
