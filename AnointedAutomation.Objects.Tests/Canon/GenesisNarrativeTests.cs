// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Genesis narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class GenesisNarrativeTests
    {
        [Fact]
        public void GodCreatesTheHeavensAndEarth_IsADivineAct()
        {
            // "In the beginning God created the heavens and the earth." (Genesis 1:1)
            OracleHarness.DivineAct(OracleHarness.Witness("God creates and orders the cosmos", new CreationOrder(), new Goodness()));
        }

        [Fact]
        public void GodRestsAndBlessesTheSeventhDay_IsADivineAct()
        {
            // "God blessed the seventh day and made it holy." (Genesis 2:3)
            OracleHarness.DivineAct(OracleHarness.Witness("God blesses and hallows the Sabbath", new CreationOrder(), new Peace()));
        }

        [Fact]
        public void TheSerpentDeceivesEve_IsSin()
        {
            // "The serpent deceived me, and I ate." (Genesis 3:13)
            OracleHarness.Sin(OracleHarness.Witness("the serpent deceives Eve", new Deceit(), new Rebellion()));
        }

        [Fact]
        public void CainMurdersAbel_IsCapitalSin()
        {
            // "Cain attacked his brother Abel and killed him." (Genesis 4:8)
            Resolution r = OracleHarness.Witness("Cain murders Abel", new Murder(), new Bloodshed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void EnochWalksWithGod_IsRighteous()
        {
            // "Enoch walked faithfully with God; then he was no more, because God took him away." (Genesis 5:24)
            OracleHarness.Righteous(OracleHarness.Witness("Enoch walks with God", new Fidelity(), new ObedienceToGod()));
        }

        [Fact]
        public void TheGenerationOfTheFlood_IsSin()
        {
            // "Every inclination of the thoughts of the human heart was only evil all the time." (Genesis 6:5)
            OracleHarness.Sin(OracleHarness.Witness("the wickedness that brings the flood", new Wrongdoing(), new Bloodshed()));
        }

        [Fact]
        public void NoahObeysAndBuildsTheArk_IsRighteous()
        {
            // "Noah did everything just as God commanded him." (Genesis 6:22; 7:5)
            OracleHarness.Righteous(OracleHarness.Witness("Noah obeys and builds the ark", new ObedienceToGod(), new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void GodMakesTheRainbowCovenant_IsADivineAct()
        {
            // "I have set my rainbow in the clouds... the covenant between me and the earth." (Genesis 9:13)
            OracleHarness.DivineAct(OracleHarness.Witness("God seals the covenant with Noah", new CovenantFaithfulness(), new PromiseFulfilled(), new Peacemaking()));
        }

        [Fact]
        public void BabelBuildsAgainstHeaven_IsSin()
        {
            // "Let us build... a tower that reaches to the heavens, so that we may make a name for ourselves." (Genesis 11:4)
            OracleHarness.Sin(OracleHarness.Witness("the tower of Babel", new Pride(), new Boasting(), new Rebellion()));
        }

        [Fact]
        public void GodCallsAbramAndPromisesBlessing_IsADivineAct()
        {
            // "I will make you into a great nation, and I will bless you... and all peoples on earth will be blessed through you." (Genesis 12:2-3)
            OracleHarness.DivineAct(OracleHarness.Witness("God calls and promises Abram", new PromiseFulfilled(), new Generosity(), new CovenantFaithfulness()));
        }

        [Fact]
        public void AbramBelievesTheLord_IsRighteous()
        {
            // "Abram believed the LORD, and he credited it to him as righteousness." (Genesis 15:6)
            OracleHarness.Righteous(OracleHarness.Witness("Abram trusts God's promise", new Trust(), new Fidelity(), new Righteousness()));
        }

        [Fact]
        public void AbrahamWelcomesTheThreeVisitors_IsRighteous()
        {
            // "He hurried... to meet them... 'Let me get you something to eat.'" (Genesis 18:2-5)
            OracleHarness.Righteous(OracleHarness.Witness("Abraham welcomes the three visitors at Mamre", new Hospitality(), new Kindness(), new FeedingTheHungry()));
        }

        [Fact]
        public void SodomsViolenceAgainstTheStrangers_IsCapitalSin()
        {
            // The men of Sodom demand to violate Lot's guests. (Genesis 19:4-9; Ezekiel 16:49)
            Resolution r = OracleHarness.Witness("Sodom's violence against Lot's guests", new Defilement(), new Oppression(), new SexualImmorality());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void AbrahamOffersIsaacInObedientFaith_IsRighteous()
        {
            // "By faith Abraham, when God tested him, offered Isaac." Abraham trusts God to provide; no child is harmed. (Genesis 22:8-12; Hebrews 11:17)
            OracleHarness.Righteous(OracleHarness.Witness("Abraham obeys at the binding of Isaac, trusting God to provide", new ObedienceToGod(), new CourageousFaith(), new Trust()));
        }

        [Fact]
        public void JacobDeceivesIsaacForTheBlessing_IsSin()
        {
            // "Your brother came deceitfully and took your blessing." (Genesis 27:35)
            OracleHarness.Sin(OracleHarness.Witness("Jacob deceives Isaac to steal Esau's blessing", new Deceit(), new Theft()));
        }

        [Fact]
        public void JosephRefusesPotipharsWife_IsRighteous()
        {
            // "How then could I do such a wicked thing and sin against God?" (Genesis 39:9)
            OracleHarness.Righteous(OracleHarness.Witness("Joseph refuses Potiphar's wife", new Purity(), new SelfControl(), new Fidelity()));
        }

        [Fact]
        public void JosephForgivesAndProvidesForHisBrothers_IsRighteous()
        {
            // "You intended to harm me, but God intended it for good... So then, don't be afraid. I will provide for you." (Genesis 50:20-21)
            OracleHarness.Righteous(OracleHarness.Witness("Joseph forgives his brothers and feeds them in the famine", new Forgiveness(), new Compassion(), new FeedingTheHungry()));
        }
    }
}
