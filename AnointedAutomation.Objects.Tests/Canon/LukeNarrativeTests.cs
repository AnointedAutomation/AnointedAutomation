// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    // Luke narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class LukeNarrativeTests
    {
        [Fact]
        public void GabrielAnnouncesJohnsBirthToZechariah_IsADivineAct()
        {
            // "Your prayer has been heard, and your wife Elizabeth will bear you a son." (Luke 1:13-17)
            // God answers the barren couple and prepares a forerunner for the Lord.
            OracleHarness.DivineAct(OracleHarness.Witness("Gabriel announces the birth of John to Zechariah",
                new PromiseFulfilled(), new RestoringLife(), new Fidelity(), new Goodness()));
        }

        [Fact]
        public void MaryConsentsToTheAnnunciation_IsRighteous()
        {
            // "Behold, I am the servant of the Lord; let it be to me according to your word." (Luke 1:38)
            // Mary's humble, trusting, obedient submission to God's call.
            OracleHarness.Righteous(OracleHarness.Witness("Mary submits to the angel's word at the Annunciation",
                new Humility(), new ObedienceToGod(), new Trust(), new CourageousFaith(), new Fidelity()));
        }

        [Fact]
        public void TheIncarnationOfChrist_IsADivineAct()
        {
            // "The Holy Spirit will come upon you ... the child to be born will be called holy." (Luke 1:35; 2:7)
            // God takes flesh to dwell with and save His people.
            OracleHarness.DivineAct(OracleHarness.Witness("The Son of God is conceived and born of Mary",
                new SelfSacrifice(), new CovenantFaithfulness(), new PromiseFulfilled(),
                new Deliverance(), new Goodness(), new Purity()));
        }

        [Fact]
        public void MaryMagnifiesTheLordInTheMagnificat_IsRighteous()
        {
            // "My soul magnifies the Lord ... he has regarded the low estate of his servant." (Luke 1:46-55)
            // Mary worships God who lifts the lowly and fills the hungry.
            OracleHarness.Righteous(OracleHarness.Witness("Mary sings the Magnificat in praise of God",
                new Worship(), new Joy(), new Humility(), new RejoicingInTruth(), new Fidelity()));
        }

        [Fact]
        public void GoodSamaritanCaresForTheWoundedTraveler_IsRighteous()
        {
            // "He had compassion ... bound up his wounds ... took care of him." (Luke 10:33-35)
            // The neighbor who shows mercy across enmity, at his own cost.
            OracleHarness.Righteous(OracleHarness.Witness("The Good Samaritan binds the wounds of the beaten man",
                new Compassion(), new Kindness(), new CaringForTheSick(),
                new Generosity(), new SelfSacrifice(), new HonoringTheVulnerable()));
        }

        [Fact]
        public void PriestAndLevitePassByTheBeatenMan_IsSin()
        {
            // "A priest ... saw him ... passed by on the other side. So likewise a Levite." (Luke 10:31-32)
            // Religious men withhold mercy from a dying neighbor.
            OracleHarness.Sin(OracleHarness.Witness("The priest and Levite pass by the dying man",
                new Indifference(), new Heartlessness(), new WithholdingWhatIsDue()));
        }

        [Fact]
        public void FatherRunsToWelcomeHomeTheProdigalSon_IsRighteous()
        {
            // "While he was still a long way off, his father ... ran and embraced him." (Luke 15:20-24)
            // The father pardons the wayward son and rejoices over his return.
            OracleHarness.Righteous(OracleHarness.Witness("The father welcomes home the prodigal son",
                new Forgiveness(), new Compassion(), new Pardon(), new Joy(), new Generosity()));
        }

        [Fact]
        public void ProdigalSonSquandersAndRepents_IsRighteous()
        {
            // "I will arise and go to my father, and I will say ... I have sinned." (Luke 15:18-21)
            // Coming to himself, the son turns back in humble repentance.
            OracleHarness.Righteous(OracleHarness.Witness("The prodigal son returns in repentance",
                new Repentance(), new Humility(), new PovertyOfSpirit()));
        }

        [Fact]
        public void ZacchaeusRepentsAndRestoresFourfold_IsRighteous()
        {
            // "Half of my goods I give to the poor ... I restore it fourfold." (Luke 19:8-9)
            // The chief tax collector repents, gives generously, and makes just restitution.
            OracleHarness.Righteous(OracleHarness.Witness("Zacchaeus repents and restores what he took",
                new Repentance(), new Generosity(), new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void ChristHealsTenLepersAndOneReturnsGivingThanks_IsADivineAct()
        {
            // "As they went they were cleansed ... one of them ... turned back, praising God." (Luke 17:12-19)
            // Christ cleanses the ten lepers by His healing word.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ heals the ten lepers",
                new Healing(), new Compassion(), new HonoringTheVulnerable(), new Goodness()));
        }

        [Fact]
        public void ChristForgivesFromTheCross_IsADivineAct()
        {
            // "Father, forgive them, for they know not what they do." (Luke 23:34)
            // The crucified Christ pardons His executioners and offers Himself for sinners.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ forgives His crucifiers from the cross",
                new Forgiveness(), new Pardon(), new SelfSacrifice(), new Atonement(), new Compassion()));
        }

        [Fact]
        public void ChristPromisesParadiseToThePenitentThief_IsADivineAct()
        {
            // "Truly, I say to you, today you will be with me in Paradise." (Luke 23:43)
            // Christ pardons the dying thief who confesses Him.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ promises Paradise to the penitent thief",
                new Pardon(), new Forgiveness(), new Deliverance(), new PromiseFulfilled(), new Compassion()));
        }

        [Fact]
        public void JudasBetraysJesusForMoney_IsSin()
        {
            // "Then Satan entered into Judas ... he ... conferred ... how he might betray him." (Luke 22:3-6)
            // Judas sells the innocent Christ to His enemies for silver.
            OracleHarness.Sin(OracleHarness.Witness("Judas agrees to betray Jesus for money",
                new Treachery(), new Greed(), new Deceit(), new CovenantBreaking()));
        }

        [Fact]
        public void PeterDeniesChristThreeTimes_IsSin()
        {
            // "Man, I do not know what you are talking about ... and the cock crowed." (Luke 22:57-61)
            // Peter denies his Lord out of fear, then weeps bitterly.
            OracleHarness.Sin(OracleHarness.Witness("Peter denies Jesus three times in the courtyard",
                new Deceit(), new FalseWitness(), new CovenantBreaking()));
        }

        [Fact]
        public void TheResurrectionOfChrist_IsADivineAct()
        {
            // "He is not here, but has risen." (Luke 24:5-6)
            // God raises the crucified Christ, conquering death for His people.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ is raised from the dead",
                new RestoringLife(), new Deliverance(), new PromiseFulfilled(),
                new CovenantFaithfulness(), new Goodness()));
        }
    }
}
