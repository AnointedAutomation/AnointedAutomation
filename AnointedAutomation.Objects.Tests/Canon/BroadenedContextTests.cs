// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 5: context-sensitivity now reaches beyond Rahab's deception. The Law itself recontextualizes
    /// certain acts: taking food in dire need as you pass is provision, not theft (Deuteronomy
    /// 23:24-25; Matthew 12:1), and a work of mercy on the Sabbath does not break it (Matthew 12:11-12).
    /// The hard rule holds: context never excuses a capital crime (tested in HarmonizationAndContextTests).
    /// </summary>
    public class BroadenedContextTests
    {
        private static Circumstance[] Context(string name)
        {
            return new[] { new Circumstance(name) };
        }

        [Fact]
        public void TakingFoodInDireNeed_IsNotTheft()
        {
            // The disciples plucking grain on the Sabbath; eating from a field you pass (Deut 23:24-25).
            Resolution r = OracleHarness.WitnessInContext("plucking grain to eat in hunger",
                Context("direNeed"), new Theft());

            Assert.Equal(0.0, r.Disorder);   // the Law permits it; it is not the sin of theft
        }

        [Fact]
        public void TheSameTaking_WithoutDireNeed_IsStillTheft()
        {
            OracleHarness.Sin(OracleHarness.Witness("taking what is not yours", new Theft()));
        }

        [Fact]
        public void AWorkOfMercyOnTheSabbath_DoesNotBreakIt()
        {
            // "It is lawful to do good on the Sabbath." (Matthew 12:11-12)
            Resolution r = OracleHarness.WitnessInContext("healing on the Sabbath",
                Context("workOfMercy"), new SabbathBreaking(), new Healing());

            OracleHarness.Righteous(r);
        }

        [Fact]
        public void ProfaningTheSabbath_WithoutMercy_IsStillSin()
        {
            OracleHarness.Sin(OracleHarness.Witness("profaning the Sabbath", new SabbathBreaking()));
        }
    }
}
