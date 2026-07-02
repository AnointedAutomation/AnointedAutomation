// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // 1 Maccabees narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class FirstMaccabeesNarrativeTests
    {
        [Fact]
        public void AntiochusPlundersTheTempleAndShedsBlood_IsSin()
        {
            // Antiochus Epiphanes enters the sanctuary in arrogance, strips the golden altar,
            // lampstand, and vessels, and "shed much blood, and spoke with great arrogance." (1 Macc 1:20-24)
            // Proud, sacrilegious plunder of God's house, steeped in bloodshed.
            OracleHarness.Sin(OracleHarness.Witness("Antiochus plunders the temple in Jerusalem and sheds much blood",
                new Theft(), new Defilement(), new Bloodshed(), new Pride(), new Blasphemy()));
            Assert.Equal(1.0, OracleHarness.Witness("Antiochus plunders the temple in Jerusalem and sheds much blood",
                new Theft(), new Defilement(), new Bloodshed(), new Pride(), new Blasphemy()).Disorder);
        }

        [Fact]
        public void AntiochusOutlawsTheLawAndCommandsIdolatry_IsSin()
        {
            // The king decrees that all should "follow customs strange to the land," forbidding burnt
            // offerings, profaning Sabbaths, building altars to idols, and sacrificing swine. (1 Macc 1:41-50)
            // Lawless decree forcing apostasy, idolatry, and the breaking of God's covenant.
            OracleHarness.Sin(OracleHarness.Witness("Antiochus outlaws the Law and commands the nations to worship idols",
                new Idolatry(), new Lawlessness(), new CovenantBreaking(), new Oppression(), new Blasphemy()));
        }

        [Fact]
        public void TheAbominationOfDesolationIsSetUpOnTheAltar_IsSin()
        {
            // "They erected a desolating sacrilege upon the altar of burnt offering," sacrificing on
            // pagan altars and defiling the sanctuary. (1 Macc 1:54-59)
            // The supreme defilement of God's holy place by idolatrous worship.
            OracleHarness.Sin(OracleHarness.Witness("The abomination of desolation is set up on the altar of God",
                new Defilement(), new Idolatry(), new Blasphemy(), new Lawlessness()));
            Assert.Equal(1.0, OracleHarness.Witness("The abomination of desolation is set up on the altar of God",
                new Defilement(), new Idolatry(), new Blasphemy(), new Lawlessness()).Disorder);
        }

        [Fact]
        public void FaithfulIsraelitesChooseDeathRatherThanDefileThemselves_IsRighteous()
        {
            // "Many in Israel stood firm and resolved in their hearts not to eat unclean food. They chose
            // to die rather than be defiled ... and they did die." (1 Macc 1:62-63)
            // Faithful endurance unto death rather than break the covenant.
            OracleHarness.Righteous(OracleHarness.Witness("Faithful Israelites choose to die rather than defile themselves",
                new CourageousFaith(), new Endurance(), new Fidelity(), new ObedienceToGod(), new Purity()));
        }

        [Fact]
        public void MattathiasRefusesToSacrificeToIdols_IsRighteous()
        {
            // "I and my sons and my brothers will live by the covenant of our fathers ... we will not obey
            // the king's words by turning aside from our religion." (1 Macc 2:19-22)
            // Steadfast covenant faithfulness and courageous refusal of idolatry.
            OracleHarness.Righteous(OracleHarness.Witness("Mattathias refuses to forsake the covenant and sacrifice to idols",
                new CovenantFaithfulness(), new CourageousFaith(), new ObedienceToGod(), new Fidelity()));
        }

        [Fact]
        public void MattathiasBurnsWithZealAndTearsDownTheIdolAltar_IsRighteous()
        {
            // Mattathias "burned with zeal for the law," pulled down the pagan altar, and called out,
            // "Let every one who is zealous for the law and supports the covenant come out with me!" (1 Macc 2:24-27)
            // Holy zeal defending God's covenant and casting down idolatry.
            OracleHarness.Righteous(OracleHarness.Grounded("Mattathias tears down the idol altar and rallies the zealous for the covenant",
                Grounding.InGod(),
                new CovenantFaithfulness(), new CourageousFaith(), new ObedienceToGod(), new Worship()));
        }

        [Fact]
        public void TheFaithfulResolveToDefendLifeOnTheSabbath_IsRighteous()
        {
            // After a thousand are slaughtered for refusing to fight on the Sabbath, the people resolve,
            // "Let us fight ... that we may not all die." (1 Macc 2:39-41)
            // Wise, life-protecting defense of God's people without forsaking Him.
            OracleHarness.Righteous(OracleHarness.Witness("The faithful resolve to defend their lives even on the Sabbath",
                new Protection(), new DefendingTheWeak(), new CourageousFaith(), new Endurance()));
        }

        [Fact]
        public void JudasMaccabeusPraysAndDeliversIsraelFromOppressors_IsRighteous()
        {
            // Judas Maccabeus, before battle, leads the people in prayer and fasting, then routs Apollonius
            // and Seron, "and saved Israel." (1 Macc 3:10-26; 4:8-11)
            // Courageous, trusting defense of God's people against the oppressor.
            OracleHarness.Righteous(OracleHarness.Grounded("Judas Maccabeus prays and delivers Israel from her oppressors",
                Grounding.InGod(),
                new CourageousFaith(), new Trust(), new DefendingTheWeak(), new Deliverance(), new Protection()));
        }

        [Fact]
        public void GodGivesVictoryToJudasOverTheMightyArmyOfGorgias_IsADivineAct()
        {
            // Against an overwhelming host, Judas cries, "It is not on the size of the army that victory
            // ... depends, but strength comes from Heaven," and the Lord crushes the enemy. (1 Macc 3:18-19; 4:14-15)
            // Scripture's verdict is the salvation Heaven works for the weak who trust Him.
            OracleHarness.DivineAct(OracleHarness.Grounded("God gives Judas the victory over the mighty army at Emmaus",
                Grounding.InGod(),
                new Deliverance(), new Protection(), new DefendingTheWeak(), new JustRecompense(),
                new Fidelity(), new PromiseFulfilled()));
        }

        [Fact]
        public void JudasCleansesAndRededicatesTheTemple_IsRighteous()
        {
            // Judas tears down the defiled altar, builds a new one, and rededicates the sanctuary with
            // joy, song, and burnt offerings. (1 Macc 4:36-58)
            // Joyful, worshipful restoration of God's house and covenant worship.
            OracleHarness.Righteous(OracleHarness.Grounded("Judas cleanses the temple and rededicates it with worship and joy",
                Grounding.InGod(),
                new Worship(), new Joy(), new CovenantFaithfulness(), new RejoicingInTruth(), new ObedienceToGod()));
        }

        [Fact]
        public void NicanorBlasphemesAndThreatensToBurnTheTemple_IsSin()
        {
            // Nicanor stretches out his hand against the sanctuary in arrogance, threatening to burn
            // God's house unless Judas is handed over, and blasphemes. (1 Macc 7:33-35)
            // Proud blasphemy and threatened defilement of the holy place.
            OracleHarness.Sin(OracleHarness.Witness("Nicanor blasphemes the temple and threatens to burn it down",
                new Blasphemy(), new Pride(), new Defilement(), new Boasting(), new Oppression()));
        }

        [Fact]
        public void SimonLeadsIsraelInPeaceJusticeAndCareForTheLowly_IsRighteous()
        {
            // Under Simon "the land had rest ... he made the poor man's heart glad," providing food,
            // strengthening the lowly, and the people sat under their own vines in peace. (1 Macc 14:8-15)
            // Just, generous, peace-giving stewardship that cares for the vulnerable.
            OracleHarness.Righteous(OracleHarness.Witness("Simon governs Israel in peace, justice, and care for the lowly",
                new Peace(), new Goodness(), new Generosity(), new HonoringTheVulnerable(),
                new DefendingTheWeak(), new Peacemaking()));
        }
    }
}
