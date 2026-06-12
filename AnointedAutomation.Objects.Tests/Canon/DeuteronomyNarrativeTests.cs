// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Deuteronomy narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class DeuteronomyNarrativeTests
    {
        [Fact]
        public void GodCommandsIsraelToPossessTheLandHePromised_IsDivineAct()
        {
            // "Behold, I have set the land before you: go in and possess the land which the LORD sware unto your fathers." (Deuteronomy 1:8)
            OracleHarness.DivineAct(OracleHarness.Witness("God gives Israel the promised land sworn to the fathers", new PromiseFulfilled(), new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void IsraelRebelsAndRefusesToEnterAtKadesh_IsSin()
        {
            // "Ye rebelled against the commandment of the LORD your God: and ye believed him not." (Deuteronomy 1:26, 32)
            OracleHarness.Sin(OracleHarness.Witness("Israel refuses to enter the land and distrusts the LORD", new Rebellion(), new Disobedience()));
        }

        [Fact]
        public void GodCaresForIsraelFortyYearsInTheWilderness_IsDivineAct()
        {
            // "These forty years the LORD thy God hath been with thee; thou hast lacked nothing." (Deuteronomy 2:7; 8:4)
            OracleHarness.DivineAct(OracleHarness.Witness("God provides for Israel forty years so they lack nothing", new Fidelity(), new Compassion(), new Protection()));
        }

        [Fact]
        public void GodGivesTheTenCommandmentsAtHoreb_IsDivineAct()
        {
            // "The LORD made a covenant with us in Horeb... and he wrote them in two tables of stone." (Deuteronomy 5:2-22)
            OracleHarness.DivineAct(OracleHarness.Witness("God speaks the Ten Commandments and his covenant at Horeb", new CovenantFaithfulness(), new TeachingTruth(), new Righteousness()));
        }

        [Fact]
        public void TheShemaCommandsWholeHeartedLoveAndWorshipOfGod_IsRighteous()
        {
            // "Hear, O Israel: The LORD our God is one LORD: and thou shalt love the LORD thy God with all thine heart." (Deuteronomy 6:4-5)
            OracleHarness.Righteous(OracleHarness.Witness("Israel is called to love and worship the one LORD with all the heart", new Worship(), new ObedienceToGod(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodChoosesIsraelInLoveNotForTheirGreatness_IsDivineAct()
        {
            // "The LORD did not set his love upon you... because ye were more in number... but because the LORD loved you." (Deuteronomy 7:7-9)
            OracleHarness.DivineAct(OracleHarness.Witness("God sets his love on Israel and keeps his oath to the fathers", new CovenantFaithfulness(), new Compassion(), new PromiseFulfilled()));
        }

        [Fact]
        public void IsraelIsCommandedToOpenTheirHandToThePoor_IsRighteous()
        {
            // "Thou shalt open thine hand wide unto thy brother, to thy poor, and to thy needy." (Deuteronomy 15:7-11)
            OracleHarness.Righteous(OracleHarness.Witness("Israel opens its hand wide to the poor and the needy", new Generosity(), new Compassion(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void IsraelMustDefendTheFatherlessWidowAndStranger_IsRighteous()
        {
            // "He doth execute the judgment of the fatherless and widow, and loveth the stranger... Love ye therefore the stranger." (Deuteronomy 10:18-19; 24:17)
            OracleHarness.Righteous(OracleHarness.Witness("Israel does justice for the fatherless, the widow, and the stranger", new DefendingTheWeak(), new HonoringTheVulnerable(), new Kindness()));
        }

        [Fact]
        public void PassingChildrenThroughTheFireToIdols_IsSin()
        {
            // "There shall not be found among you any one that maketh his son or his daughter to pass through the fire." (Deuteronomy 18:10; 12:31)
            Resolution r = OracleHarness.Grounded("the nations burn their sons and daughters to false gods", Grounding.InIdol("Molech"), new ChildSacrifice(), new Idolatry(), new Bloodshed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void ProvidingCitiesOfRefugeToSpareInnocentBlood_IsRighteous()
        {
            // "Thou shalt separate three cities... that innocent blood be not shed in thy land." (Deuteronomy 19:2-10)
            OracleHarness.Righteous(OracleHarness.Witness("Israel sets apart cities of refuge to protect the innocent", new Protection(), new Righteousness(), new JustRecompense()));
        }

        [Fact]
        public void RemovingLandmarksAndFalseWitnessAreForbidden_IsSin()
        {
            // "Thou shalt not remove thy neighbour's landmark... One witness shall not rise up... a false witness." (Deuteronomy 19:14-19)
            OracleHarness.Sin(OracleHarness.Witness("a man bears false witness and steals his neighbor's boundary", new FalseWitness(), new Theft(), new Deceit()));
        }

        [Fact]
        public void BlessingsAndCursesSetBeforeIsraelOnEbalAndGerizim_IsDivineAct()
        {
            // "I have set before you life and death, blessing and cursing: therefore choose life." (Deuteronomy 30:19; 11:26-29)
            OracleHarness.DivineAct(OracleHarness.Witness("God sets life and blessing before Israel and calls them to choose life", new TeachingTruth(), new CovenantFaithfulness(), new Goodness()));
        }

        [Fact]
        public void GodPromisesToCircumciseTheirHeartsToLoveHim_IsDivineAct()
        {
            // "The LORD thy God will circumcise thine heart... to love the LORD thy God with all thine heart." (Deuteronomy 30:6)
            OracleHarness.DivineAct(OracleHarness.Witness("God promises to circumcise Israel's heart so they may love him", new Healing(), new CovenantFaithfulness(), new Purity()));
        }

        [Fact]
        public void GodCommissionsJoshuaAndPromisesToGoBeforeIsrael_IsDivineAct()
        {
            // "Be strong and of a good courage... the LORD, he it is that doth go before thee; he will not fail thee." (Deuteronomy 31:6-8)
            OracleHarness.DivineAct(OracleHarness.Witness("God commissions Joshua and promises never to fail or forsake Israel", new Fidelity(), new Protection(), new PromiseFulfilled()));
        }
    }
}
