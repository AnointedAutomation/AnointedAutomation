// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Oracle 01: Genesis. The engine must return the verdict Scripture gives at every step.
    public class GenesisOracleTests
    {
        [Fact]
        public void CreationIsVeryGood()
        {
            // "God saw all that he had made, and it was very good." (Genesis 1:31)
            OracleHarness.DivineAct(OracleHarness.Witness("creation", new CreationOrder()));
        }

        [Fact]
        public void TheFallIsSin()
        {
            // Eating what God forbade; death enters (Genesis 3:6; Romans 5:12).
            OracleHarness.Sin(OracleHarness.Witness("the fall", new Disobedience(), new Rebellion()));
        }

        [Fact]
        public void CainMurdersAbel()
        {
            // "Your brother's blood cries out to me from the ground." (Genesis 4:10)
            OracleHarness.Sin(OracleHarness.Witness("Cain kills Abel", new Murder()));
        }

        [Fact]
        public void TheEarthFilledWithViolence_IsSin()
        {
            // "The earth was filled with violence." (Genesis 6:11)
            OracleHarness.Sin(OracleHarness.Witness("the violence before the flood", new Bloodshed(), new Wrongdoing()));
        }

        [Fact]
        public void GodSavesNoah_IsFaithfulDeliverance()
        {
            // God preserves Noah and keeps covenant with creation (Genesis 6-9).
            OracleHarness.DivineAct(OracleHarness.Witness("God saves Noah", new Deliverance(), new CovenantFaithfulness()));
        }

        [Fact]
        public void AbrahamBelievesGod_IsRighteousness()
        {
            // "Abram believed the LORD, and he credited it to him as righteousness." (Genesis 15:6)
            OracleHarness.Righteous(OracleHarness.Witness("Abraham believes God", new Trust(), new ObedienceToGod()));
        }

        [Fact]
        public void SodomsCruelty_IsGraveSin()
        {
            // Inhospitality and attempted violation of the strangers (Genesis 19; Ezekiel 16:49).
            OracleHarness.Sin(OracleHarness.Witness("Sodom's cruelty", new Oppression(), new Defilement()));
        }

        [Fact]
        public void JosephForgivesHisBrothers_IsRighteous()
        {
            // "You intended to harm me, but God intended it for good." (Genesis 50:20)
            OracleHarness.Righteous(OracleHarness.Witness("Joseph forgives his brothers", new Forgiveness(), new Compassion()));
        }

        [Fact]
        public void TheBrothersSellJoseph_IsSin()
        {
            // Envy drives them to wrong their brother (Genesis 37).
            OracleHarness.Sin(OracleHarness.Witness("the brothers sell Joseph", new Wrongdoing(), new Envy()));
        }
    }
}
