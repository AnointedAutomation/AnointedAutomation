// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Hosea narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class HoseaNarrativeTests
    {
        [Fact]
        public void HoseaObeysGodAndTakesGomerAsASignToIsrael_IsRighteous()
        {
            // "Go, take unto thee a wife of whoredoms ... So he went and took Gomer." (Hosea 1:2-3)
            // Hosea obeys a costly command, enacting God's faithful love toward a faithless people.
            OracleHarness.Righteous(OracleHarness.Witness("Hosea obeys God and takes Gomer as a living sign to Israel",
                new ObedienceToGod(), new Fidelity(), new SelfSacrifice(),
                new CourageousFaith(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GodNamesTheChildrenAsSignsOfComingJudgment_IsADivineAct()
        {
            // "Call his name Jezreel ... Lo-ruhamah ... Lo-ammi." (Hosea 1:4-9)
            // God names the children to teach Israel the truth of judgment for her covenant-breaking.
            OracleHarness.DivineAct(OracleHarness.Witness("God names Hosea's children as signs warning Israel of judgment",
                new TeachingTruth(), new Righteousness(), new RejoicingInTruth(),
                new CovenantFaithfulness(), new JustRecompense()));
        }

        [Fact]
        public void IsraelPlaysTheHarlotAndForsakesTheLordForBaals_IsSin()
        {
            // "She went after her lovers, and forgat me, saith the LORD." (Hosea 2:5-13)
            // Israel commits spiritual adultery, attributing her gifts to Baal and forsaking her God.
            Resolution r = OracleHarness.Grounded("Israel plays the harlot and runs after the Baals, forsaking the LORD",
                Grounding.InIdol("Baal"),
                new Idolatry(), new Adultery(), new CovenantBreaking(),
                new Ingratitude(), new Treachery());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void GodWoosFaithlessIsraelBackIntoTheWilderness_IsADivineAct()
        {
            // "Therefore ... I will allure her ... and speak comfortably unto her." (Hosea 2:14-15)
            // Rather than abandon her, God tenderly draws His unfaithful people back to Himself.
            OracleHarness.DivineAct(OracleHarness.Witness("God allures faithless Israel into the wilderness to win her back",
                new Compassion(), new Kindness(), new CovenantFaithfulness(),
                new Forgiveness(), new RestoringLife()));
        }

        [Fact]
        public void GodBetrothsIsraelToHimselfForeverInRighteousness_IsADivineAct()
        {
            // "I will betroth thee unto me for ever ... in righteousness ... in lovingkindness, and in mercies." (Hosea 2:19-20)
            // God pledges an everlasting covenant of faithful, righteous, merciful love.
            OracleHarness.DivineAct(OracleHarness.Witness("God betroths Israel to Himself forever in righteousness and mercy",
                new CovenantFaithfulness(), new Righteousness(), new Compassion(),
                new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void HoseaRedeemsAndLovesBackHisAdulterousWife_IsRighteous()
        {
            // "Go yet, love a woman ... So I bought her to me for fifteen pieces of silver." (Hosea 3:1-3)
            // Hosea buys back and loves his estranged wife, picturing God's redeeming love for Israel.
            OracleHarness.Righteous(OracleHarness.Witness("Hosea redeems Gomer and loves her again despite her adultery",
                new Forgiveness(), new SelfSacrifice(), new Atonement(),
                new CovenantFaithfulness(), new Kindness()));
        }

        [Fact]
        public void IsraelIsChargedWithBloodshedDeceitAndNoKnowledgeOfGod_IsSin()
        {
            // "By swearing, and lying ... they break out, and blood toucheth blood." (Hosea 4:1-2)
            // God's controversy with the land: no truth, no mercy, no knowledge of God, only bloodshed.
            Resolution r = OracleHarness.Grounded("Israel fills the land with bloodshed, lying, and theft, forsaking the knowledge of God",
                Grounding.InIdol("Israel's lovers"),
                new Bloodshed(), new Deceit(), new Theft(),
                new Lawlessness(), new CovenantBreaking());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void TheProstitutingPeopleSacrificeOnTheHilltopsToIdols_IsSin()
        {
            // "They sacrifice upon the tops of the mountains ... thy daughters shall commit whoredom." (Hosea 4:12-14)
            // A spirit of whoredoms leads Israel into idolatrous sacrifice and ritual immorality.
            OracleHarness.Sin(OracleHarness.Grounded("Israel sacrifices on the hilltops and gives herself to idolatrous whoredom",
                Grounding.InIdol("idols of the high places"),
                new Idolatry(), new SexualImmorality(), new Defilement(),
                new Rebellion(), new Disobedience()));
        }

        [Fact]
        public void GodDesiresMercyAndKnowledgeOfHimRatherThanSacrifice_IsADivineAct()
        {
            // "For I desired mercy, and not sacrifice; and the knowledge of God more than burnt offerings." (Hosea 6:6)
            // God reveals the heart of true religion: steadfast love and knowing Him above mere ritual.
            OracleHarness.DivineAct(OracleHarness.Witness("God declares He desires mercy and knowledge of Him rather than sacrifice",
                new Compassion(), new TeachingTruth(), new Righteousness(),
                new Worship(), new RejoicingInTruth()));
        }

        [Fact]
        public void IsraelMakesKingsAndIdolsWithoutGodAndSowsTheWind_IsSin()
        {
            // "They have set up kings, but not by me ... of their silver and gold have they made them idols." (Hosea 8:4-7)
            // Israel exalts herself, crowns kings of her own will, and casts idols, sowing the wind.
            Resolution r = OracleHarness.Grounded("Israel sets up kings without God and makes idols of her silver and gold",
                Grounding.InIdol("the calf of Samaria"),
                new Idolatry(), new GravenImage(), new Rebellion(),
                new Pride(), new Disobedience());
            OracleHarness.Sin(r);
        }

        [Fact]
        public void HoseaPleadsWithIsraelToReturnAndSowRighteousness_IsRighteous()
        {
            // "Sow to yourselves in righteousness ... seek the LORD, till he come and rain righteousness." (Hosea 10:12)
            // The prophet calls the people to repentance, to break up fallow ground and seek the LORD.
            OracleHarness.Righteous(OracleHarness.Witness("Hosea pleads with Israel to sow righteousness and seek the LORD",
                new Repentance(), new TeachingTruth(), new HungerForRighteousness(),
                new Worship(), new Fidelity()));
        }

        [Fact]
        public void GodRecallsHisFatherlyLoveAndRefusesToGiveUpEphraim_IsADivineAct()
        {
            // "When Israel was a child, then I loved him ... How shall I give thee up, Ephraim?" (Hosea 11:1-8)
            // God's heart recoils within Him; His compassion overrides wrath toward His wayward son.
            OracleHarness.DivineAct(OracleHarness.Witness("God recalls His fatherly love and refuses to give up Ephraim",
                new Compassion(), new Kindness(), new CovenantFaithfulness(),
                new HonoringTheVulnerable(), new Forgiveness()));
        }

        [Fact]
        public void HoseaCallsIsraelToReturnConfessAndReceiveHealing_IsRighteous()
        {
            // "O Israel, return unto the LORD thy God ... Take with you words, and turn to the LORD." (Hosea 14:1-2)
            // The closing call: repentance and confession, that God may heal their backsliding.
            OracleHarness.Righteous(OracleHarness.Witness("Hosea calls Israel to return to the LORD with words of confession",
                new Repentance(), new Humility(), new Worship(),
                new TeachingTruth(), new Trust()));
        }

        [Fact]
        public void GodPromisesToHealBacksliddenIsraelAndLoveHerFreely_IsADivineAct()
        {
            // "I will heal their backsliding, I will love them freely ... I will be as the dew unto Israel." (Hosea 14:4-7)
            // God answers repentance with free, healing love and the promise of renewed, ordered life.
            OracleHarness.DivineAct(OracleHarness.Witness("God promises to heal Israel's backsliding and love her freely",
                new Healing(), new Forgiveness(), new RestoringLife(),
                new Compassion(), new PromiseFulfilled()));
        }
    }
}
