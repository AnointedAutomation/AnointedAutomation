// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 13: Jubilees (Ethiopic-unique). The heavenly tablets hold law, history, and judgment as
    // one record, the source of the engine's HeavenlyTablets substrate.
    public class JubileesOracleTests
    {
        [Fact]
        public void TheDecreedOrderOfCreation_IsGood()
        {
            // Creation and its seasons are ordained and written on the heavenly tablets (Jubilees 2).
            OracleHarness.DivineAct(OracleHarness.Witness("the decreed order of creation",
                new CreationOrder(), new CovenantFaithfulness()));
        }

        [Fact]
        public void KeepingTheAppointedCovenant_IsRighteous()
        {
            // The feasts and the covenant are ordained on the tablets; keeping them is righteous (Jub 6).
            OracleHarness.Righteous(OracleHarness.Witness("keeping the appointed covenant",
                new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void TransgressingTheDecreedOrder_IsSin()
        {
            // To transgress what is written and ordained is to rebel (Jubilees).
            OracleHarness.Sin(OracleHarness.Witness("transgressing the decree", new Disobedience()));
        }
    }
}
