// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Judges narrative oracle: the engine must return the verdict Scripture gives at each step.
    public class JudgesNarrativeTests
    {
        [Fact]
        public void IsraelAbandonsTheLordAndServesTheBaals_IsSin()
        {
            // "They forsook the LORD... and followed and worshiped various gods of the peoples around them." (Judges 2:11-13)
            OracleHarness.Sin(OracleHarness.Grounded("Israel forsakes the LORD and bows down to the Baals and Ashtoreths", Grounding.InIdol("Baal"), new Idolatry(), new CovenantBreaking(), new Disobedience()));
        }

        [Fact]
        public void GodRaisesUpJudgesToDeliverIsrael_IsADivineAct()
        {
            // "Then the LORD raised up judges, who saved them out of the hands of these raiders." (Judges 2:16-18)
            OracleHarness.DivineAct(OracleHarness.Witness("God raises up judges to deliver Israel when they cry out to Him", new Deliverance(), new Compassion(), new CovenantFaithfulness()));
        }

        [Fact]
        public void OthnielDeliversIsraelFromCushanRishathaim_IsRighteous()
        {
            // "The Spirit of the LORD came on him, so that he became Israel's judge and went to war." (Judges 3:9-11)
            OracleHarness.Righteous(OracleHarness.Witness("Othniel is raised as the first judge and delivers Israel from Cushan-Rishathaim", new Deliverance(), new CourageousFaith(), new ObedienceToGod()));
        }

        [Fact]
        public void DeborahJudgesIsraelAndSummonsBarakAtGodsWord_IsRighteous()
        {
            // "Deborah... was leading Israel at that time... 'The LORD, the God of Israel, commands you.'" (Judges 4:4-7)
            OracleHarness.Righteous(OracleHarness.Witness("Deborah judges Israel and delivers God's word to Barak to muster the deliverance", new TeachingTruth(), new CourageousFaith(), new ObedienceToGod(), new Righteousness()));
        }

        [Fact]
        public void GodRoutsSiseraAndDeliversIsrael_IsADivineAct()
        {
            // "At Barak's advance, the LORD routed Sisera and all his army." (Judges 4:14-15)
            OracleHarness.DivineAct(OracleHarness.Witness("God routs Sisera's army before Barak and delivers Israel", new Deliverance(), new JustRecompense(), new PromiseFulfilled()));
        }

        [Fact]
        public void DeborahAndBarakSingPraiseToTheLordForVictory_IsRighteous()
        {
            // "On that day Deborah and Barak son of Abinoam sang this song: 'When the princes... willingly offer themselves, praise the LORD!'" (Judges 5:1-3)
            OracleHarness.Righteous(OracleHarness.Witness("Deborah and Barak sing praise to the LORD for the victory He gave", new Worship(), new Joy(), new RejoicingInTruth()));
        }

        [Fact]
        public void GideonTearsDownTheAltarOfBaal_IsRighteous()
        {
            // "Tear down your father's altar to Baal and cut down the Asherah pole beside it." (Judges 6:25-27)
            OracleHarness.Righteous(OracleHarness.Witness("Gideon tears down his father's altar to Baal and the Asherah pole in obedience to God", new ObedienceToGod(), new CourageousFaith(), new Worship()));
        }

        [Fact]
        public void GodSavesIsraelByGideonsThreeHundred_IsADivineAct()
        {
            // "With the three hundred men... I will save you and give the Midianites into your hands." (Judges 7:7, 22)
            OracleHarness.DivineAct(OracleHarness.Witness("God delivers Midian into Israel's hand through Gideon's three hundred so the glory is His alone", new Deliverance(), new PromiseFulfilled(), new CovenantFaithfulness()));
        }

        [Fact]
        public void GideonMakesAnEphodThatBecomesASnare_IsSin()
        {
            // "All Israel prostituted themselves by worshiping it there, and it became a snare to Gideon and his family." (Judges 8:27)
            OracleHarness.Sin(OracleHarness.Grounded("Gideon makes a golden ephod that Israel worships, ensnaring his house", Grounding.InIdol("Ephod"), new Idolatry(), new GravenImage()));
        }

        [Fact]
        public void AbimelechMurdersHisSeventyBrothers_IsSin()
        {
            // "He murdered his seventy brothers... on one stone." (Judges 9:5)
            Resolution r = OracleHarness.Witness("Abimelech murders his seventy brothers on one stone to seize power", new Murder(), new Bloodshed(), new SelfishAmbition(), new Treachery());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void JephthahMakesAndKeepsARashVowSacrificingHisDaughter_IsSin()
        {
            // "He did to her as he had vowed." (Judges 11:30-39) - a rash vow ending in the death of his only child.
            Resolution r = OracleHarness.Witness("Jephthah carries out his rash vow and offers up his only daughter", new ChildSacrifice(), new Bloodshed());
            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void SamsonBreaksHisNaziriteVowsInLustAndRevelry_IsSin()
        {
            // Samson chases Philistine women, touches the unclean, and is undone at Delilah's feet. (Judges 14-16)
            OracleHarness.Sin(OracleHarness.Witness("Samson follows his lusts after foreign women and breaks his Nazirite consecration", new Lust(), new SexualImmorality(), new Disobedience()));
        }

        [Fact]
        public void SamsonCriesToGodAndIsAnsweredInHisFinalAct_IsRighteous()
        {
            // "Sovereign LORD, remember me. Please, God, strengthen me just once more." (Judges 16:28-30)
            OracleHarness.Righteous(OracleHarness.Witness("Samson repents and cries to God for strength one last time, dying to deliver Israel from the Philistines", new Repentance(), new Deliverance(), new SelfSacrifice(), new Trust()));
        }

        [Fact]
        public void TheTribesAvengeTheOutrageAtGibeah_IsRighteous()
        {
            // The Levite's concubine is abused to death and Israel rises to purge the evil from among them. (Judges 19-20)
            OracleHarness.Righteous(OracleHarness.Witness("the tribes of Israel rise to bring just recompense for the outrage at Gibeah", new JustRecompense(), new Righteousness()));
        }

        [Fact]
        public void EveryoneDidWhatWasRightInHisOwnEyes_IsSin()
        {
            // "In those days Israel had no king; everyone did as they saw fit." (Judges 21:25)
            OracleHarness.Sin(OracleHarness.Witness("with no king in Israel, everyone does what is right in his own eyes", new Lawlessness(), new Chaos(), new Rebellion()));
        }
    }
}
