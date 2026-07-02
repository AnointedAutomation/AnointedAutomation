// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 2 Book of the Covenant (Dominos) narrative oracle: the engine must return the verdict the book gives at each step.
    // Canon: ETH (Ethiopic-only; best-effort). The "Book of the Covenant" is the Ethiopic Mashafa Kidan
    // (Testamentum Domini / Testament of our Lord), the risen Christ's teaching to the apostles. The SECOND book
    // governs the life of the laity: preparation for and admission to baptism, the renunciation of Satan, the
    // baptismal confession of faith, anointing and the seal, the Eucharist, the keeping of Easter and Pentecost,
    // the agape love-feast for the poor, the hours of prayer, visiting the sick, the burial of the dead, and the
    // ordered support of widows and virgins, all in a tone of ascetic self-discipline. It warns the faithful
    // against returning to idols and against the immorality and violence of the world.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class SecondDominosNarrativeTests
    {
        [Fact]
        public void TheRisenChristTeachesTheApostlesTheWayOfTheCovenant_IsADivineAct()
        {
            // The second Book of the Covenant is the words the risen Lord speaks to His apostles, ordering
            // the worship and life of His people in covenant faithfulness. (2 Covenant / Testament II, prologue)
            OracleHarness.DivineAct(OracleHarness.Witness("the risen Christ teaches the apostles the way of the covenant",
                new TeachingTruth(), new CovenantFaithfulness(), new CreationOrder(), new RejoicingInTruth()));
        }

        [Fact]
        public void TheCatechumenIsInstructedAndPreparedForBaptism_IsRighteous()
        {
            // The candidate is examined, taught the words of godliness, and made ready over a season of
            // preparation before he may be admitted to the laver. (2 Covenant / Testament II.1-8)
            OracleHarness.Righteous(OracleHarness.Witness("the catechumen is instructed in the faith and prepared for baptism",
                new TeachingTruth(), new ObedienceToGod(), new Patience(), new Purity()));
        }

        [Fact]
        public void TheCandidateRenouncesSatanAndTurnsToGod_IsRighteous()
        {
            // Standing toward the west, the candidate renounces Satan and all his works, then turns to the
            // east and gives himself to Christ. (2 Covenant / Testament II.8)
            OracleHarness.Righteous(OracleHarness.Witness("the candidate renounces Satan and all his works and turns to God",
                new Repentance(), new ObedienceToGod(), new CovenantFaithfulness(), new CourageousFaith()));
        }

        [Fact]
        public void TheBaptizedConfessesFaithAndIsSealed_IsRighteous()
        {
            // The candidate confesses faith in the Father, the Son, and the Holy Spirit, is washed in the
            // laver, and receives the anointing and the seal. (2 Covenant / Testament II.8-9)
            OracleHarness.Righteous(OracleHarness.Witness("the baptized confesses the faith, is washed, and receives the seal",
                new Worship(), new Fidelity(), new Purity(), new ObedienceToGod()));
        }

        [Fact]
        public void TheFaithfulKeepTheEucharistInThanksgiving_IsRighteous()
        {
            // The newly sealed and the whole body partake of the Eucharist with thanksgiving, the
            // remembrance the Lord appointed. (2 Covenant / Testament II.10)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful keep the Eucharist with thanksgiving and remembrance",
                new Worship(), new Fidelity(), new Joy(), new ObedienceToGod()));
        }

        [Fact]
        public void TheChurchKeepsEasterAndPentecostWithJoy_IsRighteous()
        {
            // The book commands the faithful keeping of the Pasch and of the fifty days of Pentecost,
            // rejoicing in the Lord's resurrection. (2 Covenant / Testament II.11-12)
            OracleHarness.Righteous(OracleHarness.Witness("the church keeps Easter and the fifty days of Pentecost with joy",
                new Worship(), new Joy(), new Fidelity(), new ObedienceToGod()));
        }

        [Fact]
        public void TheAgapeFeastFeedsThePoorAndTheWidows_IsRighteous()
        {
            // At the agape the faithful gather and the poor, the widows, and the orphans are fed from the
            // gifts of the Church, not sent empty away. (2 Covenant / Testament II.13)
            OracleHarness.Righteous(OracleHarness.Witness("the agape love-feast feeds the poor, the widows, and the orphans",
                new FeedingTheHungry(), new Generosity(), new HonoringTheVulnerable(), new Compassion()));
        }

        [Fact]
        public void TheFaithfulKeepTheHoursOfPrayerAndPsalmody_IsRighteous()
        {
            // The book appoints the hours of prayer and the singing of psalms by night and day, that the
            // people may continue steadfast before God. (2 Covenant / Testament II.22)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful keep the appointed hours of prayer and psalmody",
                new Worship(), new Fidelity(), new Endurance(), new SelfControl()));
        }

        [Fact]
        public void TheChurchVisitsAndTendsTheSick_IsRighteous()
        {
            // The faithful and the deacons are charged to visit the sick, to pray over them, and to tend
            // them in their affliction. (2 Covenant / Testament II.21)
            OracleHarness.Righteous(OracleHarness.Witness("the church visits and tends the sick in their affliction",
                new CaringForTheSick(), new Compassion(), new Kindness(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void TheDeadAreBuriedWithHonorAndPrayer_IsRighteous()
        {
            // The book orders that the bodies of the faithful departed be carried out and buried with
            // honor and prayer, without grasping or extortion over the dead. (2 Covenant / Testament II.23)
            OracleHarness.Righteous(OracleHarness.Witness("the faithful departed are buried with honor and prayer",
                new HonoringTheVulnerable(), new Compassion(), new Fidelity(), new Mourning()));
        }

        [Fact]
        public void TheWidowsAndVirginsLiveInChastePrayerfulDiscipline_IsRighteous()
        {
            // The widows who have precedence and the consecrated virgins are to give themselves to prayer
            // and fasting, keeping themselves chaste and sober. (2 Covenant / Testament II.19; cf. I.46)
            OracleHarness.Righteous(OracleHarness.Witness("the widows and virgins live in chaste, prayerful, sober discipline",
                new Purity(), new SelfControl(), new Worship(), new Endurance()));
        }

        [Fact]
        public void TheBelieverWhoReturnsToIdolsAndTheTableOfDemons_IsSin()
        {
            // The one who, after renouncing Satan at the laver, turns back to idols and shares the table of
            // demons breaks the covenant he confessed. (2 Covenant / Testament II.8, warning)
            OracleHarness.Sin(OracleHarness.Witness("a baptized believer returns to idols and shares the table of demons",
                new Idolatry(), new CovenantBreaking(), new Defilement(), new Disobedience()));
        }

        [Fact]
        public void TheWorldDefilesTheFaithfulWithImmoralityAndDrunkenness_IsSin()
        {
            // The book sets the ascetic, chaste life of the covenant against the immorality, drunkenness,
            // and revelry of the world, which defile the body. (2 Covenant / Testament II, ethical exhortation)
            OracleHarness.Sin(OracleHarness.Witness("the world defiles the faithful with sexual immorality, drunkenness, and carousing",
                new SexualImmorality(), new Drunkenness(), new Carousing(), new Impurity()));
        }

        [Fact]
        public void ThePersecutorWhoShedsTheBloodOfTheFaithful_IsSin()
        {
            // The Testament is written under the shadow of persecution; the persecutor who sheds the blood
            // of the faithful for their confession does murder. (2 Covenant / Testament, persecution warning)
            Resolution r = OracleHarness.Witness("the persecutor sheds the blood of the faithful for their confession of Christ",
                new Murder(), new Bloodshed(), new Oppression(), new Hatred());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }
    }
}
