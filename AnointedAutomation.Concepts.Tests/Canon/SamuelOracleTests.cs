// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-06-11
// Stewarded by Alexander Fields

using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests.Canon
{
    // Oracle 09: 1 & 2 Samuel.
    public class SamuelOracleTests
    {
        [Fact]
        public void SaulsRebellion_IsSin()
        {
            // "Rebellion is like the sin of divination." (1 Samuel 15:23)
            OracleHarness.Sin(OracleHarness.Witness("Saul disobeys God", new Rebellion(), new Disobedience()));
        }

        [Fact]
        public void DavidSparesSaul_IsRighteousMercy()
        {
            // David will not raise his hand against the LORD's anointed (1 Samuel 24; 26).
            OracleHarness.Righteous(OracleHarness.Witness("David spares Saul", new Pardon()));
        }

        [Fact]
        public void DavidAndBathsheba_IsGraveSin()
        {
            // Adultery, then the murder of Uriah to hide it (2 Samuel 11).
            Resolution r = OracleHarness.Witness("David takes Bathsheba and kills Uriah", new Adultery(), new Murder());

            OracleHarness.Sin(r);
            Assert.Equal(1.0, r.Disorder);
        }

        [Fact]
        public void DavidsRepentance_IsRighteous()
        {
            // "Against you, you only, have I sinned ... a broken and contrite heart." (Psalm 51)
            OracleHarness.Righteous(OracleHarness.Witness("David repents", new Repentance(), new Humility()));
        }

        [Fact]
        public void GodsCovenantWithDavid_IsADivineAct()
        {
            // "Your house and your kingdom will endure forever." (2 Samuel 7:16)
            OracleHarness.DivineAct(OracleHarness.Witness("the covenant with David",
                new CovenantFaithfulness(), new PromiseFulfilled()));
        }
    }
}
