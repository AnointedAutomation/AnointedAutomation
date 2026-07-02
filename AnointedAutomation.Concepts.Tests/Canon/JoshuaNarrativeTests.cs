// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Joshua narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JoshuaNarrativeTests
    {
        [Fact]
        public void GodCommissionsJoshuaToBeStrongAndCourageous_IsADivineAct()
        {
            // "Be strong and courageous... for the LORD your God will be with you wherever you go." (Joshua 1:5-9)
            OracleHarness.DivineAct(OracleHarness.Witness("God commissions Joshua and pledges to be with him always", new CovenantFaithfulness(), new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void RahabHidesTheSpiesAndProtectsThem_IsRighteous()
        {
            // "But the woman had taken the two men and hidden them." (Joshua 2:4-6)
            OracleHarness.Righteous(OracleHarness.Witness("Rahab hides the Israelite spies and shelters them from harm", new Protection(), new Hospitality(), new CourageousFaith()));
        }

        [Fact]
        public void GodStopsTheJordanSoIsraelCrossesOnDryGround_IsADivineAct()
        {
            // "The water from upstream stopped flowing... and the people crossed over." (Joshua 3:13-17)
            OracleHarness.DivineAct(OracleHarness.Witness("God halts the Jordan so Israel crosses into the land on dry ground", new Deliverance(), new PromiseFulfilled(), new CreationOrder()));
        }

        [Fact]
        public void IsraelSetsUpTwelveStonesAsAMemorial_IsRighteous()
        {
            // "These stones are to be a memorial to the people of Israel forever." (Joshua 4:5-7)
            OracleHarness.Righteous(OracleHarness.Witness("Israel raises twelve memorial stones to remember God's faithfulness", new Worship(), new CovenantFaithfulness(), new TeachingTruth()));
        }

        [Fact]
        public void GodGivesJerichoIntoIsraelsHand_IsADivineAct()
        {
            // "See, I have delivered Jericho into your hands." (Joshua 6:2, 20)
            OracleHarness.DivineAct(OracleHarness.Witness("God delivers Jericho into Israel's hand as the walls fall", new Deliverance(), new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void RahabAndHerHouseholdAreSparedAndDelivered_IsRighteous()
        {
            // "Joshua spared Rahab the prostitute, with her family... because she hid the men." (Joshua 6:25)
            OracleHarness.Righteous(OracleHarness.Witness("Rahab and her household are spared and brought to safety for her faith", new Deliverance(), new Protection(), new CovenantFaithfulness()));
        }

        [Fact]
        public void AchanStealsTheDevotedThingsAtJericho_IsSin()
        {
            // "Achan... took some of the devoted things; so the LORD's anger burned against Israel." (Joshua 7:1, 20-21)
            OracleHarness.Sin(OracleHarness.Witness("Achan steals the devoted spoil of Jericho and hides it", new Theft(), new Covetousness(), new Disobedience(), new Deceit()));
        }

        [Fact]
        public void JoshuaRenewsTheCovenantAndReadsTheLawAtMountEbal_IsRighteous()
        {
            // "Joshua read all the words of the law... There was not a word... that Joshua did not read." (Joshua 8:34-35)
            OracleHarness.Righteous(OracleHarness.Witness("Joshua renews the covenant and reads all the law to Israel at Mount Ebal", new ObedienceToGod(), new TeachingTruth(), new Worship(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheGibeonitesDeceiveIsraelIntoATreaty_IsSin()
        {
            // "They... resorted to a ruse... 'We have come from a distant country.'" (Joshua 9:3-6, 22)
            OracleHarness.Sin(OracleHarness.Witness("the Gibeonites deceive Israel with a lying ruse to secure a treaty", new Deceit(), new FalseWitness()));
        }

        [Fact]
        public void IsraelKeepsItsOathToTheGibeonites_IsRighteous()
        {
            // "We have given them our oath by the LORD... we cannot touch them now." (Joshua 9:18-20)
            OracleHarness.Righteous(OracleHarness.Witness("Israel honors its sworn oath and spares the Gibeonites despite the deception", new CovenantFaithfulness(), new Trustworthiness(), new Fidelity()));
        }

        [Fact]
        public void JoshuaDefendsTheGibeoniteAlliesFromTheFiveKings_IsRighteous()
        {
            // "Do not abandon your servants. Come up to us quickly and save us." (Joshua 10:6-7)
            OracleHarness.Righteous(OracleHarness.Witness("Joshua marches to rescue the Gibeonites from the five attacking kings", new Protection(), new DefendingTheWeak(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodMakesTheSunStandStillAtGibeon_IsADivineAct()
        {
            // "The sun stood still... until the nation avenged itself on its enemies." (Joshua 10:12-14)
            OracleHarness.DivineAct(OracleHarness.Witness("God halts the sun over Gibeon to give Israel victory", new Deliverance(), new JustRecompense(), new CreationOrder()));
        }

        [Fact]
        public void GodGivesIsraelRestInTheLandAsHePromised_IsADivineAct()
        {
            // "Not one of all the LORD's good promises... failed; every one was fulfilled." (Joshua 21:43-45)
            OracleHarness.DivineAct(OracleHarness.Witness("God gives Israel the land and rest, fulfilling every good promise", new PromiseFulfilled(), new CovenantFaithfulness(), new Fidelity(), new Peace()));
        }

        [Fact]
        public void CalebReceivesHebronForHisWholehearedFaith_IsRighteous()
        {
            // "Hebron has belonged to Caleb... ever since, because he followed the LORD... wholeheartedly." (Joshua 14:13-14)
            OracleHarness.Righteous(OracleHarness.Witness("Caleb receives Hebron as his just inheritance for following God wholeheartedly", new JustRecompense(), new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void JoshuaChargesIsraelToChooseAndServeTheLordAtShechem_IsRighteous()
        {
            // "Choose for yourselves this day whom you will serve... as for me and my household, we will serve the LORD." (Joshua 24:14-15)
            OracleHarness.Righteous(OracleHarness.Witness("Joshua charges Israel to put away idols and serve the LORD alone at Shechem", new Worship(), new ObedienceToGod(), new CovenantFaithfulness(), new TeachingTruth()));
        }
    }
}
