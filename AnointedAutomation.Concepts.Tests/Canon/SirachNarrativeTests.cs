// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Sirach (Wisdom of Ben Sira / Ecclesiasticus) narrative oracle: the engine must return the verdict
    // Scripture gives at each step. Ben Sira teaches the fear of the Lord as the beginning of wisdom,
    // and weighs the deeds of the wise against the deeds of fools and the wicked.
    public class SirachNarrativeTests
    {
        [Fact]
        public void AllWisdomComesFromTheLordAndIsPouredOutByHim_IsADivineAct()
        {
            // "All wisdom comes from the Lord and is with him forever... he poured her out upon all his works." (Sirach 1:1-10)
            OracleHarness.DivineAct(OracleHarness.Witness("the Lord creates wisdom and lavishes her on all his works", new CreationOrder(), new Generosity(), new Goodness()));
        }

        [Fact]
        public void TheFearOfTheLordIsTheBeginningOfWisdom_IsRighteous()
        {
            // "The fear of the Lord is the beginning of wisdom... To fear the Lord is the fullness of wisdom." (Sirach 1:14-20)
            OracleHarness.Righteous(OracleHarness.Witness("the godly man begins in the fear of the Lord", new Worship(), new ObedienceToGod(), new Humility()));
        }

        [Fact]
        public void TrustingGodWithEnduranceWhenTestedInTheFurnace_IsRighteous()
        {
            // "My son, if you come forward to serve the Lord... set your heart right and be steadfast... For gold is tested in the fire." (Sirach 2:1-6)
            OracleHarness.Righteous(OracleHarness.Witness("the servant stays steadfast and trusts God through testing", new Trust(), new Endurance(), new Fidelity(), new CourageousFaith()));
        }

        [Fact]
        public void HonoringFatherAndMotherInDeedAndWord_IsRighteous()
        {
            // "Whoever honors his father atones for sins... Honor your father by word and deed." (Sirach 3:1-9)
            OracleHarness.Righteous(OracleHarness.Witness("the son honors and cares for his father and mother", new ObedienceToGod(), new HonoringTheVulnerable(), new Kindness()));
        }

        [Fact]
        public void TheGreaterYouAreTheMoreYouMustHumbleYourself_IsRighteous()
        {
            // "The greater you are, the more you must humble yourself... For great is the might of the Lord." (Sirach 3:18-20)
            OracleHarness.Righteous(OracleHarness.Witness("the great man humbles himself before the Lord", new Humility(), new Meekness()));
        }

        [Fact]
        public void NotRejectingTheSupplicantOrTurningAwayFromThePoor_IsRighteous()
        {
            // "Do not reject an afflicted suppliant... Incline your ear to the poor, and answer him peaceably." (Sirach 4:1-10)
            OracleHarness.Righteous(OracleHarness.Witness("the wise man hears the poor and rescues the oppressed", new Compassion(), new FeedingTheHungry(), new DefendingTheWeak(), new Kindness()));
        }

        [Fact]
        public void PresumingOnGodsMercyToHeapSinUponSin_IsSin()
        {
            // "Do not say, 'His mercy is great, he will forgive...' and so heap sin upon sin." (Sirach 5:4-7)
            OracleHarness.Sin(OracleHarness.Witness("the presumptuous man heaps sin upon sin, gambling on God's mercy", new Pride(), new Disobedience(), new Insolence()));
        }

        [Fact]
        public void TheDoubleTonguedSlandererWhoBetraysAFriend_IsSin()
        {
            // "Curse the gossip and the double-tongued, for they destroy the peace of many." (Sirach 5:14-6:1, 28:13-15)
            OracleHarness.Sin(OracleHarness.Witness("the double-tongued slanderer destroys friendships and peace", new Slander(), new Gossip(), new Treachery(), new SowingDiscord()));
        }

        [Fact]
        public void GivingGladlyAndLendingToTheNeighborInNeed_IsRighteous()
        {
            // "Help the poor for the commandment's sake... Lose your silver for the sake of a brother." (Sirach 29:1-13)
            OracleHarness.Righteous(OracleHarness.Witness("the merciful man lends gladly and gives alms to his neighbor", new Generosity(), new Compassion(), new Kindness()));
        }

        [Fact]
        public void NourishingRageAndNursingTheGrudgeAgainstANeighbor_IsSin()
        {
            // "Anger and wrath, these also are abominations... He who takes vengeance will suffer the vengeance of the Lord." (Sirach 27:30-28:7)
            OracleHarness.Sin(OracleHarness.Witness("the man who nurses wrath and refuses to forgive his neighbor", new FitsOfRage(), new Grudge(), new Unforgiveness(), new Hatred()));
        }

        [Fact]
        public void ForgivingTheNeighborSoThatYourOwnSinsArePardoned_IsRighteous()
        {
            // "Forgive your neighbor the wrong he has done, and then your sins will be pardoned when you pray." (Sirach 28:2-4)
            OracleHarness.Righteous(OracleHarness.Witness("the godly man forgives his neighbor's wrong before he prays", new Forgiveness(), new Pardon(), new Compassion()));
        }

        [Fact]
        public void TheDrunkardAndGluttonGivenToWineAndExcess_IsSin()
        {
            // "Wine drunk to excess is bitterness of soul... Do not try to prove your strength by wine-drinking." (Sirach 31:25-31, 37:27-31)
            OracleHarness.Sin(OracleHarness.Witness("the man given over to drunkenness and gluttonous excess", new Drunkenness(), new Gluttony(), new Carousing()));
        }

        [Fact]
        public void DefraudingTheHiredLaborerOfHisWages_IsSin()
        {
            // "To take away a neighbor's living is to commit murder; to deprive an employee of his wages is to shed blood." (Sirach 34:21-27)
            OracleHarness.Sin(OracleHarness.Witness("the oppressor robs the hired laborer of the wages that sustain his life", new WithholdingWhatIsDue(), new Oppression(), new Wrongdoing(), new Theft()));
        }

        [Fact]
        public void FearingTheLordAndKeepingTheCommandmentsAsTheWholeDuty_IsRighteous()
        {
            // "Whoever keeps the law controls his thoughts... fear of the Lord is the beginning of acceptance." (Sirach 19:20, 1:11-13)
            OracleHarness.Righteous(OracleHarness.Witness("the wise man keeps the law and walks in the fear of the Lord", new ObedienceToGod(), new Fidelity(), new SelfControl(), new Worship()));
        }
    }
}
