// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Sirate Tsion (the "Order of Zion") narrative oracle: the engine must return the verdict the
    // book gives at each step. Canon: ETH (Ethiopic-only; best-effort). Sirate Tsion is the first
    // of the four parts of the Sinodos, the apostolic "Book of Order" of the Church: it sets out the
    // ordination of bishops, priests, and deacons, the order of worship and the Eucharist, the care
    // of the poor, widows, and orphans, baptism, and the disciplines that guard the Church from
    // false teaching and immorality. The tests below trace its load-bearing teachings and orders.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class SirateTsionNarrativeTests
    {
        [Fact]
        public void ChristGivesTheApostlesTheOrderOfTheChurch_IsADivineAct()
        {
            // The apostles set down the "Book of Order" because the Lord Himself appointed them and
            // delivered to them the pattern of the Church's worship and ministry. (Sirate Tsion,
            // prologue: the Holy Order given by Christ to the apostles)
            // Christ ordering His Church: teaching, covenant, and the right ordering of creation.
            OracleHarness.DivineAct(OracleHarness.Grounded("Christ delivers to the apostles the Holy Order of Zion for His Church",
                Grounding.InGod(),
                new TeachingTruth(), new CreationOrder(), new CovenantFaithfulness(),
                new ObedienceToGod(), new Fidelity()));
        }

        [Fact]
        public void TheApostlesOrdainBishopsToShepherdTheFlock_IsRighteous()
        {
            // The apostles, as Christ ordained them, ordain bishops to succeed them and to guard and
            // feed the flock. (Sirate Tsion, on the ordination of the bishop)
            // Faithful obedience that shepherds and protects the people of God.
            OracleHarness.Righteous(OracleHarness.Witness("the apostles ordain bishops to shepherd and guard the flock",
                new ObedienceToGod(), new Protection(), new CovenantFaithfulness(),
                new TeachingTruth(), new Fidelity()));
        }

        [Fact]
        public void TheBishopMustBeBlamelessTemperateAndHospitable_IsRighteous()
        {
            // The bishop is to be without reproach, self-controlled, gentle, given to hospitality,
            // and apt to teach. (Sirate Tsion, on the qualities of the bishop; cf. 1 Tim 3)
            // The ordering of a holy life: self-mastery, gentleness, welcome, and sound teaching.
            OracleHarness.Righteous(OracleHarness.Witness("the bishop is to be blameless, self-controlled, gentle, hospitable, and apt to teach",
                new SelfControl(), new Gentleness(), new Hospitality(),
                new TeachingTruth(), new Humility(), new Purity()));
        }

        [Fact]
        public void DeaconsAreSetApartToServeThePoorAndTheSick_IsRighteous()
        {
            // Deacons are ordained to minister at the table of the Lord and to serve the widows, the
            // poor, and the sick. (Sirate Tsion, on the order of deacons)
            // Service to the vulnerable: feeding, caring for the sick, and honoring the lowly.
            OracleHarness.Righteous(OracleHarness.Witness("deacons are set apart to serve the widows, the poor, and the sick",
                new CaringForTheSick(), new FeedingTheHungry(), new HonoringTheVulnerable(),
                new Kindness(), new SelfSacrifice()));
        }

        [Fact]
        public void TheChurchGathersToOfferTheEucharistInWorship_IsRighteous()
        {
            // The order of the holy offering: the people gather, give thanks, and the Body and Blood
            // of the Lord are offered in remembrance of Him. (Sirate Tsion, on the order of the
            // Eucharist and the prayers of thanksgiving)
            // The Church's right worship: thanksgiving, reverence, and obedient remembrance.
            OracleHarness.Righteous(OracleHarness.Grounded("the Church gathers to offer the Eucharist with thanksgiving and worship",
                Grounding.InGod(),
                new Worship(), new ObedienceToGod(), new Joy(),
                new Peace(), new Fidelity()));
        }

        [Fact]
        public void ConvertsAreBaptizedAndRenounceTheDevil_IsRighteous()
        {
            // The order of baptism: the catechumen renounces the devil and his works, is washed, and
            // is sealed unto the Lord. (Sirate Tsion, on the order of baptism)
            // A turning from evil to God: repentance, cleansing, and faithful worship.
            OracleHarness.Righteous(OracleHarness.Witness("the convert renounces the devil and is baptized unto the Lord",
                new Repentance(), new Purity(), new Worship(),
                new ObedienceToGod(), new CourageousFaith()));
        }

        [Fact]
        public void TheChurchCommandsCareForWidowsAndOrphans_IsRighteous()
        {
            // The Order commands the bishop and faithful to feed the hungry, clothe the naked, and
            // sustain the widows and the fatherless out of the Church's gifts. (Sirate Tsion, on the
            // care of the poor and the offerings of the faithful)
            // Mercy embodied: feeding, clothing, generosity, and honor toward the defenseless.
            OracleHarness.Righteous(OracleHarness.Witness("the Church feeds, clothes, and sustains the widows and orphans",
                new FeedingTheHungry(), new ClothingTheNaked(), new Generosity(),
                new HonoringTheVulnerable(), new DefendingTheWeak(), new Compassion()));
        }

        [Fact]
        public void TheFaithfulOfferTheirFirstfruitsAndTithesToGod_IsRighteous()
        {
            // The Order directs the people to bring their firstfruits and offerings to the bishop for
            // the Lord and for the poor. (Sirate Tsion, on offerings and firstfruits)
            // Grateful, generous worship that obeys God and provides for the needy.
            OracleHarness.Righteous(OracleHarness.Grounded("the faithful bring their firstfruits and offerings to God and the poor",
                Grounding.InGod(),
                new Generosity(), new Worship(), new ObedienceToGod(), new Goodness()));
        }

        [Fact]
        public void TheUnworthyManSeizesTheOfficeOfBishopForGain_IsSin()
        {
            // The Order warns against the man who pushes himself forward to the episcopate out of
            // pride and greed rather than calling. (Sirate Tsion, against unworthy ordination)
            // Ambition and avarice corrupting the holy office: pride, greed, and self-seeking.
            OracleHarness.Sin(OracleHarness.Grounded("an unworthy, greedy man seizes the office of bishop for gain",
                Grounding.InIdol("self"),
                new Pride(), new Greed(), new SelfishAmbition(), new SelfSeeking(), new Boasting()));
        }

        [Fact]
        public void FalseTeachersAndHereticsLeadTheFlockAstray_IsSin()
        {
            // The Order commands that heretics and false teachers who corrupt the doctrine of the
            // apostles be rejected. (Sirate Tsion, against false teaching and heresy)
            // Deceit and rebellion against the truth, sowing division in the Church.
            OracleHarness.Sin(OracleHarness.Witness("false teachers corrupt the apostolic doctrine and lead the flock astray",
                new Deceit(), new FalseWitness(), new Rebellion(), new SowingDiscord(), new Blasphemy()));
        }

        [Fact]
        public void TheClergymanGivenToDrunkennessAndImmorality_IsSin()
        {
            // The Order forbids the clergy from drunkenness, fornication, and filthy living, and
            // commands the deposing of such. (Sirate Tsion, on the discipline of the clergy)
            // Debauchery defiling the holy office: drunkenness, immorality, and impurity.
            OracleHarness.Sin(OracleHarness.Witness("a clergyman given to drunkenness and fornication defiles his office",
                new Drunkenness(), new SexualImmorality(), new Debauchery(), new Impurity(), new Defilement()));
        }

        [Fact]
        public void TheChurchPursuesPeaceAndReconcilesTheBrethren_IsRighteous()
        {
            // The Order charges the bishop to make peace among the brethren, to forgive the penitent,
            // and to restore the fallen. (Sirate Tsion, on reconciliation and discipline)
            // The shepherd's mercy: peacemaking, patience, forgiveness, and restored fellowship.
            OracleHarness.Righteous(OracleHarness.Witness("the bishop makes peace among the brethren and forgives the penitent",
                new Peacemaking(), new Forgiveness(), new Patience(), new Pardon(), new Gentleness()));
        }
    }
}
