// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Objects.Concepts;

namespace AnointedAutomation.Objects.Tests.Canon
{
    /// <summary>
    /// Gap 11: the authoritative, hand-reviewed corpus. The engine's chief limitation is that it
    /// judges whatever concepts are assigned to a deed; this file is the vetted mapping of the
    /// redemptive spine, creation to new creation, from event to its correct concept set and verdict.
    /// It is the reference against which any future re-modeling of these events must still agree, so
    /// the engine rests on a reviewed corpus rather than ad-hoc labeling.
    /// </summary>
    public class CanonicalCorpusTests
    {
        [Fact] public void Creation() => OracleHarness.DivineAct(OracleHarness.Witness("Genesis 1: creation", new CreationOrder(), new CovenantFaithfulness()));
        [Fact] public void TheFall() => OracleHarness.Sin(OracleHarness.Witness("Genesis 3: the fall", new Disobedience(), new Rebellion()));
        [Fact] public void GodSavesNoah() => OracleHarness.DivineAct(OracleHarness.Witness("Genesis 6-9: the flood and the ark", new Deliverance(), new CovenantFaithfulness()));
        [Fact] public void AbrahamsFaith() => OracleHarness.Righteous(OracleHarness.Witness("Genesis 15:6: Abraham believes God", new Trust(), new ObedienceToGod()));
        [Fact] public void TheExodus() => OracleHarness.DivineAct(OracleHarness.Witness("Exodus 3-14: deliverance from Egypt", new Deliverance(), new CovenantFaithfulness()));
        [Fact] public void TheLawAtSinai() => OracleHarness.DivineAct(OracleHarness.Witness("Exodus 20: the righteous Law given", new Righteousness(), new CovenantFaithfulness()));
        [Fact] public void TheGoldenCalf() => OracleHarness.Sin(OracleHarness.Witness("Exodus 32: the golden calf", new Idolatry()));
        [Fact] public void DavidAndBathsheba() => OracleHarness.Sin(OracleHarness.Witness("2 Samuel 11: David takes Bathsheba and kills Uriah", new Adultery(), new Murder()));
        [Fact] public void DavidsRepentance() => OracleHarness.Righteous(OracleHarness.Witness("Psalm 51: David repents", new Repentance(), new Humility()));
        [Fact] public void TheApostasyThatBringsExile() => OracleHarness.Sin(OracleHarness.Witness("2 Kings 17: apostasy brings exile", new Idolatry(), new Rebellion()));
        [Fact] public void TheReturnFromExile() => OracleHarness.DivineAct(OracleHarness.Witness("Ezra 1: God restores His people", new CovenantFaithfulness(), new Deliverance()));
        [Fact] public void TheIncarnation() => OracleHarness.FullyDivine(OracleHarness.Witness("John 1:14: the Word made flesh", new Incarnation()));
        [Fact] public void JesusHealsForgivesTeaches() => OracleHarness.DivineAct(OracleHarness.Witness("the ministry of Jesus", new Healing(), new Pardon(), new TeachingTruth()));
        [Fact] public void TheGoodSamaritan() => OracleHarness.Righteous(OracleHarness.Witness("Luke 10:37: the Good Samaritan", new Compassion(), new Protection()));
        [Fact] public void TheCross() => OracleHarness.FullyDivine(OracleHarness.Witness("the cross", new SelfSacrifice(), new Atonement(), new Pardon(), new CovenantFaithfulness()));
        [Fact] public void TheResurrection() => OracleHarness.FullyDivine(OracleHarness.Witness("1 Corinthians 15:4: raised on the third day", new Resurrection()));
        [Fact] public void Pentecost() => OracleHarness.FullyDivine(OracleHarness.Witness("Acts 2: the Spirit poured out", new Pentecost()));
        [Fact] public void TheNewCreation() => OracleHarness.FullyDivine(OracleHarness.Witness("Revelation 21:5: making everything new", new NewCreation()));
    }
}
