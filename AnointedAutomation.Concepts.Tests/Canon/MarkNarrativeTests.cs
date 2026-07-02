// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Mark narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class MarkNarrativeTests
    {
        [Fact]
        public void JohnTheBaptistPreachesRepentanceAndPreparesTheWay_IsRighteous()
        {
            // "John did baptize in the wilderness, and preach the baptism of repentance for the remission of sins." (Mark 1:4-8)
            // The forerunner calls Israel to turn, humbling himself before the One mightier who comes after him.
            OracleHarness.Righteous(OracleHarness.Witness("John the Baptist preaches the baptism of repentance and prepares the way of the Lord",
                new TeachingTruth(), new Repentance(), new Humility(),
                new ObedienceToGod(), new CourageousFaith()));
        }

        [Fact]
        public void JesusCleansesTheLeperWithCompassion_IsADivineAct()
        {
            // "Jesus, moved with compassion, put forth his hand, and touched him ... and immediately the leprosy departed." (Mark 1:40-42)
            // Christ touches the untouchable and restores the outcast to wholeness.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus is moved with compassion and heals the leper, making him clean",
                new Compassion(), new Healing(), new Kindness(),
                new HonoringTheVulnerable(), new Goodness()));
        }

        [Fact]
        public void JesusForgivesAndHealsTheParalytic_IsADivineAct()
        {
            // "Son, thy sins be forgiven thee ... Arise, and take up thy bed, and walk." (Mark 2:5-12)
            // Christ exercises the divine authority to pardon sin and then heals the paralyzed man to prove it.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus forgives the paralytic's sins and heals him so he rises and walks",
                new Forgiveness(), new Pardon(), new Healing(),
                new Compassion(), new Righteousness()));
        }

        [Fact]
        public void JesusHealsOnTheSabbathToDoGoodAndSaveLife_IsADivineAct()
        {
            // "Is it lawful ... to do good on the sabbath days, or to do evil? to save life, or to kill?" (Mark 3:1-5)
            // Christ restores the withered hand, teaching that mercy, not legalism, fulfills the Sabbath.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus heals the man with the withered hand on the Sabbath to do good and save life",
                new Healing(), new Compassion(), new TeachingTruth(),
                new Goodness(), new RestoringLife()));
        }

        [Fact]
        public void JesusStillsTheStormAndCommandsPeaceBeStill_IsADivineAct()
        {
            // "Peace, be still. And the wind ceased, and there was a great calm." (Mark 4:39)
            // Christ rebukes the wind and sea, and the chaos of the deep obeys its Creator.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus rebukes the storm and commands Peace, be still, and the wind and sea obey Him",
                new Protection(), new Deliverance(), new Peace(),
                new CreationOrder(), new Goodness()));
        }

        [Fact]
        public void JesusRaisesJairusDaughterFromTheDead_IsADivineAct()
        {
            // "Talitha cumi ... And straightway the damsel arose, and walked." (Mark 5:41-42)
            // Christ takes the dead girl by the hand and gives her back her life.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus takes the dead girl by the hand and raises Jairus's daughter to life",
                new RestoringLife(), new Compassion(), new Healing(),
                new HonoringTheVulnerable(), new Goodness()));
        }

        [Fact]
        public void HerodiasSchemesAndHerodBeheadsJohnTheBaptist_IsSin()
        {
            // "And he ... beheaded him in the prison ... and gave it to the damsel: and the damsel gave it to her mother." (Mark 6:17-28)
            // Herodias nurses her grudge and through a rash oath Herod murders the righteous prophet.
            OracleHarness.Sin(OracleHarness.Grounded("Herodias schemes against John and Herod has John the Baptist beheaded in prison",
                Grounding.InIdol("Herodias's grudge"),
                new Murder(), new Bloodshed(), new Grudge(),
                new WickedScheming(), new Treachery()));
            // capital vices (Murder, Bloodshed) drive disorder to its limit
            Assert.Equal(1.0, OracleHarness.Grounded("Herodias schemes against John and Herod has John the Baptist beheaded in prison",
                Grounding.InIdol("Herodias's grudge"),
                new Murder(), new Bloodshed(), new Grudge(),
                new WickedScheming(), new Treachery()).Disorder);
        }

        [Fact]
        public void JesusHasCompassionAndFeedsTheFiveThousand_IsADivineAct()
        {
            // "Jesus ... was moved with compassion toward them ... they did all eat, and were filled." (Mark 6:34-44)
            // Seeing the crowd as sheep without a shepherd, Christ teaches them and multiplies the loaves to feed them.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus has compassion on the crowd, teaches them, and feeds the five thousand",
                new Compassion(), new FeedingTheHungry(), new TeachingTruth(),
                new Generosity(), new Goodness()));
        }

        [Fact]
        public void JesusTeachesThatDefilementComesFromTheHeart_IsADivineAct()
        {
            // "That which cometh out of the man, that defileth the man ... evil thoughts, adulteries, fornications, murders." (Mark 7:20-23)
            // Christ reveals that true uncleanness is moral, flowing from the corrupt heart, not from unwashed hands.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus teaches that what comes out of the heart, not unwashed hands, is what defiles a person",
                new TeachingTruth(), new RejoicingInTruth(), new Purity(),
                new Righteousness(), new Goodness()));
        }

        [Fact]
        public void TheRichYoungManTurnsAwayInGreed_IsSin()
        {
            // "He was sad at that saying, and went away grieved: for he had great possessions." (Mark 10:21-22)
            // Called to sell all and follow Christ, the man clings to his wealth and walks away from the Lord.
            OracleHarness.Sin(OracleHarness.Grounded("The rich young man refuses to sell his possessions and turns away from following Jesus",
                Grounding.InIdol("his great possessions"),
                new Greed(), new Covetousness(), new Disobedience(),
                new SelfSeeking(), new Idolatry()));
        }

        [Fact]
        public void JesusServesAsRansomGivingHisLifeForMany_IsADivineAct()
        {
            // "The Son of man came not to be ministered unto, but to minister, and to give his life a ransom for many." (Mark 10:43-45)
            // Christ defines greatness as service and offers His own life as the ransom for the many.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus teaches that He came to serve and to give His life a ransom for many",
                new SelfSacrifice(), new Atonement(), new Humility(),
                new Deliverance(), new Goodness()));
        }

        [Fact]
        public void JudasIscariotBetraysJesusToTheChiefPriests_IsSin()
        {
            // "Judas Iscariot ... went unto the chief priests, to betray him unto them ... and promised to give him money." (Mark 14:10-11)
            // For money Judas hands his Master over to those who seek to kill Him.
            OracleHarness.Sin(OracleHarness.Grounded("Judas Iscariot goes to the chief priests and betrays Jesus to them for money",
                Grounding.InIdol("thirty pieces of silver"),
                new Treachery(), new Greed(), new Deceit(),
                new Disobedience(), new WickedScheming()));
        }

        [Fact]
        public void JesusGivesHisBodyAndBloodAtTheLastSupper_IsADivineAct()
        {
            // "Take, eat: this is my body ... This is my blood of the new testament, which is shed for many." (Mark 14:22-24)
            // Christ institutes the covenant meal, giving His body and blood for the remission of sins.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus gives His body and His blood of the new covenant for many at the Last Supper",
                new SelfSacrifice(), new Atonement(), new CovenantFaithfulness(),
                new PromiseFulfilled(), new Goodness()));
        }

        [Fact]
        public void JesusSubmitsInGethsemaneNotMyWillButThine_IsADivineAct()
        {
            // "Not what I will, but what thou wilt." (Mark 14:36)
            // In the garden Christ entrusts Himself wholly to the Father, embracing the cup of suffering in obedience.
            OracleHarness.DivineAct(OracleHarness.Witness("Jesus prays in Gethsemane, Not my will but thine be done, and submits to the Father",
                new ObedienceToGod(), new SelfSacrifice(), new Humility(),
                new Trust(), new Endurance()));
        }

        [Fact]
        public void PeterDeniesJesusThreeTimes_IsSin()
        {
            // "I know not this man of whom ye speak ... And the second time the cock crew." (Mark 14:66-72)
            // Out of fear Peter swears he does not know his Lord, denying Christ three times before the cock crows.
            OracleHarness.Sin(OracleHarness.Grounded("Peter denies that he knows Jesus three times in the high priest's courtyard",
                Grounding.InIdol("fear of the crowd"),
                new Treachery(), new Deceit(), new FalseWitness(),
                new Disobedience(), new CovenantBreaking()));
        }

        [Fact]
        public void JosephOfArimatheaCourageouslyBuriesTheBodyOfJesus_IsRighteous()
        {
            // "Joseph of Arimathaea ... went in boldly unto Pilate, and craved the body of Jesus ... and laid him in a sepulchre." (Mark 15:43-46)
            // Honorably and at risk to himself, Joseph asks for the Lord's body and lays it reverently in the tomb.
            OracleHarness.Righteous(OracleHarness.Witness("Joseph of Arimathea goes boldly to Pilate, takes the body of Jesus, and lays it in a tomb",
                new CourageousFaith(), new HonoringTheVulnerable(), new Kindness(),
                new Goodness(), new Fidelity()));
        }
    }
}
