// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Ruth narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class RuthNarrativeTests
    {
        [Fact]
        public void RuthClingsToNaomi_IsRighteous()
        {
            // "Where you go I will go ... your people shall be my people, and your God my God." (Ruth 1:16-17)
            // Covenant-loyal love (hesed) of the Moabite widow to her bereaved mother-in-law.
            OracleHarness.Righteous(OracleHarness.Witness("Ruth refuses to leave Naomi",
                new CovenantFaithfulness(), new Compassion(), new SelfSacrifice(), new Fidelity()));
        }

        [Fact]
        public void RuthForsakesHerGodsToTrustTheLordOfIsrael_IsRighteous()
        {
            // "Your God shall be my God." (Ruth 1:16) Ruth turns from the idols of Moab to the Lord. (Ruth 2:12)
            // She comes "to take refuge under the wings" of the God of Israel.
            OracleHarness.Righteous(OracleHarness.Witness("Ruth takes refuge under the wings of Israel's God",
                new Worship(), new Trust(), new ObedienceToGod(), new CourageousFaith()));
        }

        [Fact]
        public void NaomiBlessesHerDaughtersInLaw_IsRighteous()
        {
            // "May the LORD deal kindly with you, as you have dealt with the dead and with me." (Ruth 1:8-9)
            // Naomi releases Orpah and Ruth back to their kin, blessing them with kindness.
            OracleHarness.Righteous(OracleHarness.Witness("Naomi blesses Orpah and Ruth",
                new Kindness(), new Generosity(), new Compassion()));
        }

        [Fact]
        public void RuthGleansToFeedNaomi_IsRighteous()
        {
            // Ruth goes to glean grain to feed herself and the widow Naomi. (Ruth 2:2,18)
            // Humble labor that feeds the hungry vulnerable.
            OracleHarness.Righteous(OracleHarness.Witness("Ruth gleans grain for Naomi",
                new SelfSacrifice(), new FeedingTheHungry(), new Humility(), new Endurance()));
        }

        [Fact]
        public void BoazShowsFavorAndProtectionToTheForeignWidow_IsRighteous()
        {
            // Boaz lets Ruth glean, gives her drink, orders his men not to touch her. (Ruth 2:8-9,14-16)
            // Kindness, protection, and provision to a defenseless foreign widow.
            OracleHarness.Righteous(OracleHarness.Witness("Boaz protects and provides for Ruth in his field",
                new Kindness(), new Protection(), new Generosity(),
                new GivingDrink(), new HonoringTheVulnerable(), new DefendingTheWeak()));
        }

        [Fact]
        public void BoazBlessesRuthInTheNameOfTheLord_IsRighteous()
        {
            // "The LORD repay you for what you have done ... under whose wings you have come to take refuge." (Ruth 2:12)
            // Boaz commends her covenant loyalty and blesses her in God's name.
            OracleHarness.Righteous(OracleHarness.Witness("Boaz blesses Ruth for her loyal love",
                new Kindness(), new RejoicingInTruth(), new Worship(), new Goodness()));
        }

        [Fact]
        public void RuthAppealsToBoazAsKinsmanRedeemerAtTheThreshingFloor_IsRighteous()
        {
            // "Spread your wings over your servant, for you are a redeemer." (Ruth 3:9)
            // A chaste, faithful appeal for covenant redemption, made in trust and humility.
            OracleHarness.Righteous(OracleHarness.Witness("Ruth asks Boaz to act as kinsman-redeemer",
                new Humility(), new Trust(), new Purity(), new CovenantFaithfulness()));
        }

        [Fact]
        public void BoazPraisesRuthsKindnessAndProtectsHerHonor_IsRighteous()
        {
            // "This last kindness is greater than the first ... all my people know you are a worthy woman." (Ruth 3:10-14)
            // Boaz guards her reputation and vows to do the kinsman's part.
            OracleHarness.Righteous(OracleHarness.Witness("Boaz honors Ruth and guards her at the threshing floor",
                new Kindness(), new Protection(), new Purity(), new SelfControl(), new Trustworthiness()));
        }

        [Fact]
        public void BoazRedeemsTheFieldAndTakesRuthAsWife_IsRighteous()
        {
            // At the gate Boaz redeems Elimelech's land and takes Ruth to wife, raising up the dead man's name. (Ruth 4:9-10)
            // Just, lawful covenant redemption of the widow and the inheritance.
            OracleHarness.Righteous(OracleHarness.Witness("Boaz redeems the inheritance and marries Ruth",
                new CovenantFaithfulness(), new JustRecompense(), new Righteousness(),
                new HonoringTheVulnerable(), new Fidelity()));
        }

        [Fact]
        public void TheLordGivesRuthConceptionAndAChild_IsADivineAct()
        {
            // "The LORD gave her conception, and she bore a son." (Ruth 4:13)
            // God's own act of restoring life and fruitfulness to the household.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD gives Ruth conception",
                new RestoringLife(), new CreationOrder(), new Goodness(), new Fidelity()));
        }

        [Fact]
        public void ObedRestoresLifeToNaomiAndContinuesThePromisedLine_IsADivineAct()
        {
            // "He shall be to you a restorer of life ... Obed ... the father of Jesse, the father of David." (Ruth 4:15-17)
            // God restores the widow Naomi and preserves the Davidic/Messianic line.
            OracleHarness.DivineAct(OracleHarness.Witness("God restores Naomi's life and keeps the line of David",
                new RestoringLife(), new CovenantFaithfulness(), new PromiseFulfilled(), new Deliverance()));
        }
    }
}
