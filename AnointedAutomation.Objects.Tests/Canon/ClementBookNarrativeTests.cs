// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Book of Clement (Ethiopic Mäshafä Qälemenṭos / Qalementos) narrative oracle:
    // the engine must return the verdict Scripture gives at each step.
    // Canon: ETH (Ethiopic-only; best-effort). The work is framed as the secret teaching the
    // Lord gave to Peter, who delivers it to Clement; it retells creation, the watchers, the flood,
    // the patriarchs and the priestly line, and closes with exhortation and warning.
    [Trait("Canon", "EthiopicNonLoadBearing")]
    public class ClementBookNarrativeTests
    {
        [Fact]
        public void GodCreatesTheHeavensAndTheEarthInOrder_IsADivineAct()
        {
            // The book opens by retelling how God in His wisdom established the heavens, the earth,
            // and every ordered creature (Qalementos, Book 1, the creation account).
            // God's ordering of reality is the very pattern of coherence.
            OracleHarness.DivineAct(OracleHarness.Grounded("God creates the heavens and the earth and orders all things",
                Grounding.InGod(),
                new CreationOrder(), new Goodness(), new Fidelity(), new TeachingTruth()));
        }

        [Fact]
        public void ChristEntrustsTheSecretTeachingToPeterForClement_IsADivineAct()
        {
            // The frame: the Lord delivers His hidden teaching to Peter, charging him to hand it to
            // Clement so the truth is preserved for the church (Qalementos, Book 1, the commission).
            // Christ teaching truth and entrusting His covenant word is a divine act.
            OracleHarness.DivineAct(OracleHarness.Grounded("Christ entrusts His secret teaching to Peter to deliver to Clement",
                Grounding.InGod(),
                new TeachingTruth(), new CovenantFaithfulness(), new Trustworthiness(), new RejoicingInTruth()));
        }

        [Fact]
        public void PeterFaithfullyHandsTheTeachingToClement_IsRighteous()
        {
            // Peter, obedient to the charge, writes and delivers the teaching to Clement
            // (Qalementos, Book 1). A faithful steward passing on the deposit of truth.
            OracleHarness.Righteous(OracleHarness.Witness("Peter faithfully writes and delivers the Lord's teaching to Clement",
                new ObedienceToGod(), new Fidelity(), new TeachingTruth(), new Trustworthiness()));
        }

        [Fact]
        public void TheWatchersDescendAndDefileTheDaughtersOfMen_IsSin()
        {
            // Retelling the fall of the watchers: the sons of heaven abandon their station, descend,
            // and defile the daughters of men, teaching forbidden arts (Qalementos, the antediluvian
            // history echoing 1 Enoch). Rebellion, immorality, defilement, and sorcery together.
            Resolution r = OracleHarness.Witness("the watchers abandon heaven and defile the daughters of men, teaching forbidden arts",
                new Rebellion(), new SexualImmorality(), new Defilement(), new Sorcery(), new Disobedience());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheEarthIsFilledWithBloodshedBeforeTheFlood_IsSin()
        {
            // The offspring of the watchers and the wickedness of men fill the earth with violence and
            // blood, so that the ground itself cries out (Qalementos, the corruption before the flood).
            // Capital bloodshed drives reality to full disorder.
            Resolution r = OracleHarness.Witness("the earth is filled with violence and bloodshed before the flood",
                new Bloodshed(), new Murder(), new Lawlessness(), new Chaos(), new Oppression());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void NoahWalksWithGodAndIsFoundRighteous_IsRighteous()
        {
            // Amid the corruption, Noah alone is found upright and obedient, walking with God
            // (Qalementos, the Noah narrative). A righteous remnant clinging to obedience and trust.
            OracleHarness.Righteous(OracleHarness.Witness("Noah walks with God and is found righteous in his generation",
                new Righteousness(), new ObedienceToGod(), new Fidelity(), new Trust(), new Purity()));
        }

        [Fact]
        public void GodPreservesNoahThroughTheFloodAndRenewsTheCovenant_IsADivineAct()
        {
            // God shuts Noah in the ark, delivers him and his house through the waters, and afterward
            // renews His covenant with the earth (Qalementos, the flood and the covenant of Noah).
            // Deliverance and a kept promise are a divine act.
            OracleHarness.DivineAct(OracleHarness.Grounded("God delivers Noah through the flood and renews His covenant",
                Grounding.InGod(),
                new Deliverance(), new Protection(), new CovenantFaithfulness(), new PromiseFulfilled(), new Compassion()));
        }

        [Fact]
        public void GodAppointsThePriestlyLineToGuardWorshipAndTruth_IsADivineAct()
        {
            // The book traces how God sets apart the priestly order to guard true worship and to teach
            // His commandments to the people (Qalementos, the priesthood material drawn into the Sinodos).
            // God establishing right worship and ordered teaching is a divine act.
            OracleHarness.DivineAct(OracleHarness.Grounded("God appoints the priestly line to guard worship and teach His commandments",
                Grounding.InGod(),
                new Worship(), new CreationOrder(), new TeachingTruth(), new CovenantFaithfulness(), new Righteousness()));
        }

        [Fact]
        public void TheNationsTurnToIdolsAndForbiddenWorship_IsSin()
        {
            // After the flood the book laments how the nations forsake the living God and bow to idols,
            // mingling worship with sorcery and deceit (Qalementos, the apostasy of the nations).
            // Idolatry and its attendant lies disorder reality.
            OracleHarness.Sin(OracleHarness.Grounded("the nations forsake God and turn to idols and forbidden worship",
                Grounding.InIdol("the idols of the nations"),
                new Idolatry(), new Sorcery(), new Deceit(), new Rebellion(), new CovenantBreaking()));
        }

        [Fact]
        public void PeterExhortsTheChurchToRepentanceAndHolyLiving_IsRighteous()
        {
            // The teaching turns to exhortation: Peter calls the faithful to repent, keep purity, and
            // endure in obedience until the Lord's appearing (Qalementos, the closing exhortation).
            // A pastoral summons to repentance, purity, and steadfastness.
            OracleHarness.Righteous(OracleHarness.Witness("Peter exhorts the church to repentance, purity, and patient endurance",
                new Repentance(), new Purity(), new Endurance(), new ObedienceToGod(), new TeachingTruth()));
        }

        [Fact]
        public void TheLordPromisesToJudgeTheWickedAndRewardTheRighteous_IsADivineAct()
        {
            // The book closes with apocalyptic promise: the Lord will come to render just recompense,
            // delivering the righteous and judging the wicked (Qalementos, the eschatological warning).
            // Just judgment and the deliverance of His own is a divine act.
            OracleHarness.DivineAct(OracleHarness.Grounded("the Lord promises just judgment of the wicked and deliverance of the righteous",
                Grounding.InGod(),
                new JustRecompense(), new Righteousness(), new Deliverance(), new PromiseFulfilled(), new Fidelity()));
        }
    }
}
