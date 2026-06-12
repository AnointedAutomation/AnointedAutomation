// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 1 Book of the Covenant (Dominos) narrative oracle: the engine must return the verdict the book
    // gives at each step. Canon: ETH (Ethiopic-only; best-effort). The "Book of the Covenant" is the
    // Ethiopic Mäshafä Kidan (the Testament of Our Lord, Testamentum Domini). Book One opens with the
    // risen Christ, on the evening after His resurrection, revealing Himself to the apostles (with
    // Martha, Mary, and Salome) and unfolding to them the signs of the end and the coming of the
    // antichrist; it then sets out the "Holy Order" of the Church: the bishop, presbyters, deacons,
    // widows, the care of the poor, the sick, the prisoner, baptism, the Eucharist, and the discipline
    // that guards the flock from immorality and false teaching. These tests trace its load-bearing
    // events and teachings.
    public class FirstDominosNarrativeTests
    {
        [Fact]
        public void TheRisenLordRevealsHimselfToTheApostlesAfterTheResurrection_IsADivineAct()
        {
            // On the evening after His resurrection the Lord shows Himself living to the apostles and,
            // at their request, begins to teach them the things that must come to pass. (1 Covenant /
            // Testamentum Domini I, prologue: the self-revelation of the risen Lord)
            // Christ, raised, teaching truth and keeping His covenant promise to His own.
            OracleHarness.DivineAct(OracleHarness.Grounded("the risen Lord reveals Himself to the apostles and teaches them after His resurrection",
                Grounding.InGod(),
                new RestoringLife(), new TeachingTruth(), new PromiseFulfilled(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void ChristForetellsTheSignsOfTheEndToWarnAndPrepareTheFaithful_IsADivineAct()
        {
            // The Lord unfolds to the apostles the signs of the end, that the faithful may watch, endure,
            // and not be deceived. (1 Covenant I: the apocalyptic discourse, the signs of the end)
            // Christ teaching truth and guarding His people: a warning given in faithfulness, not fear.
            OracleHarness.DivineAct(OracleHarness.Grounded("Christ foretells the signs of the end to warn and prepare His faithful",
                Grounding.InGod(),
                new TeachingTruth(), new Protection(), new CovenantFaithfulness(),
                new Fidelity(), new Goodness()));
        }

        [Fact]
        public void TheLordChargesTheFaithfulToEndureAndKeepCovenantInPersecution_IsRighteous()
        {
            // The faithful are set in the midst of wolves, despised by the worldly; the Lord charges them
            // to stand firm, keep His covenant, and hold fast in patience. (1 Covenant I: exhortation to
            // endurance amid persecution)
            // The disciple's steadfastness: courageous faith, patient endurance, and covenant loyalty.
            OracleHarness.Righteous(OracleHarness.Grounded("the faithful endure persecution and keep the Lord's covenant",
                Grounding.InGod(),
                new CourageousFaith(), new Endurance(), new Patience(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void TheAntichristExaltsHimselfAndPersecutesTheSaints_IsSin()
        {
            // The apocalyptic discourse names the antichrist, who exalts himself, deceives the nations,
            // claims worship, and persecutes the saints. (1 Covenant I: the coming of the antichrist)
            // The lawless one: pride, deceit, blasphemy, idolatrous self-worship, and oppression.
            OracleHarness.Sin(OracleHarness.Grounded("the antichrist exalts himself, deceives the nations, claims worship, and persecutes the saints",
                Grounding.InIdol("antichrist"),
                new Pride(), new Deceit(), new Blasphemy(), new Idolatry(),
                new Oppression(), new Rebellion(), new Lawlessness()));
        }

        [Fact]
        public void TheApostlesWorshipTheRisenLordAndReceiveHisCovenant_IsRighteous()
        {
            // Hearing the Lord, the apostles fall down, worship Him, and receive the covenant He gives.
            // (1 Covenant I: the apostles' response and the giving of the Testament)
            // Right response to the risen Lord: worship, obedience, humility, and covenant faith.
            OracleHarness.Righteous(OracleHarness.Grounded("the apostles worship the risen Lord and receive His covenant",
                Grounding.InGod(),
                new Worship(), new ObedienceToGod(), new Humility(),
                new CovenantFaithfulness(), new Trust()));
        }

        [Fact]
        public void TheChurchOrdainsTheBishopToShepherdAndGuardTheFlock_IsRighteous()
        {
            // The Order sets out the ordination of the bishop, who is to feed, guard, and teach the flock
            // as a faithful shepherd. (1 Covenant I: the ordination and office of the bishop)
            // Faithful obedience that protects and teaches the people of God.
            OracleHarness.Righteous(OracleHarness.Witness("the Church ordains the bishop to shepherd, guard, and teach the flock",
                new ObedienceToGod(), new Protection(), new TeachingTruth(),
                new CovenantFaithfulness(), new Fidelity()));
        }

        [Fact]
        public void TheBishopMustBeBlamelessMercifulAndHospitable_IsRighteous()
        {
            // The bishop is to be without reproach, self-controlled, gentle, merciful to the poor, and
            // given to hospitality. (1 Covenant I: the qualities required of the bishop)
            // The ordering of a holy life: self-mastery, gentleness, welcome, mercy, and purity.
            OracleHarness.Righteous(OracleHarness.Witness("the bishop must be blameless, self-controlled, gentle, hospitable, and merciful",
                new SelfControl(), new Gentleness(), new Hospitality(),
                new Compassion(), new Humility(), new Purity()));
        }

        [Fact]
        public void TheWidowsAndDeaconsServeThePoorSickAndImprisoned_IsRighteous()
        {
            // The Order appoints the widows to prayer and the deacons to minister to the poor, the sick,
            // and those in prison. (1 Covenant I: the order of widows and deacons; the care of the needy)
            // Mercy embodied: feeding the hungry, tending the sick, and visiting the imprisoned.
            OracleHarness.Righteous(OracleHarness.Witness("the widows pray and the deacons serve the poor, the sick, and the imprisoned",
                new FeedingTheHungry(), new CaringForTheSick(), new VisitingThePrisoner(),
                new HonoringTheVulnerable(), new SelfSacrifice(), new Kindness()));
        }

        [Fact]
        public void TheChurchClothesTheNakedAndGivesDrinkToTheThirsty_IsRighteous()
        {
            // The faithful bring their offerings so that the naked are clothed and the thirsty given drink
            // from the Church's gifts. (1 Covenant I: the offerings and the care of the destitute)
            // Generosity in action: clothing the naked, giving drink, and honoring the lowly.
            OracleHarness.Righteous(OracleHarness.Witness("the Church clothes the naked and gives drink to the thirsty from its offerings",
                new ClothingTheNaked(), new GivingDrink(), new Generosity(),
                new HonoringTheVulnerable(), new Compassion()));
        }

        [Fact]
        public void TheConvertRenouncesEvilAndIsBaptizedUntoTheLord_IsRighteous()
        {
            // The Order of baptism: the catechumen renounces the devil and his works, is washed, and is
            // sealed unto the Lord. (1 Covenant I: the order of baptism)
            // A turning from evil to God: repentance, cleansing, and faithful worship.
            OracleHarness.Righteous(OracleHarness.Witness("the convert renounces evil and is baptized and sealed unto the Lord",
                new Repentance(), new Purity(), new Worship(),
                new ObedienceToGod(), new CourageousFaith()));
        }

        [Fact]
        public void TheChurchOffersTheEucharistWithThanksgivingAndWorship_IsRighteous()
        {
            // The order of the holy offering: the people gather, give thanks, and the Body and Blood of
            // the Lord are offered in remembrance of Him. (1 Covenant I: the order of the Eucharist and
            // the prayers of thanksgiving)
            // The Church's right worship: thanksgiving, reverence, joy, and obedient remembrance.
            OracleHarness.Righteous(OracleHarness.Grounded("the Church offers the Eucharist with thanksgiving and worship",
                Grounding.InGod(),
                new Worship(), new ObedienceToGod(), new Joy(),
                new Peace(), new Fidelity()));
        }

        [Fact]
        public void TheUnworthyManSeizesTheHolyOfficeForGain_IsSin()
        {
            // The Order warns against the man who pushes himself forward to office out of pride and greed
            // rather than calling. (1 Covenant I: against unworthy and ambitious ordination)
            // Ambition and avarice corrupting the holy office: pride, greed, and self-seeking.
            OracleHarness.Sin(OracleHarness.Grounded("an unworthy, greedy man seizes the holy office for gain",
                Grounding.InIdol("self"),
                new Pride(), new Greed(), new SelfishAmbition(), new SelfSeeking(), new Boasting()));
        }

        [Fact]
        public void TheClergymanGivenToDrunkennessAndFornicationDefilesHisOffice_IsSin()
        {
            // The Order forbids the clergy from drunkenness, fornication, and filthy living, and commands
            // the deposing of such. (1 Covenant I: the discipline of the clergy)
            // Debauchery defiling the holy office: drunkenness, immorality, and impurity.
            OracleHarness.Sin(OracleHarness.Witness("a clergyman given to drunkenness and fornication defiles his office",
                new Drunkenness(), new SexualImmorality(), new Debauchery(), new Impurity(), new Defilement()));
        }

        [Fact]
        public void FalseTeachersCorruptTheApostlesDoctrineAndLeadTheFlockAstray_IsSin()
        {
            // The Order commands that heretics and false teachers who corrupt the doctrine of the Lord and
            // His apostles be rejected. (1 Covenant I: against false teaching and heresy)
            // Deceit and rebellion against the truth, sowing division among the faithful.
            OracleHarness.Sin(OracleHarness.Witness("false teachers corrupt the apostolic doctrine and lead the flock astray",
                new Deceit(), new FalseWitness(), new Rebellion(), new SowingDiscord(), new Blasphemy()));
        }
    }
}
