// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Amos narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class AmosNarrativeTests
    {
        [Fact]
        public void TheLordRoarsFromZionAndJudgesTheNations_IsADivineAct()
        {
            // "The LORD will roar from Zion ... For three transgressions of Damascus, and for four." (Amos 1:2-3)
            // God, the righteous Judge of all peoples, declares just recompense on the surrounding nations.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD roars from Zion and judges the transgressions of the nations",
                new Righteousness(), new JustRecompense(), new TeachingTruth(),
                new RejoicingInTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void AmmonRipsOpenPregnantWomenToEnlargeItsBorder_IsSin()
        {
            // "because they have ripped up the women with child of Gilead, that they might enlarge their border." (Amos 1:13)
            // Ammon sheds innocent blood in greed for territory, an atrocity God will not let pass.
            Resolution r = OracleHarness.Grounded("Ammon rips open the pregnant women of Gilead to enlarge its border",
                Grounding.InIdol("Molech"),
                new Bloodshed(), new Murder(), new Greed(),
                new Oppression(), new Lawlessness());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void MoabBurnsTheBonesOfTheKingOfEdomToLime_IsSin()
        {
            // "because he burned the bones of the king of Edom into lime." (Amos 2:1)
            // Moab desecrates the dead in vengeful hatred, defiling what God says is His to judge.
            OracleHarness.Sin(OracleHarness.Grounded("Moab burns the bones of the king of Edom into lime",
                Grounding.InIdol("Chemosh"),
                new Defilement(), new Hatred(), new Treachery(),
                new Rebellion(), new Insolence()));
        }

        [Fact]
        public void IsraelSellsTheRighteousForSilverAndTheNeedyForShoes_IsSin()
        {
            // "they sold the righteous for silver, and the poor for a pair of shoes." (Amos 2:6-7)
            // Israel monetizes justice, crushing the head of the poor into the dust of the earth.
            Resolution r = OracleHarness.Grounded("Israel sells the righteous for silver and the needy for a pair of shoes",
                Grounding.InIdol("greed for gain"),
                new Oppression(), new Greed(), new WithholdingWhatIsDue(),
                new Heartlessness(), new Lawlessness());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void FatherAndSonGoInToTheSameGirlAndProfaneGodsName_IsSin()
        {
            // "a man and his father will go in unto the same maid, to profane my holy name." (Amos 2:7-8)
            // Sexual defilement and idolatrous feasting on pledged garments profane the holy name of God.
            OracleHarness.Sin(OracleHarness.Grounded("Father and son defile the same girl and feast on pledged garments by every altar",
                Grounding.InIdol("the altars of the high places"),
                new SexualImmorality(), new Defilement(), new Blasphemy(),
                new Idolatry(), new Impurity()));
        }

        [Fact]
        public void GodRemindsIsraelHeBroughtThemUpFromEgypt_IsADivineAct()
        {
            // "I brought you up from the land of Egypt, and led you forty years through the wilderness." (Amos 2:10)
            // God recalls His saving deliverance and faithful provision, the ground of His covenant claim.
            OracleHarness.DivineAct(OracleHarness.Witness("God reminds Israel He delivered them from Egypt and led them through the wilderness",
                new Deliverance(), new CovenantFaithfulness(), new Protection(),
                new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void TheLordRevealsHisSecretToHisServantsTheProphets_IsADivineAct()
        {
            // "Surely the Lord GOD will do nothing, but he revealeth his secret unto his servants the prophets." (Amos 3:7)
            // God graciously discloses His purposes through the prophets, teaching truth before judgment falls.
            OracleHarness.DivineAct(OracleHarness.Witness("The Lord reveals His secret counsel to His servants the prophets",
                new TeachingTruth(), new RejoicingInTruth(), new Fidelity(),
                new CovenantFaithfulness(), new Goodness()));
        }

        [Fact]
        public void TheCowsOfBashanOppressThePoorAndCrushTheNeedy_IsSin()
        {
            // "ye kine of Bashan ... which oppress the poor, which crush the needy." (Amos 4:1)
            // The pampered women of Samaria demand drink while grinding down the poor in luxury and excess.
            OracleHarness.Sin(OracleHarness.Grounded("The cows of Bashan oppress the poor and crush the needy to fund their feasting",
                Grounding.InIdol("Samaria's luxury"),
                new Oppression(), new Greed(), new Drunkenness(),
                new Heartlessness(), new Indifference()));
        }

        [Fact]
        public void AmosCallsIsraelToSeekTheLordAndLive_IsRighteous()
        {
            // "Seek ye me, and ye shall live ... Seek good, and not evil, that ye may live." (Amos 5:4,14)
            // The prophet pleads with Israel to turn from death to life by seeking the LORD and good.
            OracleHarness.Righteous(OracleHarness.Witness("Amos calls Israel to seek the LORD and seek good that they may live",
                new Repentance(), new TeachingTruth(), new HungerForRighteousness(),
                new Worship(), new Goodness()));
        }

        [Fact]
        public void GodCommandsJusticeToRollDownLikeWaters_IsADivineAct()
        {
            // "let judgment run down as waters, and righteousness as a mighty stream." (Amos 5:24)
            // God rejects empty festivals and demands a flood of justice and unending righteousness instead.
            OracleHarness.DivineAct(OracleHarness.Witness("God commands that justice roll down like waters and righteousness like a mighty stream",
                new Righteousness(), new JustRecompense(), new DefendingTheWeak(),
                new TeachingTruth(), new RejoicingInTruth()));
        }

        [Fact]
        public void TheComplacentAtEaseInZionLieOnIvoryBedsAndIgnoreTheRuin_IsSin()
        {
            // "that lie upon beds of ivory ... but they are not grieved for the affliction of Joseph." (Amos 6:1-6)
            // The idle rich indulge in luxury, drink, and music, heedless of the coming ruin of the nation.
            OracleHarness.Sin(OracleHarness.Grounded("Those at ease in Zion lie on ivory beds and drink wine, unmoved by the ruin of Joseph",
                Grounding.InIdol("self-indulgent ease"),
                new Pride(), new Indifference(), new Drunkenness(),
                new Gluttony(), new Sloth()));
        }

        [Fact]
        public void AmosIntercedesAndAsksGodToForgiveJacobWhoIsSmall_IsRighteous()
        {
            // "O Lord GOD, forgive, I beseech thee: by whom shall Jacob arise? for he is small." (Amos 7:2,5)
            // Seeing the locust and fire, the prophet pleads for mercy, and the LORD relents from the judgment.
            OracleHarness.Righteous(OracleHarness.Witness("Amos intercedes for small Jacob and pleads with God to forgive",
                new Compassion(), new HonoringTheVulnerable(), new Humility(),
                new Trust(), new DefendingTheWeak()));
        }

        [Fact]
        public void AmaziahThePriestOpposesTheProphetAndTellsHimToFlee_IsSin()
        {
            // "O thou seer, go, flee thee away ... but prophesy not again any more at Bethel." (Amos 7:10-13)
            // The priest of Bethel slanders Amos to the king and silences God's word for the sake of the shrine.
            OracleHarness.Sin(OracleHarness.Grounded("Amaziah the priest opposes Amos, slanders him to the king, and forbids him to prophesy at Bethel",
                Grounding.InIdol("the king's sanctuary at Bethel"),
                new Slander(), new Rebellion(), new Idolatry(),
                new Pride(), new Disobedience()));
        }

        [Fact]
        public void MerchantsWaitToCheatWithFalseScalesAndSellTheRefuseOfWheat_IsSin()
        {
            // "Making the ephah small, and the shekel great, and falsifying the balances by deceit." (Amos 8:4-6)
            // Greedy traders rig the scales, sell chaff for grain, and buy the poor for silver and sandals.
            OracleHarness.Sin(OracleHarness.Grounded("Merchants falsify the balances by deceit and sell the refuse of wheat to buy the poor",
                Grounding.InIdol("dishonest gain"),
                new Deceit(), new Greed(), new Oppression(),
                new WithholdingWhatIsDue(), new Lawlessness()));
        }

        [Fact]
        public void GodPromisesToRaiseUpTheFallenTabernacleOfDavidAndRestoreHisPeople_IsADivineAct()
        {
            // "In that day will I raise up the tabernacle of David that is fallen ... I will bring again the captivity." (Amos 9:11-15)
            // After judgment, God pledges to rebuild David's fallen house and plant His people never to be uprooted.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises to raise up the fallen tabernacle of David and restore the fortunes of His people",
                new RestoringLife(), new PromiseFulfilled(), new CovenantFaithfulness(),
                new Healing(), new Compassion()));
        }
    }
}
