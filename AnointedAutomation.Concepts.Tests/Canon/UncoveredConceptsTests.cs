// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    /// <summary>
    /// Closes the audit gap of concepts that had only the reflection guard and no named scenario.
    /// Each is witnessed in a concrete deed and asserted to the verdict its Scripture gives.
    /// </summary>
    public class UncoveredConceptsTests
    {
        [Fact] public void Wisdom_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("the fear of the LORD is wisdom", new Wisdom()));
        [Fact] public void Integrity_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("uprightness of heart", new Integrity()));
        [Fact] public void Reverence_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("the fear of the LORD", new Reverence()));
        [Fact] public void Thankfulness_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("give thanks in all things", new Thankfulness()));
        [Fact] public void Contentment_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("godliness with contentment", new Contentment()));
        [Fact] public void Diligence_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("diligent hands", new Diligence()));
        [Fact] public void Discernment_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("discerning good from evil", new Discernment()));
        [Fact] public void Friendship_IsRighteous() => OracleHarness.Righteous(OracleHarness.Witness("a friend loves at all times", new Friendship()));

        [Fact] public void Grace_IsADivineAct() => OracleHarness.DivineAct(OracleHarness.Witness("by grace you are saved", new Grace()));
        [Fact] public void Provision_IsADivineAct() => OracleHarness.DivineAct(OracleHarness.Witness("your Father feeds the birds", new Provision()));
        [Fact] public void Comfort_IsADivineAct() => OracleHarness.DivineAct(OracleHarness.Witness("the God of all comfort", new Comfort()));
        [Fact] public void TheAscension_IsADivineAct() => OracleHarness.DivineAct(OracleHarness.Witness("taken up before their eyes", new Ascension()));
        [Fact] public void DivineRevelation_IsADivineAct() => OracleHarness.DivineAct(OracleHarness.Witness("God has spoken by his Son", new DivineRevelation()));

        [Fact] public void Cruelty_IsSin() => OracleHarness.Sin(OracleHarness.Witness("a cruel man brings trouble", new Cruelty()));
        [Fact] public void Tyranny_IsSin() => OracleHarness.Sin(OracleHarness.Witness("when the wicked rule the people groan", new Tyranny()));
        [Fact] public void Plunder_IsSin() => OracleHarness.Sin(OracleHarness.Witness("woe to you who plunder", new Plunder()));
        [Fact] public void Sacrilege_IsSin() => OracleHarness.Sin(OracleHarness.Witness("robbing the temple", new Sacrilege()));
        [Fact] public void Hypocrisy_IsSin() => OracleHarness.Sin(OracleHarness.Witness("full of hypocrisy within", new Hypocrisy()));
        [Fact] public void Simony_IsSin() => OracleHarness.Sin(OracleHarness.Witness("buying the gift of God with money", new Simony()));
        [Fact] public void QuickToEvil_IsSin() => OracleHarness.Sin(OracleHarness.Witness("feet quick to rush into evil", new QuickToEvil()));
        [Fact] public void DishonoringParents_IsSin() => OracleHarness.Sin(OracleHarness.Witness("dishonoring father and mother", new DishonoringParents()));
        [Fact] public void DisobedienceToParents_IsSin() => OracleHarness.Sin(OracleHarness.Witness("disobedient to parents", new DisobedienceToParents()));
    }
}
