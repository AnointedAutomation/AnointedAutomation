// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Micah narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class MicahNarrativeTests
    {
        [Fact]
        public void TheLordComesDownToWitnessAgainstSamariaAndJerusalem_IsADivineAct()
        {
            // "the Lord GOD be witness against you ... behold, the LORD cometh forth out of his place." (Micah 1:2-3)
            // God descends as righteous Witness and Judge against the high places of His people's sin.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD comes down from His holy temple to witness against Samaria and Jerusalem",
                new Righteousness(), new JustRecompense(), new TeachingTruth(),
                new RejoicingInTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void SamariasHarlotryAndIdolWorshipAreCondemned_IsSin()
        {
            // "all the graven images thereof shall be beaten to pieces ... she gathered it of the hire of an harlot." (Micah 1:6-7)
            // Samaria built her shrines from the wages of idolatrous harlotry, the very sin that brings her ruin.
            OracleHarness.Sin(OracleHarness.Grounded("Samaria heaps up graven images and wages of harlotry in idol worship",
                Grounding.InIdol("Baal"),
                new Idolatry(), new GravenImage(), new SexualImmorality(),
                new CovenantBreaking(), new Rebellion()));
        }

        [Fact]
        public void TheGreedyDeviseEvilOnTheirBedsAndSeizeFieldsAndHouses_IsSin()
        {
            // "Woe to them that devise iniquity ... they covet fields, and take them by violence; and houses, and take them away." (Micah 2:1-2)
            // On their beds the powerful scheme to steal the inheritance of the poor and oppress a man and his house.
            OracleHarness.Sin(OracleHarness.Grounded("The powerful devise iniquity on their beds and seize the fields and houses of the poor",
                Grounding.InIdol("their own power"),
                new WickedScheming(), new Covetousness(), new Theft(),
                new Oppression(), new Greed()));
        }

        [Fact]
        public void FalseProphetsCryPeaceForBreadAndWarOnThoseWhoFeedThemNot_IsSin()
        {
            // "the prophets that make my people err ... he that putteth not into their mouths, they even prepare war against him." (Micah 3:5)
            // The hireling prophets bless those who feed them and curse the rest, deceiving the people for gain.
            OracleHarness.Sin(OracleHarness.Grounded("The false prophets cry peace for bread and declare war on those who do not fill their mouths",
                Grounding.InIdol("dishonest gain"),
                new Deceit(), new Greed(), new FalseWitness(),
                new Treachery(), new Lawlessness()));
        }

        [Fact]
        public void TheRulersHateGoodLoveEvilAndTearTheFleshOfThePeople_IsSin()
        {
            // "Who hate the good, and love the evil; who pluck off their skin ... and eat the flesh of my people." (Micah 3:1-3)
            // The heads of Jacob who should know justice instead devour the people like meat in a pot.
            OracleHarness.Sin(OracleHarness.Grounded("The rulers hate good and love evil, tearing the skin and flesh off the people",
                Grounding.InIdol("their own appetite"),
                new Oppression(), new Bloodshed(), new Hatred(),
                new DelightingInEvil(), new Heartlessness()));
            // capital vice (Bloodshed) drives disorder to its limit
        }

        [Fact]
        public void ZionsHeadsJudgeForRewardAndPriestsTeachForHire_IsSin()
        {
            // "the heads thereof judge for reward, and the priests thereof teach for hire ... yet will they lean upon the LORD." (Micah 3:11)
            // Bribed judges, mercenary priests, and money-prophets presume on God while building Zion with blood.
            OracleHarness.Sin(OracleHarness.Grounded("Zion's heads judge for reward and her priests teach for hire while presuming on the LORD",
                Grounding.InIdol("bribes and hire"),
                new Greed(), new Deceit(), new Pride(),
                new FalseWitness(), new Blasphemy()));
        }

        [Fact]
        public void ManyNationsShallFlowToTheMountainOfTheLordToLearnHisWays_IsADivineAct()
        {
            // "many nations shall come, and say, Come, and let us go up to the mountain of the LORD ... he will teach us of his ways." (Micah 4:1-2)
            // In the last days God draws the peoples to Zion, teaching His ways so they walk in His paths.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD draws many nations to His mountain to teach them His ways and judge among them",
                new TeachingTruth(), new Peacemaking(), new Righteousness(),
                new Peace(), new Worship()));
        }

        [Fact]
        public void TheNationsBeatSwordsIntoPlowsharesAndLearnWarNoMore_IsADivineAct()
        {
            // "they shall beat their swords into plowshares ... neither shall they learn war any more." (Micah 4:3)
            // God's reign turns weapons into farm tools and gives every man rest under his own vine and fig tree.
            OracleHarness.DivineAct(OracleHarness.Witness("Under God's reign the nations beat swords into plowshares and learn war no more",
                new Peacemaking(), new Peace(), new Protection(),
                new Goodness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheRulerOfIsraelShallComeForthFromBethlehem_IsADivineAct()
        {
            // "out of thee shall he come forth ... that is to be ruler in Israel; whose goings forth have been ... from everlasting." (Micah 5:2)
            // The promised Shepherd-King comes from little Bethlehem, the eternal Ruler who fulfills God's word.
            OracleHarness.DivineAct(OracleHarness.Witness("Out of Bethlehem comes forth the eternal Ruler of Israel, the promised Messiah",
                new PromiseFulfilled(), new CovenantFaithfulness(), new Fidelity(),
                new Deliverance(), new Goodness()));
        }

        [Fact]
        public void TheRulerShallStandAndFeedHisFlockInTheStrengthOfTheLord_IsADivineAct()
        {
            // "he shall stand and feed in the strength of the LORD ... and this man shall be the peace." (Micah 5:4-5)
            // The coming Ruler shepherds His flock in God's might, securing them, and He Himself is their peace.
            OracleHarness.DivineAct(OracleHarness.Witness("The Ruler stands and feeds His flock in the strength of the LORD and is their peace",
                new Protection(), new Peace(), new CovenantFaithfulness(),
                new Fidelity(), new Goodness()));
        }

        [Fact]
        public void GodRecallsHowHeRedeemedIsraelFromEgyptAndSentMosesAaronAndMiriam_IsADivineAct()
        {
            // "I brought thee up out of the land of Egypt, and redeemed thee ... I sent before thee Moses, Aaron, and Miriam." (Micah 6:4)
            // God pleads His case by recalling the redemption from Egypt and the leaders He gave in covenant love.
            OracleHarness.DivineAct(OracleHarness.Witness("God recalls how He redeemed Israel out of Egypt and sent Moses, Aaron, and Miriam before them",
                new Deliverance(), new CovenantFaithfulness(), new PromiseFulfilled(),
                new Protection(), new Fidelity()));
        }

        [Fact]
        public void GodRequiresToDoJustlyLoveMercyAndWalkHumblyWithHim_IsADivineAct()
        {
            // "what doth the LORD require of thee, but to do justly, and to love mercy, and to walk humbly with thy God?" (Micah 6:8)
            // Above sacrifices God teaches the heart of His will: justice, steadfast mercy, and humble walking.
            OracleHarness.DivineAct(OracleHarness.Witness("The LORD requires of His people to do justly, to love mercy, and to walk humbly with their God",
                new Righteousness(), new Compassion(), new Humility(),
                new TeachingTruth(), new CovenantFaithfulness()));
        }

        [Fact]
        public void TheCityUsesWickedScalesDeceitfulWeightsAndViolence_IsSin()
        {
            // "the bag of deceitful weights ... the rich men thereof are full of violence ... they have spoken lies." (Micah 6:11-12)
            // The merchants cheat with rigged scales, the rich live by violence, and every tongue is full of deceit.
            OracleHarness.Sin(OracleHarness.Grounded("The city's merchants use wicked balances and deceitful weights while the rich are full of violence and lies",
                Grounding.InIdol("dishonest gain"),
                new Deceit(), new Greed(), new Oppression(),
                new FalseWitness(), new Lawlessness()));
        }

        [Fact]
        public void TheGodlyHavePerishedAndEveryManHuntsHisBrotherWithANet_IsSin()
        {
            // "The good man is perished out of the earth ... they hunt every man his brother with a net." (Micah 7:2)
            // Faithfulness has vanished from the land; men lie in wait for blood and snare even their own kin.
            OracleHarness.Sin(OracleHarness.Grounded("The godly have perished and every man hunts his brother with a net to shed blood",
                Grounding.InIdol("self-seeking violence"),
                new Bloodshed(), new Treachery(), new Malice(),
                new Deceit(), new Hatred()));
            // capital vice (Bloodshed) drives disorder to its limit
        }

        [Fact]
        public void MicahWaitsForGodHisSaviorWhoWillBringHimToTheLight_IsRighteous()
        {
            // "I will look unto the LORD; I will wait for the God of my salvation: my God will hear me." (Micah 7:7)
            // Amid the darkness the prophet trusts and waits for God, confident his Savior will bring him to light.
            OracleHarness.Righteous(OracleHarness.Witness("Micah waits in trust for the LORD, the God of his salvation, who will hear him and bring him to the light",
                new Trust(), new Endurance(), new Worship(),
                new CourageousFaith(), new Patience()));
        }

        [Fact]
        public void GodPardonsIniquityAndCastsAllSinsIntoTheDepthsOfTheSea_IsADivineAct()
        {
            // "Who is a God like unto thee, that pardoneth iniquity ... he will subdue our iniquities; ... cast all their sins into the depths of the sea." (Micah 7:18-19)
            // Delighting in mercy, God forgives His people's sin and drowns their iniquities in the sea forever.
            OracleHarness.DivineAct(OracleHarness.Witness("God pardons iniquity, delights in mercy, and casts all the sins of His people into the depths of the sea",
                new Pardon(), new Forgiveness(), new Compassion(),
                new CovenantFaithfulness(), new Fidelity()));
        }
    }
}
