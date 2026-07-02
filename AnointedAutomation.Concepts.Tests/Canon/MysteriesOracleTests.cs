// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-12
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    /// <summary>
    /// Closes the "events and sacraments" gap. These are not moral virtues or sins but holy mysteries
    /// in which the whole character of God is present; each reads as a perfectly coherent divine act.
    /// </summary>
    public class MysteriesOracleTests
    {
        [Fact]
        public void TheIncarnation_IsFullyDivine()
        {
            OracleHarness.FullyDivine(OracleHarness.Witness("the Word became flesh", new Incarnation()));
        }

        [Fact]
        public void TheResurrection_IsFullyDivine()
        {
            OracleHarness.FullyDivine(OracleHarness.Witness("raised on the third day", new Resurrection()));
        }

        [Fact]
        public void TheTransfiguration_IsFullyDivine()
        {
            OracleHarness.FullyDivine(OracleHarness.Witness("his face shone like the sun", new Transfiguration()));
        }

        [Fact]
        public void Pentecost_IsFullyDivine()
        {
            OracleHarness.FullyDivine(OracleHarness.Witness("filled with the Holy Spirit", new Pentecost()));
        }

        [Fact]
        public void TheEucharist_IsADivineMystery()
        {
            OracleHarness.DivineAct(OracleHarness.Witness("this is my body, given for you", new Eucharist()));
        }

        [Fact]
        public void Baptism_IsADivineMystery()
        {
            OracleHarness.DivineAct(OracleHarness.Witness("baptized into the Triune name", new Baptism()));
        }

        [Fact]
        public void Ordination_IsADivineMystery()
        {
            OracleHarness.DivineAct(OracleHarness.Witness("the laying on of hands", new Ordination()));
        }

        [Fact]
        public void TheNewCreation_IsFullyDivine()
        {
            OracleHarness.FullyDivine(OracleHarness.Witness("making everything new", new NewCreation()));
        }
    }
}
