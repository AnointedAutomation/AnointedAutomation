// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-30
//Stewarded by Alexander Fields

using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class ResponseDataTests
    {
        [Fact]
        public void ResponseData_Links_IncludesAnointedUmbrella()
        {
            // The Anointed umbrella site (anointed.company) ties the family of ventures together
            // and must be advertised first in every API response's Links.
            ResponseData response = new ResponseData();

            Assert.True(response.Links.ContainsKey("Anointed"));
            Assert.Equal("https://anointed.company", response.Links["Anointed"]);
        }

        [Fact]
        public void GenericResponseData_Links_IncludesAnointedUmbrella()
        {
            ResponseData<string> response = new ResponseData<string>();

            Assert.True(response.Links.ContainsKey("Anointed"));
            Assert.Equal("https://anointed.company", response.Links["Anointed"]);
        }

        [Fact]
        public void ResponseData_Links_RetainsAutomationVentureLink()
        {
            // The umbrella is added alongside — not in place of — the existing automation link.
            ResponseData response = new ResponseData();

            Assert.Equal("https://www.anointedautomation.net", response.Links["Automation"]);
        }
    }
}
