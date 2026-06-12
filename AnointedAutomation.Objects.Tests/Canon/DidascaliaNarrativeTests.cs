// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Didascalia narrative oracle (ETH): the Didascalia Apostolorum, the apostolic teaching of
    // church order. It commands bishops and the faithful toward mercy to the penitent, care for the
    // poor, orphan and widow, just judgment, honoring the church, and away from harshness, division,
    // and the snares of the world. The engine must return the verdict Scripture gives at each step.
    public class DidascaliaNarrativeTests
    {
        [Fact]
        public void Didascalia_ReceivingThePenitentSinnerWithMercy_IsRighteous()
        {
            // Did. ch. 6-7: the bishop is to receive the one who repents, heal and restore him, not crush him.
            OracleHarness.Righteous(OracleHarness.Witness("the bishop receives the penitent and restores him",
                new Forgiveness(), new Compassion(), new Repentance()));
        }

        [Fact]
        public void Didascalia_TheBishopJudgingJustlyWithoutPartiality_IsRighteous()
        {
            // Did. ch. 11: the bishop is to judge righteously, hearing both sides, without respect of persons.
            OracleHarness.Righteous(OracleHarness.Witness("the bishop judges justly, without partiality",
                new Righteousness(), new JustRecompense()));
        }

        [Fact]
        public void Didascalia_CaringForOrphansAndWidows_IsRighteous()
        {
            // Did. ch. 14-15: the church is to nourish the orphan and the widow as a sacred charge.
            OracleHarness.Righteous(OracleHarness.Witness("the church nourishes orphans and widows",
                new HonoringTheVulnerable(), new Compassion(), new Generosity()));
        }

        [Fact]
        public void Didascalia_FeedingTheHungryAndClothingTheNaked_IsRighteous()
        {
            // Did. ch. 17-18: out of the offerings the poor are fed and clothed and the stranger received.
            OracleHarness.Righteous(OracleHarness.Witness("feeding the poor and clothing the naked from the offerings",
                new FeedingTheHungry(), new ClothingTheNaked(), new Hospitality()));
        }

        [Fact]
        public void Didascalia_VisitingTheImprisonedAndTheSick_IsRighteous()
        {
            // Did. ch. 18-19: the faithful are to visit the sick and minister to those in prison and chains.
            OracleHarness.Righteous(OracleHarness.Witness("visiting the sick and the imprisoned confessors",
                new CaringForTheSick(), new VisitingThePrisoner(), new Kindness()));
        }

        [Fact]
        public void Didascalia_HonoringTheBishopAndKeepingChurchOrder_IsRighteous()
        {
            // Did. ch. 9: honor the bishop as the steward of God, keeping the order of the church.
            OracleHarness.Righteous(OracleHarness.Witness("honoring the bishop and keeping church order",
                new Worship(), new ObedienceToGod(), new CreationOrder()));
        }

        [Fact]
        public void Didascalia_ForgivingTheBrotherWhoTrespasses_IsRighteous()
        {
            // Did. ch. 6: be not unforgiving toward the brother who turns; pardon him as God pardons.
            OracleHarness.Righteous(OracleHarness.Witness("pardoning the brother who turns from his trespass",
                new Pardon(), new Forgiveness(), new Peacemaking()));
        }

        [Fact]
        public void Didascalia_TheRichManFavoredOverThePoorInTheAssembly_IsSin()
        {
            // Did. ch. 12: rebukes the bishop who honors the rich man and dishonors the poor man who enters.
            OracleHarness.Sin(OracleHarness.Witness("favoring the rich and despising the poor in the assembly",
                new Oppression(), new Indifference()));
        }

        [Fact]
        public void Didascalia_TheHarshBishopWhoRefusesThePenitent_IsSin()
        {
            // Did. ch. 6: the shepherd who will not heal the wounded sheep but condemns him sins gravely.
            OracleHarness.Sin(OracleHarness.Witness("the bishop who refuses and condemns the penitent",
                new Condemnation(), new Unforgiveness(), new Heartlessness()));
        }

        [Fact]
        public void Didascalia_KeepingTheBrotherInAGrudgeAndStrife_IsSin()
        {
            // Did. ch. 10-11: warns against the man who holds a grudge and sows strife among the brethren.
            OracleHarness.Sin(OracleHarness.Witness("holding a grudge and stirring strife among brethren",
                new Grudge(), new Discord(), new SowingDiscord()));
        }

        [Fact]
        public void Didascalia_FollowingTheRepetitionsAndBondsOfTheLaw_IsSin()
        {
            // Did. ch. 26: those who return to the "second legislation," the heavy bonds, after Christ's freedom.
            OracleHarness.Sin(OracleHarness.Witness("turning back to the bondage of the second legislation",
                new Rebellion(), new Disobedience()));
        }

        [Fact]
        public void Didascalia_AcceptingAlmsFromOppressorsAndExtortioners_IsSin()
        {
            // Did. ch. 18: forbids the church to take the gifts of the extortioner and the oppressor for the poor.
            OracleHarness.Sin(OracleHarness.Witness("taking the gifts of the extortioner and oppressor",
                new Greed(), new Oppression(), new Theft()));
        }

        [Fact]
        public void Didascalia_FleshlyImmoralityAndDrunkennessOfTheWorld_IsSin()
        {
            // Did. ch. 3 & 22: warns the faithful away from the immorality and drunkenness of the heathen.
            OracleHarness.Sin(OracleHarness.Witness("the immorality and drunkenness of the world",
                new SexualImmorality(), new Drunkenness(), new Debauchery()));
        }

        [Fact]
        public void Didascalia_SchismRendingTheUnityOfTheChurch_IsSin()
        {
            // Did. ch. 23: the heretic and schismatic who tear the one church are likened to wolves.
            OracleHarness.Sin(OracleHarness.Witness("rending the unity of the church by schism",
                new Faction(), new Rebellion(), new Treachery()));
        }
    }
}
