// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // 1 & 2 Samuel narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class SamuelNarrativeTests
    {
        [Fact]
        public void HannahVowsAndPraysToTheLordForASon_IsRighteous()
        {
            // "In her deep anguish Hannah prayed to the LORD... 'I will give him to the LORD for all the days of his life.'" (1 Samuel 1:10-11)
            OracleHarness.Righteous(OracleHarness.Witness("Hannah pours out her soul before the LORD and vows to give her son back to Him", new Worship(), new Trust(), new SelfSacrifice()));
        }

        [Fact]
        public void GodRemembersHannahAndGivesHerSamuel_IsADivineAct()
        {
            // "So in the course of time Hannah became pregnant and gave birth to a son... 'Because I asked the LORD for him.'" (1 Samuel 1:19-20)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD remembers Hannah and gives her a son, Samuel, in answer to her prayer", new Compassion(), new PromiseFulfilled(), new RestoringLife()));
        }

        [Fact]
        public void EliSonsAbuseTheOfferingsAndLieWithTheServingWomen_IsSin()
        {
            // "This sin of the young men was very great in the LORD's sight... they were treating the LORD's offering with contempt." (1 Samuel 2:12-22)
            OracleHarness.Sin(OracleHarness.Witness("Hophni and Phinehas steal the LORD's offerings by force and defile the women serving at the tent of meeting", new Theft(), new SexualImmorality(), new Blasphemy(), new Greed()));
        }

        [Fact]
        public void GodCallsTheBoySamuelAndRevealsHisWord_IsADivineAct()
        {
            // "The LORD came and stood there, calling as at the other times, 'Samuel! Samuel!'" (1 Samuel 3:10-21)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD calls the boy Samuel and reveals His word, letting none of it fall to the ground", new TeachingTruth(), new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void SamuelDeclaresTheWholeWordOfGodToEli_IsRighteous()
        {
            // "So Samuel told him everything, hiding nothing from him." (1 Samuel 3:18)
            OracleHarness.Righteous(OracleHarness.Witness("Samuel faithfully tells Eli the LORD's whole word, hiding nothing", new TeachingTruth(), new Trustworthiness(), new ObedienceToGod()));
        }

        [Fact]
        public void GodStrikesDagonAndAfflictsThePhilistinesForTheArk_IsADivineAct()
        {
            // "There was Dagon, fallen on his face... his head and hands had been broken off." (1 Samuel 5:1-6)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD topples Dagon before His ark and afflicts the Philistines until they return it", new JustRecompense(), new Deliverance(), new Righteousness()));
        }

        [Fact]
        public void IsraelRepentsAtMizpahAndSamuelIntercedes_IsRighteous()
        {
            // "They... fasted and there confessed, 'We have sinned against the LORD.' ... Samuel cried out to the LORD on Israel's behalf." (1 Samuel 7:5-9)
            OracleHarness.Righteous(OracleHarness.Witness("Israel repents and confesses its sin at Mizpah while Samuel intercedes for them before the LORD", new Repentance(), new Worship(), new ObedienceToGod()));
        }

        [Fact]
        public void GodAnointsAndDeliversByDavidAgainstGoliath_IsADivineAct()
        {
            // "The LORD who rescued me from the paw of the lion... will rescue me from the hand of this Philistine." (1 Samuel 17:37, 45-50)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD delivers Goliath into David's hand so all may know there is a God in Israel", new Deliverance(), new Protection(), new PromiseFulfilled()));
        }

        [Fact]
        public void DavidFightsGoliathInTheNameOfTheLord_IsRighteous()
        {
            // "You come against me with sword and spear... but I come against you in the name of the LORD Almighty." (1 Samuel 17:45)
            OracleHarness.Righteous(OracleHarness.Witness("David goes out against Goliath in courageous faith, trusting the LORD to defend His people", new CourageousFaith(), new Trust(), new DefendingTheWeak()));
        }

        [Fact]
        public void SaulHurlsHisSpearAtDavidInJealousRage_IsSin()
        {
            // "Saul was afraid of David... and Saul tried to pin him to the wall with his spear." (1 Samuel 18:10-11, 28-29)
            OracleHarness.Sin(OracleHarness.Witness("Saul, eaten with jealousy, hurls his spear to murder David and hunts him through the wilderness", new Jealousy(), new Murder(), new Bloodshed(), new Hatred()));
        }

        [Fact]
        public void DavidSparesSaulInTheCaveRefusingToHarmTheLordsAnointed_IsRighteous()
        {
            // "The LORD forbid that I should... lay my hand on him; for he is the anointed of the LORD." (1 Samuel 24:6-12)
            OracleHarness.Righteous(OracleHarness.Witness("David spares Saul's life in the cave, refusing to lift his hand against the LORD's anointed", new Forgiveness(), new SelfControl(), new ObedienceToGod(), new Patience()));
        }

        [Fact]
        public void SaulConsultsTheMediumOfEndor_IsSin()
        {
            // "Saul then said to his attendants, 'Find me a woman who is a medium, so I may go and inquire of her.'" (1 Samuel 28:7-19)
            OracleHarness.Sin(OracleHarness.Grounded("Saul forsakes the LORD and consults the medium of Endor to call up the dead", Grounding.InIdol("Medium of Endor"), new Sorcery(), new Disobedience(), new Rebellion()));
        }

        [Fact]
        public void DavidMournsSaulAndJonathanInsteadOfRejoicing_IsRighteous()
        {
            // "David took up this lament concerning Saul and his son Jonathan... 'How the mighty have fallen!'" (2 Samuel 1:17-27)
            OracleHarness.Righteous(OracleHarness.Witness("David mourns and laments Saul and Jonathan rather than gloating over his enemy's fall", new Mourning(), new Kindness(), new Forgiveness()));
        }

        [Fact]
        public void GodCovenantsToEstablishDavidsThroneForever_IsADivineAct()
        {
            // "Your house and your kingdom will endure forever before me; your throne will be established forever." (2 Samuel 7:11-16)
            OracleHarness.DivineAct(OracleHarness.Witness("the LORD makes covenant with David to establish his throne and offspring forever", new CovenantFaithfulness(), new PromiseFulfilled(), new Fidelity()));
        }

        [Fact]
        public void DavidShowsKindnessToMephiboshethForJonathansSake_IsRighteous()
        {
            // "I will surely show you kindness for the sake of your father Jonathan... and you will always eat at my table." (2 Samuel 9:1-13)
            OracleHarness.Righteous(OracleHarness.Witness("David seeks out lame Mephibosheth and restores his land and a seat at the king's table for Jonathan's sake", new Kindness(), new CovenantFaithfulness(), new HonoringTheVulnerable(), new Generosity()));
        }

        [Fact]
        public void DavidTakesBathshebaAndHasUriahKilled_IsSin()
        {
            // "David committed adultery with Bathsheba... and Uriah the Hittite is struck down." (2 Samuel 11:2-17)
            Resolution r = OracleHarness.Witness("David lies with Bathsheba and then has Uriah killed at the battle front to cover it", new Adultery(), new Murder(), new Bloodshed(), new Deceit());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void DavidConfessesAndRepentsAtNathansRebuke_IsRighteous()
        {
            // "Then David said to Nathan, 'I have sinned against the LORD.'" (2 Samuel 12:13)
            OracleHarness.Righteous(OracleHarness.Witness("David confesses his sin before Nathan and humbles himself before the LORD", new Repentance(), new Humility(), new RejoicingInTruth()));
        }

        [Fact]
        public void AbsalomRebelsAgainstHisFatherDavid_IsSin()
        {
            // "Absalom... stole the hearts of the people of Israel" and rose to seize his father's throne. (2 Samuel 15:1-14, 18:14)
            OracleHarness.Sin(OracleHarness.Witness("Absalom conspires, steals the hearts of Israel, and rises in rebellion to overthrow his father David", new Rebellion(), new Treachery(), new Deceit(), new SelfishAmbition()));
        }
    }
}
