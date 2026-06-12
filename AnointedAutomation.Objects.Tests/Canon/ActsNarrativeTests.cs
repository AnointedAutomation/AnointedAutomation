// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Acts narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class ActsNarrativeTests
    {
        [Fact]
        public void HolySpiritPouredOutAtPentecost_IsADivineAct()
        {
            // "And they were all filled with the Holy Ghost, and began to speak with other tongues." (Acts 2:1-4)
            // God pours out the promised Spirit on the gathered disciples, fulfilling the word of the prophet.
            OracleHarness.DivineAct(OracleHarness.Witness("God pours out the Holy Spirit upon the disciples at Pentecost",
                new PromiseFulfilled(), new CovenantFaithfulness(), new Deliverance(),
                new Goodness(), new Fidelity()));
        }

        [Fact]
        public void PeterPreachesAndThreeThousandRepent_IsRighteous()
        {
            // "Repent, and be baptized every one of you ... and the same day there were added unto them about three thousand souls." (Acts 2:38-41)
            // Peter proclaims the risen Christ and the crowd turns to God in repentance.
            OracleHarness.Righteous(OracleHarness.Witness("Peter preaches the risen Christ and three thousand repent and are baptized",
                new TeachingTruth(), new Repentance(), new CourageousFaith(),
                new ObedienceToGod(), new Trust()));
        }

        [Fact]
        public void PeterHealsTheLameManAtTheGate_IsADivineAct()
        {
            // "In the name of Jesus Christ of Nazareth rise up and walk ... and immediately his feet and ankle bones received strength." (Acts 3:6-8)
            // Through the apostle, God restores the man lame from birth.
            OracleHarness.DivineAct(OracleHarness.Witness("God heals the man lame from birth through Peter at the Beautiful Gate",
                new Healing(), new Compassion(), new HonoringTheVulnerable(),
                new Goodness(), new Kindness()));
        }

        [Fact]
        public void BelieversShareAllThingsInCommon_IsRighteous()
        {
            // "And all that believed ... had all things common; and sold their possessions ... and parted them to all men, as every man had need." (Acts 2:44-45; 4:32-35)
            // The early church holds nothing back, supplying every need among them.
            OracleHarness.Righteous(OracleHarness.Witness("The believers share all things in common and give to all as any had need",
                new Generosity(), new FeedingTheHungry(), new Kindness(),
                new SelfSacrifice(), new Hospitality()));
        }

        [Fact]
        public void AnaniasAndSapphiraLieToTheHolySpirit_IsSin()
        {
            // "Why hath Satan filled thine heart to lie to the Holy Ghost, and to keep back part of the price?" (Acts 5:1-10)
            // They conspire to deceive the apostles, withholding while pretending full surrender.
            OracleHarness.Sin(OracleHarness.Grounded("Ananias and Sapphira lie to the Holy Spirit and withhold part of the price",
                Grounding.InIdol("Mammon"),
                new Deceit(), new WithholdingWhatIsDue(), new Greed(),
                new FalseWitness(), new Treachery()));
        }

        [Fact]
        public void StephenForgivesHisMurderersAsHeIsStoned_IsRighteous()
        {
            // "Lord, lay not this sin to their charge. And when he had said this, he fell asleep." (Acts 7:54-60)
            // Stephen, full of the Spirit, prays pardon for those killing him.
            OracleHarness.Righteous(OracleHarness.Witness("Stephen forgives those stoning him and commits his spirit to the Lord",
                new Forgiveness(), new Pardon(), new CourageousFaith(),
                new Endurance(), new SelfSacrifice()));
        }

        [Fact]
        public void StephenIsStonedToDeathByTheCouncil_IsSin()
        {
            // "Then they cried out with a loud voice ... and cast him out of the city, and stoned him." (Acts 7:57-58)
            // The council murders the righteous witness in their rage.
            Resolution r = OracleHarness.Witness("The council murders Stephen by stoning in their rage",
                new Murder(), new Bloodshed(), new FitsOfRage(),
                new Hatred(), new WickedScheming());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void SimonTheSorcererTriesToBuyTheSpirit_IsSin()
        {
            // "Thy money perish with thee, because thou hast thought that the gift of God may be purchased with money." (Acts 8:18-23)
            // Simon, who had practiced sorcery, seeks the Spirit's power for selfish gain.
            OracleHarness.Sin(OracleHarness.Grounded("Simon the sorcerer offers money to buy the gift of the Holy Spirit",
                Grounding.InIdol("Mammon"),
                new Sorcery(), new Greed(), new SelfSeeking(),
                new SelfishAmbition(), new Pride()));
        }

        [Fact]
        public void GodCallsSaulAndTurnsThePersecutorToChrist_IsADivineAct()
        {
            // "Saul, Saul, why persecutest thou me? ... it is hard for thee to kick against the pricks." (Acts 9:3-18)
            // The risen Lord arrests the persecutor on the Damascus road and makes him His chosen vessel.
            OracleHarness.DivineAct(OracleHarness.Witness("The risen Christ confronts Saul on the road to Damascus and calls him",
                new Deliverance(), new Pardon(), new Compassion(),
                new RestoringLife(), new Goodness()));
        }

        [Fact]
        public void PeterPreachesAndGodGivesTheSpiritToTheGentiles_IsADivineAct()
        {
            // "While Peter yet spake these words, the Holy Ghost fell on all them which heard the word." (Acts 10:34-44)
            // God shows no partiality and pours out His Spirit on Cornelius and the Gentiles.
            OracleHarness.DivineAct(OracleHarness.Witness("God pours out the Holy Spirit on the Gentiles in the house of Cornelius",
                new PromiseFulfilled(), new CovenantFaithfulness(), new JustRecompense(),
                new Goodness(), new Fidelity()));
        }

        [Fact]
        public void HerodMurdersJamesAndExaltsHimself_IsSin()
        {
            // "And he killed James the brother of John with the sword ... and the people gave a shout, saying, It is the voice of a god." (Acts 12:1-3, 21-23)
            // Herod slays the apostle and accepts worship, refusing to give God the glory.
            Resolution r = OracleHarness.Grounded("Herod murders James with the sword and exalts himself as a god",
                Grounding.InIdol("Herod"),
                new Murder(), new Bloodshed(), new Pride(),
                new Blasphemy(), new Oppression());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void AnAngelDeliversPeterFromPrison_IsADivineAct()
        {
            // "And his chains fell off from his hands ... and they went out, and passed on through one street." (Acts 12:7-11)
            // God sends His angel and frees Peter from Herod's prison the night before his trial.
            OracleHarness.DivineAct(OracleHarness.Witness("God sends an angel to deliver Peter from prison",
                new Deliverance(), new Protection(), new Fidelity(),
                new Goodness(), new CovenantFaithfulness()));
        }

        [Fact]
        public void PaulAndSilasWorshipInPrisonAndKeepTheJailorFromHarm_IsRighteous()
        {
            // "Do thyself no harm: for we are all here ... and he ... washed their stripes; and was baptized." (Acts 16:25-34)
            // Paul and Silas sing hymns to God, then stay to spare the jailor's life and lead him to faith.
            OracleHarness.Righteous(OracleHarness.Witness("Paul and Silas sing praises in prison and stay to save the jailor's life",
                new Worship(), new Joy(), new Protection(),
                new TeachingTruth(), new Kindness()));
        }

        [Fact]
        public void PaulEncouragesAllAboardTheStorm_TossedShip_IsRighteous()
        {
            // "I exhort you to be of good cheer: for there shall be no loss of any man's life among you ... and so it came to pass, that they escaped all safe to land." (Acts 27:21-44)
            // Paul, trusting God's promise, strengthens the despairing crew and they are all brought safely to shore.
            OracleHarness.Righteous(OracleHarness.Witness("Paul encourages the despairing crew in the storm and all reach land safely",
                new CourageousFaith(), new Trust(), new Protection(),
                new HonoringTheVulnerable(), new Endurance()));
        }
    }
}
