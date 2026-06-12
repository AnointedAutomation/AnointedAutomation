// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Revelation narrative oracle (Eastern-Orthodox-accepted): the engine must return the verdict Scripture gives at each step.
    public class RevelationNarrativeTests
    {
        [Fact]
        public void ChristTheFaithfulWitnessLovesUsAndFreesUsByHisBlood_IsADivineAct()
        {
            // Jesus Christ, the faithful witness, loved us and freed us from our sins by His blood (Revelation 1:5).
            OracleHarness.DivineAct(OracleHarness.Grounded("Christ the faithful witness loves us and frees us from sin by His blood",
                Grounding.InGod(), new Fidelity(), new SelfSacrifice(), new Atonement(), new Deliverance()));
        }

        [Fact]
        public void ChristCommendsEphesusForEnduringPatiently_IsRighteous()
        {
            // To Ephesus: you have endurance and have borne up for My name's sake and have not grown weary (Revelation 2:2-3).
            OracleHarness.Righteous(OracleHarness.Witness("the church at Ephesus labors and endures patiently for Christ's name",
                new Endurance(), new Patience(), new Fidelity()));
        }

        [Fact]
        public void TheEnticementToEatFoodSacrificedToIdolsAndPracticeImmorality_IsSin()
        {
            // The teaching of Balaam and Jezebel: idolatry and sexual immorality among the churches (Revelation 2:14, 20).
            OracleHarness.Sin(OracleHarness.Grounded("Jezebel teaches the servants to commit sexual immorality and eat food sacrificed to idols",
                Grounding.InIdol("Jezebel"), new Idolatry(), new SexualImmorality(), new Deceit()));
        }

        [Fact]
        public void ChristCounselsLukewarmLaodiceaToRepent_IsRighteous()
        {
            // Christ rebukes and disciplines those He loves; be earnest therefore and repent (Revelation 3:19).
            OracleHarness.Righteous(OracleHarness.Witness("Laodicea is called to be earnest and repent of lukewarmness",
                new Repentance(), new Humility(), new HungerForRighteousness()));
        }

        [Fact]
        public void TheTwentyFourEldersWorshipHimWhoSitsOnTheThrone_IsRighteous()
        {
            // The elders fall down before the throne and cast their crowns, worshiping Him who lives forever (Revelation 4:10-11).
            OracleHarness.Righteous(OracleHarness.Witness("the twenty-four elders cast down their crowns and worship the Creator",
                new Worship(), new Humility(), new RejoicingInTruth()));
        }

        [Fact]
        public void TheSlainLambIsWorthyToOpenTheScroll_IsADivineAct()
        {
            // Worthy is the Lamb who was slain, for by His blood He ransomed people for God (Revelation 5:9-12).
            OracleHarness.DivineAct(OracleHarness.Grounded("the Lamb who was slain ransoms people for God by His blood",
                Grounding.InGod(), new SelfSacrifice(), new Atonement(), new Deliverance(), new Worship()));
        }

        [Fact]
        public void TheLambShepherdsTheRedeemedAndGodWipesAwayEveryTear_IsADivineAct()
        {
            // The Lamb guides them to springs of living water, and God wipes away every tear from their eyes (Revelation 7:16-17).
            OracleHarness.DivineAct(OracleHarness.Grounded("the Lamb shepherds the redeemed and God wipes away every tear",
                Grounding.InGod(), new Compassion(), new Protection(), new Healing(), new GivingDrink()));
        }

        [Fact]
        public void MankindRefusesToRepentOfMurdersAndSorceriesAndThefts_IsSin()
        {
            // The rest of mankind did not repent of their murders, sorceries, sexual immorality, or thefts (Revelation 9:20-21).
            OracleHarness.Sin(OracleHarness.Grounded("mankind refuses to repent of its murders, sorceries, immorality, and thefts",
                Grounding.InIdol("demons and idols of gold and silver"),
                new Murder(), new Sorcery(), new SexualImmorality(), new Theft(), new Idolatry()));
            Assert.Equal(1.0, OracleHarness.Grounded("mankind refuses to repent of its murders, sorceries, immorality, and thefts",
                Grounding.InIdol("demons and idols of gold and silver"),
                new Murder(), new Sorcery(), new SexualImmorality(), new Theft(), new Idolatry()).Disorder);
        }

        [Fact]
        public void TheTwoWitnessesProphesyFaithfullyUntilSlain_IsRighteous()
        {
            // The two witnesses prophesy in sackcloth and bear faithful testimony before they are killed (Revelation 11:3-7).
            OracleHarness.Righteous(OracleHarness.Witness("the two witnesses bear faithful testimony and lay down their lives",
                new TeachingTruth(), new CourageousFaith(), new Fidelity(), new SelfSacrifice()));
        }

        [Fact]
        public void TheGreatDragonDeceivesTheWholeWorldAndAccusesTheBrethren_IsSin()
        {
            // The great dragon, the deceiver of the whole world, accuses the brethren day and night (Revelation 12:9-10).
            OracleHarness.Sin(OracleHarness.Grounded("the dragon deceives the whole world and accuses the brethren before God",
                Grounding.InIdol("the great dragon, that ancient serpent"),
                new Deceit(), new Rebellion(), new Condemnation(), new Slander()));
        }

        [Fact]
        public void TheBeastBlasphemesGodAndMakesWarOnTheSaints_IsSin()
        {
            // The beast opens its mouth in blasphemy against God and is allowed to make war on the saints (Revelation 13:5-7).
            OracleHarness.Sin(OracleHarness.Grounded("the beast blasphemes God and makes war to conquer the saints",
                Grounding.InIdol("the beast from the sea"),
                new Blasphemy(), new Bloodshed(), new Oppression(), new Pride()));
            Assert.Equal(1.0, OracleHarness.Grounded("the beast blasphemes God and makes war to conquer the saints",
                Grounding.InIdol("the beast from the sea"),
                new Blasphemy(), new Bloodshed(), new Oppression(), new Pride()).Disorder);
        }

        [Fact]
        public void BabylonTheGreatIntoxicatesTheNationsWithImmoralityAndLuxury_IsSin()
        {
            // Babylon makes the nations drunk with the wine of her immorality and lives in sensual luxury (Revelation 17:1-2; 18:3).
            OracleHarness.Sin(OracleHarness.Grounded("Babylon the great seduces the nations into immorality, idolatry, and excess",
                Grounding.InIdol("Babylon the great"),
                new SexualImmorality(), new Idolatry(), new Debauchery(), new Greed()));
        }

        [Fact]
        public void GodJudgesTheGreatHarlotWithTrueAndJustJudgments_IsADivineAct()
        {
            // True and just are His judgments, for He has judged the great harlot and avenged the blood of His servants (Revelation 19:1-2).
            OracleHarness.DivineAct(OracleHarness.Grounded("God judges the great harlot with true and just judgment and avenges His servants",
                Grounding.InGod(), new Righteousness(), new JustRecompense(), new Fidelity()));
        }

        [Fact]
        public void TheMarriageSupperOfTheLambAndTheBrideMadeReady_IsRighteous()
        {
            // The Bride has made herself ready, clothed in fine linen, the righteous deeds of the saints (Revelation 19:7-8).
            OracleHarness.Righteous(OracleHarness.Witness("the Bride of the Lamb is made ready, clothed in the righteous deeds of the saints",
                new Purity(), new Righteousness(), new Fidelity(), new Joy()));
        }

        [Fact]
        public void TheRiderCalledFaithfulAndTrueJudgesAndMakesWarInRighteousness_IsADivineAct()
        {
            // The rider on the white horse is called Faithful and True, and in righteousness He judges and makes war (Revelation 19:11).
            OracleHarness.DivineAct(OracleHarness.Grounded("the rider called Faithful and True judges and wars in righteousness",
                Grounding.InGod(), new Fidelity(), new Trustworthiness(), new Righteousness(), new JustRecompense()));
        }

        [Fact]
        public void GodMakesAllThingsNewAndDwellsWithHisPeople_IsADivineAct()
        {
            // Behold, the dwelling of God is with man; He wipes away every tear and makes all things new (Revelation 21:3-5).
            OracleHarness.DivineAct(OracleHarness.Grounded("God dwells with His people, wipes away every tear, and makes all things new",
                Grounding.InGod(), new Compassion(), new Healing(), new RestoringLife(), new CovenantFaithfulness(), new Peace()));
        }

        [Fact]
        public void TheLeavesOfTheTreeOfLifeAreForTheHealingOfTheNations_IsADivineAct()
        {
            // The river of the water of life and the tree of life, whose leaves are for the healing of the nations (Revelation 22:1-2).
            OracleHarness.DivineAct(OracleHarness.Grounded("God gives the water of life and the tree whose leaves heal the nations",
                Grounding.InGod(), new Healing(), new RestoringLife(), new Generosity(), new GivingDrink()));
        }
    }
}
