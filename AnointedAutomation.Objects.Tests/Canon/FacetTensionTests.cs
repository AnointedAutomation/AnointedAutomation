// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// The conflicting-themes gap: until now the suite only tested facets in AGREEMENT (the cross,
    /// where Justice and Mercy are both fully satisfied). These tests pin what the engine does when
    /// facets genuinely pull OPPOSITE within one deed, so the harmonization rule (coherence = mean of
    /// facets, disorder = gravest wrong) is verified under tension, not just under harmony.
    /// </summary>
    public class FacetTensionTests
    {
        [Fact]
        public void AJustButMercilessAct_LeavesJusticeFullButMercyEmpty()
        {
            // The debt is rendered (Justice satisfied) but the broken are crushed (Mercy offended):
            // "judgment without mercy" (James 2:13). The engine does not pretend this resolves; it
            // shows the split and the disorder mercy's offence injects.
            Resolution r = OracleHarness.Witness("the debt paid but the contrite condemned",
                new Atonement(), new Condemnation());

            OracleHarness.Facet(r, "Justice", 1.0);
            OracleHarness.Facet(r, "Mercy", 0.0);
            OracleHarness.Exact(r, 0.5, 0.5);
        }

        [Fact]
        public void AMercifulButUnjustAct_LeavesMercyFullButJusticeEmpty()
        {
            // The guilty are spared (Mercy) at the cost of wronging the innocent party (Justice
            // offended). Mercy reads full, Justice empty, and the injustice injects disorder.
            Resolution r = OracleHarness.Witness("the guilty spared by wronging the victim",
                new Pardon(), new Wrongdoing());

            OracleHarness.Facet(r, "Mercy", 1.0);
            OracleHarness.Facet(r, "Justice", 0.0);
            OracleHarness.Exact(r, 0.375, 0.5);
        }

        [Fact]
        public void OnlyTheCrossResolvesTheTension_BothFacetsFull_NoDisorder()
        {
            // What no merely-just or merely-merciful act can do, the cross does: full Justice AND
            // full Mercy together, with nothing left offended (Romans 3:25-26; Psalm 85:10).
            Resolution r = OracleHarness.Witness("the cross", new Atonement(), new Pardon());

            OracleHarness.Facet(r, "Justice", 1.0);
            OracleHarness.Facet(r, "Mercy", 1.0);
            Assert.Equal(0.0, r.Disorder);
        }

        [Fact]
        public void WrathAndPatienceAreModeledAsCompatible_NotOpposed()
        {
            // The engine does NOT treat God's judgment and His patience as a contradiction: a just
            // recompense done patiently upholds both Justice and Love at once (Romans 2:4; Nahum 1:3,
            // "slow to anger ... will not leave the guilty unpunished"). This documents a modeling
            // stance: wrath-vs-patience is not a facet conflict here.
            Resolution r = OracleHarness.Witness("just judgment, patiently borne",
                new JustRecompense(), new Patience());

            OracleHarness.Facet(r, "Justice", 1.0);
            OracleHarness.Facet(r, "Love", 1.0);
            Assert.Equal(0.0, r.Disorder);
        }
    }
}
