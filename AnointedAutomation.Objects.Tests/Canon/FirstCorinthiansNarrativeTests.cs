// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 1 Corinthians narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class FirstCorinthiansNarrativeTests
    {
        [Fact]
        public void TheCrossIsTheWisdomAndPowerOfGod_IsADivineAct()
        {
            // "Christ crucified ... the power of God and the wisdom of God." (1 Corinthians 1:23-24)
            OracleHarness.DivineAct(OracleHarness.Witness("the message of the cross", new SelfSacrifice(), new Deliverance()));
        }

        [Fact]
        public void DivisionsAndPartyStrifeInTheChurch_IsSin()
        {
            // "One of you says, 'I follow Paul'; another, 'I follow Apollos.'" (1 Corinthians 1:11-12; 3:3)
            OracleHarness.Sin(OracleHarness.Witness("the Corinthians quarrel into factions", new Discord(), new Faction()));
        }

        [Fact]
        public void BoastingInHumanWisdomAndTeachers_IsSin()
        {
            // "Let no one boast about human leaders." (1 Corinthians 3:21; 4:7)
            OracleHarness.Sin(OracleHarness.Witness("the church boasts in human wisdom", new Boasting(), new Pride()));
        }

        [Fact]
        public void TheManLivingWithHisFathersWife_IsSin()
        {
            // "A man is sleeping with his father's wife." (1 Corinthians 5:1-2)
            OracleHarness.Sin(OracleHarness.Witness("the incestuous man defiles the body", new SexualImmorality(), new Defilement()));
            // Capital defilement drives disorder to its limit.
            Assert.Equal(1.0, OracleHarness.Witness("the incestuous man defiles the body", new SexualImmorality(), new Defilement()).Disorder);
        }

        [Fact]
        public void BelieversSuingOneAnotherBeforePagans_IsSin()
        {
            // "Brother goes to law against brother ... You yourselves cheat and do wrong." (1 Corinthians 6:6-8)
            OracleHarness.Sin(OracleHarness.Witness("brother drags brother to court", new Wrongdoing(), new Discord()));
        }

        [Fact]
        public void UnitingTheBodyWithAProstitute_IsSin()
        {
            // "Flee from sexual immorality ... your body is a temple of the Holy Spirit." (1 Corinthians 6:18-19)
            OracleHarness.Sin(OracleHarness.Witness("joining the temple of the Spirit to immorality", new SexualImmorality(), new Impurity()));
        }

        [Fact]
        public void WoundingTheWeakConscienceOverIdolFood_IsSin()
        {
            // "When you sin against them ... you sin against Christ." (1 Corinthians 8:11-12)
            OracleHarness.Sin(OracleHarness.Witness("the strong despise the weak brother's conscience", new Indifference(), new Wrongdoing()));
        }

        [Fact]
        public void PaulRefusesPayToRemoveEveryHindrance_IsRighteous()
        {
            // "I have not used any of these rights ... so as not to hinder the gospel." (1 Corinthians 9:12-19)
            OracleHarness.Righteous(OracleHarness.Witness("Paul lays down his rights for the gospel", new SelfSacrifice(), new SelfControl()));
        }

        [Fact]
        public void WorshipingIdolsAtTheTempleFeasts_IsSin()
        {
            // "You cannot drink the cup of the Lord and the cup of demons." (1 Corinthians 10:14, 21)
            OracleHarness.Sin(OracleHarness.Grounded("the Corinthians feast at idol altars", Grounding.InIdol("demons of the table"), new Idolatry()));
        }

        [Fact]
        public void HumiliatingThePoorAtTheLordsTable_IsSin()
        {
            // "One remains hungry, another gets drunk ... you humiliate those who have nothing." (1 Corinthians 11:21-22)
            OracleHarness.Sin(OracleHarness.Witness("the rich shame the poor at the supper", new Drunkenness(), new Oppression()));
        }

        [Fact]
        public void TheOneSpiritGivesGiftsToEachForTheCommonGood_IsADivineAct()
        {
            // "To each one the manifestation of the Spirit is given for the common good." (1 Corinthians 12:7, 11)
            OracleHarness.DivineAct(OracleHarness.Witness("the Spirit distributes gifts to the body", new Generosity(), new CreationOrder()));
        }

        [Fact]
        public void LoveIsPatientKindAndRejoicesInTruth_IsRighteous()
        {
            // "Love is patient, love is kind ... it rejoices with the truth." (1 Corinthians 13:4-6)
            OracleHarness.Righteous(OracleHarness.Witness("the more excellent way of love", new Patience(), new Kindness(), new RejoicingInTruth()));
        }

        [Fact]
        public void ChristRaisedFromTheDeadDefeatsDeath_IsADivineAct()
        {
            // "Christ has indeed been raised ... Death has been swallowed up in victory." (1 Corinthians 15:20, 54)
            OracleHarness.DivineAct(OracleHarness.Witness("Christ is raised and death is undone", new RestoringLife(), new Deliverance()));
        }

        [Fact]
        public void TheCollectionForTheLordsPeople_IsRighteous()
        {
            // "On the first day of every week ... set aside a sum of money for the saints." (1 Corinthians 16:1-3)
            OracleHarness.Righteous(OracleHarness.Witness("the church gathers a gift for the poor saints", new Generosity(), new Compassion()));
        }
    }
}
