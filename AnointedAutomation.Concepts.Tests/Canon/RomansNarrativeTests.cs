// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Romans narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class RomansNarrativeTests
    {
        [Fact]
        public void TheGospelIsTheRighteousnessOfGodRevealed_IsADivineAct()
        {
            // "In the gospel the righteousness of God is revealed ... 'The righteous will live by faith.'" (Romans 1:16-17)
            OracleHarness.DivineAct(OracleHarness.Witness("the gospel reveals the righteousness of God", new Righteousness(), new TeachingTruth()));
        }

        [Fact]
        public void HumanityExchangesTheTruthOfGodForIdols_IsSin()
        {
            // "They exchanged the truth about God for a lie, and worshiped ... created things." (Romans 1:23-25)
            OracleHarness.Sin(OracleHarness.Grounded("they exchange the glory of God for idols", Grounding.InIdol("created things"), new Idolatry(), new Deceit()));
        }

        [Fact]
        public void TheDepravedMindGivenOverToEveryWickedness_IsSin()
        {
            // "Filled with every kind of wickedness ... envy, murder, strife, deceit." (Romans 1:29)
            OracleHarness.Sin(OracleHarness.Witness("the depraved mind full of wickedness", new Murder(), new Envy(), new Deceit()));
            // Capital vice (Murder) drives disorder to its maximum.
            Resolution r = OracleHarness.Witness("the depraved mind full of wickedness", new Murder(), new Envy(), new Deceit());
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void GodsKindnessLeadsToRepentance_IsADivineAct()
        {
            // "God's kindness is intended to lead you to repentance." (Romans 2:4)
            OracleHarness.DivineAct(OracleHarness.Witness("God's kindness leads sinners to repentance", new Kindness(), new Patience()));
        }

        [Fact]
        public void AllHaveSinnedAndFallShortOfGlory_IsSin()
        {
            // "There is no one righteous ... all have sinned and fall short of the glory of God." (Romans 3:10-23)
            OracleHarness.Sin(OracleHarness.Witness("all have sinned and fall short", new Wrongdoing(), new Disobedience()));
        }

        [Fact]
        public void TheAtonementJustAndJustifying_IsADivineAct()
        {
            // "Whom God put forward as a propitiation ... so as to be just and the justifier." (Romans 3:24-26)
            OracleHarness.DivineAct(OracleHarness.Witness("God presents Christ as the atoning sacrifice", new Atonement(), new Pardon(), new Righteousness()));
        }

        [Fact]
        public void AbrahamBelievesGodAndItIsCreditedAsRighteousness_IsRighteous()
        {
            // "Abraham believed God, and it was credited to him as righteousness." (Romans 4:3, 20-21)
            OracleHarness.Righteous(OracleHarness.Grounded("Abraham trusts the promise of God", Grounding.InGod(), new Trust(), new CourageousFaith(), new Righteousness()));
        }

        [Fact]
        public void WhileWeWereStillSinnersChristDiedForUs_IsADivineAct()
        {
            // "While we were still sinners, Christ died for us." (Romans 5:8)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ dies for the ungodly", new SelfSacrifice(), new Compassion(), new Atonement()));
        }

        [Fact]
        public void PeaceWithGodThroughOurLordJesusChrist_IsADivineAct()
        {
            // "We have peace with God through our Lord Jesus Christ." (Romans 5:1)
            OracleHarness.DivineAct(OracleHarness.Witness("God reconciles us, giving peace through Christ", new Peacemaking(), new Pardon()));
        }

        [Fact]
        public void SufferingProducesPerseveranceCharacterAndHope_IsRighteous()
        {
            // "Suffering produces perseverance; perseverance, character; and character, hope." (Romans 5:3-4)
            OracleHarness.Righteous(OracleHarness.Grounded("the saints endure suffering and so grow in hope", Grounding.InGod(), new Endurance(), new Patience()));
        }

        [Fact]
        public void ThereIsNoCondemnationForThoseInChrist_IsADivineAct()
        {
            // "There is now no condemnation for those who are in Christ Jesus." (Romans 8:1)
            OracleHarness.DivineAct(OracleHarness.Witness("God grants no condemnation to those in Christ", new Pardon(), new Deliverance()));
        }

        [Fact]
        public void NothingCanSeparateUsFromTheLoveOfGod_IsADivineAct()
        {
            // "Neither death nor life ... will be able to separate us from the love of God." (Romans 8:38-39)
            OracleHarness.DivineAct(OracleHarness.Witness("nothing separates us from the love of God", new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void OfferYourBodiesAsALivingSacrifice_IsRighteous()
        {
            // "Offer your bodies as a living sacrifice, holy and pleasing to God." (Romans 12:1)
            OracleHarness.Righteous(OracleHarness.Grounded("the believer offers himself as a living sacrifice", Grounding.InGod(), new SelfSacrifice(), new Worship()));
        }

        [Fact]
        public void DoNotRepayEvilButOvercomeEvilWithGood_IsRighteous()
        {
            // "Bless those who persecute you ... Do not be overcome by evil, but overcome evil with good." (Romans 12:14-21)
            OracleHarness.Righteous(OracleHarness.Witness("repay no one evil, but overcome evil with good", new Forgiveness(), new Goodness(), new Peacemaking()));
        }

        [Fact]
        public void LoveDoesNoHarmAndFulfillsTheLaw_IsRighteous()
        {
            // "Love does no harm to a neighbor. Therefore love is the fulfillment of the law." (Romans 13:8-10)
            OracleHarness.Righteous(OracleHarness.Witness("love your neighbor and so fulfill the law", new Kindness(), new Compassion()));
        }

        [Fact]
        public void WelcomeTheWeakInFaithWithoutPassingJudgment_IsRighteous()
        {
            // "Accept the one whose faith is weak, without quarreling over disputable matters." (Romans 14:1-3)
            OracleHarness.Righteous(OracleHarness.Witness("welcome the one weak in faith", new Hospitality(), new Gentleness()));
        }
    }
}
