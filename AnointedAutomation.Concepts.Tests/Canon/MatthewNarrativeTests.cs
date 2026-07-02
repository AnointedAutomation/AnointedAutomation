// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Matthew narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class MatthewNarrativeTests
    {
        [Fact]
        public void GodIsWithUsInTheBirthOfChrist_IsADivineAct()
        {
            // "Behold, the virgin shall conceive ... and they shall call his name Immanuel, God with us." (Matthew 1:18-23)
            // The Holy Spirit conceives Christ; the promise to the house of David is fulfilled.
            OracleHarness.DivineAct(OracleHarness.Witness("The Holy Spirit conceives Christ in the virgin (Matthew 1:18-23)",
                new CreationOrder(), new PromiseFulfilled(), new CovenantFaithfulness(), new Deliverance(), new Goodness()));
        }

        [Fact]
        public void JosephTakesMaryAsWifeInObedience_IsRighteous()
        {
            // Joseph, a just man, obeys the angel and takes Mary, sparing her shame. (Matthew 1:19-24)
            // Quiet obedience to God and protective compassion for the vulnerable.
            OracleHarness.Righteous(OracleHarness.Witness("Joseph obeys the angel and takes Mary as his wife (Matthew 1:24)",
                new ObedienceToGod(), new Righteousness(), new Protection(), new Compassion(), new Trust()));
        }

        [Fact]
        public void HerodSlaughtersTheInnocentsOfBethlehem_IsSin()
        {
            // "Herod ... slew all the children that were in Bethlehem ... from two years old and under." (Matthew 2:16)
            // Murderous bloodshed of infants out of jealous rage. Capital vice: disorder is total.
            Resolution r = OracleHarness.Witness("Herod massacres the infants of Bethlehem (Matthew 2:16)",
                new Murder(), new Bloodshed(), new Jealousy(), new FitsOfRage(), new Oppression());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void JohnBaptizesCallingForRepentance_IsRighteous()
        {
            // "Repent ye: for the kingdom of heaven is at hand." (Matthew 3:1-6)
            // John preaches truth and a baptism of repentance, preparing the way of the Lord.
            OracleHarness.Righteous(OracleHarness.Witness("John the Baptist preaches repentance in the wilderness (Matthew 3:1-6)",
                new TeachingTruth(), new Repentance(), new Righteousness(), new ObedienceToGod()));
        }

        [Fact]
        public void ChristResistsTheTempterInTheWilderness_IsADivineAct()
        {
            // Christ answers Satan with "It is written," refusing bread, presumption, and idolatrous power. (Matthew 4:1-11)
            // Perfect obedience, self-control, and worship of God alone.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ overcomes the temptations in the wilderness (Matthew 4:1-11)",
                new ObedienceToGod(), new SelfControl(), new Worship(), new Endurance(), new CourageousFaith()));
        }

        [Fact]
        public void ChristPreachesTheBeatitudes_IsADivineAct()
        {
            // "Blessed are the poor in spirit ... they that mourn ... the meek ... they which hunger ... for righteousness." (Matthew 5:3-10)
            // Christ teaches the very shape of kingdom blessedness.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ teaches the Beatitudes on the mount (Matthew 5:3-10)",
                new TeachingTruth(), new PovertyOfSpirit(), new Mourning(), new Meekness(),
                new HungerForRighteousness(), new Purity(), new Peacemaking()));
        }

        [Fact]
        public void ChristTeachesLoveOfEnemiesAndForgiveness_IsADivineAct()
        {
            // "Love your enemies ... if ye forgive men their trespasses, your heavenly Father will also forgive you." (Matthew 5:44; 6:14)
            // The Lord commands enemy-love, mercy, and the forgiveness of debts.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ teaches love of enemies and forgiveness (Matthew 5:44; 6:14)",
                new TeachingTruth(), new Forgiveness(), new Kindness(), new Peacemaking(), new Compassion()));
        }

        [Fact]
        public void ChristHealsTheLeperAndTheSick_IsADivineAct()
        {
            // Christ cleanses the leper, heals the centurion's servant and Peter's mother-in-law, and "healed all that were sick." (Matthew 8:1-17)
            // Divine compassion that restores the broken body.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ heals the leper and the sick (Matthew 8:1-17)",
                new Healing(), new Compassion(), new CaringForTheSick(), new Goodness(), new Deliverance()));
        }

        [Fact]
        public void ChristStillsTheStormOverChaos_IsADivineAct()
        {
            // "He arose, and rebuked the winds and the sea; and there was a great calm." (Matthew 8:23-27)
            // The Lord of creation orders the raging waters and delivers His disciples.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ stills the storm on the sea (Matthew 8:23-27)",
                new CreationOrder(), new Deliverance(), new Protection(), new Peace()));
        }

        [Fact]
        public void ChristFeedsTheFiveThousand_IsADivineAct()
        {
            // Moved with compassion, Christ multiplies the loaves and "they did all eat, and were filled." (Matthew 14:14-21)
            // Divine compassion that feeds the hungry multitude.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ feeds the five thousand (Matthew 14:14-21)",
                new Compassion(), new FeedingTheHungry(), new Generosity(), new Goodness(), new CreationOrder()));
        }

        [Fact]
        public void ChristTeachesUnlimitedForgivenessSeventyTimesSeven_IsADivineAct()
        {
            // "I say not unto thee, Until seven times: but, Until seventy times seven." (Matthew 18:21-35)
            // The parable of the unforgiving servant commands boundless mercy and pardon.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ teaches forgiveness seventy times seven (Matthew 18:21-35)",
                new TeachingTruth(), new Forgiveness(), new Pardon(), new Compassion(), new Mourning()));
        }

        [Fact]
        public void ChristDescribesTheLastJudgmentOfTheNations_IsADivineAct()
        {
            // "I was an hungred, and ye gave me meat ... thirsty ... a stranger ... naked ... sick ... in prison." (Matthew 25:31-40)
            // The King names the works of mercy by which the sheep are known.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ names the works of mercy at the judgment (Matthew 25:31-40)",
                new TeachingTruth(), new FeedingTheHungry(), new GivingDrink(), new Hospitality(),
                new ClothingTheNaked(), new CaringForTheSick(), new VisitingThePrisoner()));
        }

        [Fact]
        public void JudasBetraysChristForThirtyPieces_IsSin()
        {
            // "What will ye give me, and I will deliver him unto you? And they covenanted with him for thirty pieces of silver." (Matthew 26:14-16)
            // Treacherous betrayal of the innocent for greed.
            OracleHarness.Sin(OracleHarness.Witness("Judas betrays Christ for thirty pieces of silver (Matthew 26:14-16)",
                new Treachery(), new Greed(), new Deceit(), new CovenantBreaking()));
        }

        [Fact]
        public void ChristGivesHimselfInTheCovenantOfHisBlood_IsADivineAct()
        {
            // "This is my blood of the new testament, which is shed for many for the remission of sins." (Matthew 26:26-28)
            // Christ offers Himself as the atoning sacrifice of the new covenant.
            OracleHarness.DivineAct(OracleHarness.Witness("Christ institutes the covenant in His blood (Matthew 26:26-28)",
                new SelfSacrifice(), new Atonement(), new CovenantFaithfulness(), new Pardon(), new Deliverance()));
        }

        [Fact]
        public void PilateCondemnsTheInnocentToBeCrucified_IsSin()
        {
            // Pilate, finding no fault, washes his hands yet delivers the innocent Christ to be crucified. (Matthew 27:24-26)
            // Bloodshed of the innocent through cowardly injustice and false condemnation.
            Resolution r = OracleHarness.Witness("Pilate condemns the innocent Christ to crucifixion (Matthew 27:24-26)",
                new Bloodshed(), new Condemnation(), new Wrongdoing(), new Oppression(), new FalseWitness());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void ChristIsRaisedAndSendsTheGreatCommission_IsADivineAct()
        {
            // "He is not here: for he is risen ... Go ye therefore, and teach all nations." (Matthew 28:6,19-20)
            // God raises Christ from the dead and the risen Lord commissions His church.
            OracleHarness.DivineAct(OracleHarness.Witness("The risen Christ gives the Great Commission (Matthew 28:6,19-20)",
                new RestoringLife(), new PromiseFulfilled(), new Deliverance(), new TeachingTruth(), new CovenantFaithfulness()));
        }
    }
}
