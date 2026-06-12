// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Abtilis narrative oracle: the engine must return the verdict the Apostolic Canons give at each step.
    // Canon: ETH (Ethiopic-only; best-effort). Abtilis is the fourth book of the Ethiopic Sinodos,
    // the Apostolic Canons (81/83 canons), paralleling Book VIII of the Apostolic Constitutions and the
    // Clementine Octateuch. It is church order: ordination, right worship, fasting, the discipline and
    // qualifications of clergy, care for the poor, and the rejection of simony, idolatry, and immorality.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class AbtilisNarrativeTests
    {
        [Fact]
        public void TheChurchCaresForWidowsOrphansAndTheNeedy_IsRighteous()
        {
            // The Apostolic Canons charge the bishop to distribute the offerings of the faithful to the
            // widows, the orphans, and the poor, and not to convert them to his own use. (Abtilis / Ap. Can. 4, 41)
            OracleHarness.Righteous(OracleHarness.Witness("the church cares for the widows, orphans, and the needy from its offerings",
                new HonoringTheVulnerable(), new Generosity(), new Compassion(), new Trustworthiness()));
        }

        [Fact]
        public void TheBishopIsOrdainedByLayingOnOfHandsInRightWorship_IsRighteous()
        {
            // A bishop is to be ordained by two or three bishops with prayer and the laying on of hands,
            // ordering the worship of the Church. (Abtilis / Ap. Can. 1)
            OracleHarness.Righteous(OracleHarness.Witness("a bishop is ordained by the laying on of hands in right worship",
                new Worship(), new ObedienceToGod(), new CreationOrder()));
        }

        [Fact]
        public void TheBishopTeachesSoundDoctrineToTheFlock_IsRighteous()
        {
            // The bishop is bound to teach the words of godliness rightly and to feed the people with
            // sound doctrine. (Abtilis / Ap. Can. 58)
            OracleHarness.Righteous(OracleHarness.Witness("the bishop teaches sound doctrine and feeds the flock",
                new TeachingTruth(), new Fidelity(), new RejoicingInTruth()));
        }

        [Fact]
        public void TheChurchKeepsTheLordsDayAndAppointedFastsFaithfully_IsRighteous()
        {
            // The canons command the faithful keeping of the Lord's Day, of Pentecost, and of the
            // appointed fasts of Wednesday and Friday. (Abtilis / Ap. Can. 66, 69)
            OracleHarness.Righteous(OracleHarness.Witness("the church keeps the Lord's Day and the appointed fasts faithfully",
                new Worship(), new Fidelity(), new SelfControl(), new ObedienceToGod()));
        }

        [Fact]
        public void ThePenitentBrotherIsReceivedBackInMercy_IsRighteous()
        {
            // One cut off for sin who repents is to be received again, lest he be swallowed up by
            // overmuch sorrow; the canons forbid crushing the contrite. (Abtilis / Ap. Can. 52)
            OracleHarness.Righteous(OracleHarness.Witness("the penitent brother who turns from sin is received back in mercy",
                new Forgiveness(), new Repentance(), new Compassion(), new Pardon()));
        }

        [Fact]
        public void TravelingBrethrenAreReceivedWithHospitality_IsRighteous()
        {
            // Clergy and faithful traveling abroad are to be received with letters of peace and shown
            // hospitality, not turned away. (Abtilis / Ap. Can. 12-13)
            OracleHarness.Righteous(OracleHarness.Witness("traveling brethren are received with letters of peace and hospitality",
                new Hospitality(), new Kindness(), new Peacemaking()));
        }

        [Fact]
        public void OrdainingOrBuyingHolyOfficeForMoney_IsSin()
        {
            // Whoever obtains the office of bishop, priest, or deacon by money, he and the one who
            // ordained him are to be cut off: the sin of simony. (Abtilis / Ap. Can. 29)
            OracleHarness.Sin(OracleHarness.Witness("a man buys the holy office for money and the ordainer sells it",
                new Greed(), new Theft(), new Deceit()));
        }

        [Fact]
        public void AClericWhoStrikesTheFaithfulInRage_IsSin()
        {
            // A bishop or presbyter who strikes the faithful who sin, or the unbelievers who wrong them,
            // seeking to terrify them by violence, is to be deposed. (Abtilis / Ap. Can. 27)
            OracleHarness.Sin(OracleHarness.Witness("a cleric strikes the faithful in a fit of rage to terrify them",
                new FitsOfRage(), new Oppression(), new Rudeness()));
        }

        [Fact]
        public void OfferingToIdolsAndPartakingWithIdolaters_IsSin()
        {
            // The canons forbid the faithful to bring strange offerings to the altar or to share in the
            // worship and feasts of idols. (Abtilis / Ap. Can. 3, 7, 70-71)
            OracleHarness.Sin(OracleHarness.Witness("clergy offer to idols and partake of the table of idolaters",
                new Idolatry(), new Defilement(), new CovenantBreaking()));
        }

        [Fact]
        public void ClergyGivenToDrunkennessAndGambling_IsSin()
        {
            // A bishop, presbyter, or deacon given to drunkenness or to dicing and revelry is to either
            // cease or be deposed. (Abtilis / Ap. Can. 42-43)
            OracleHarness.Sin(OracleHarness.Witness("clergy give themselves to drunkenness, dicing, and revelry",
                new Drunkenness(), new Carousing(), new Gluttony()));
        }

        [Fact]
        public void TheBishopWhoWithholdsTheChurchGoodsFromThePoor_IsSin()
        {
            // A bishop who squanders the goods of the Church to his own ends and withholds them from the
            // poor and needy is to give account and be deposed. (Abtilis / Ap. Can. 4, 38, 41)
            OracleHarness.Sin(OracleHarness.Witness("a bishop withholds the church goods from the poor and spends them on himself",
                new WithholdingWhatIsDue(), new Greed(), new Indifference(), new Heartlessness()));
        }

        [Fact]
        public void BearingFalseWitnessOrTakingBribesToPervertJudgment_IsSin()
        {
            // The canons condemn the cleric who bears false witness or accepts a bribe to pervert
            // righteous judgment against a brother. (Abtilis / Ap. Can. 74-75)
            OracleHarness.Sin(OracleHarness.Witness("a cleric bears false witness and takes a bribe to pervert judgment",
                new FalseWitness(), new Deceit(), new Wrongdoing()));
        }
    }
}
