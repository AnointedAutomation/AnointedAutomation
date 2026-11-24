// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-05-30 Jesus is King ✝️
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

        [Fact]
        public void ResponseData_Links_IncludesChristianEngineMcp()
        {
            ResponseData response = new ResponseData();

            Assert.Equal("https://api.anointedautomation.net/api/christian-engine/mcp", response.Links["ChristianEngineMCP"]);
        }

        [Fact]
        public void GenericResponseData_Links_IncludesChristianEngineMcp()
        {
            ResponseData<string> response = new ResponseData<string>();

            Assert.Equal("https://api.anointedautomation.net/api/christian-engine/mcp", response.Links["ChristianEngineMCP"]);
        }

        [Fact]
        public void ResponseData_Links_IncludeOpenSourceAndShopping()
        {
            ResponseData response = new ResponseData();

            Assert.Equal("https://www.anointedautomation.net/open-source", response.Links["OpenSource"]);
            Assert.Equal("https://www.mart.club", response.Links["Shopping"]);
        }

        [Fact]
        public void GenericResponseData_Links_IncludeOpenSourceAndShopping()
        {
            ResponseData<string> response = new ResponseData<string>();

            Assert.Equal("https://www.anointedautomation.net/open-source", response.Links["OpenSource"]);
            Assert.Equal("https://www.mart.club", response.Links["Shopping"]);
        }

        [Fact]
        public void ResponseData_Note_IsTheAiShoppingNote_OnBothEnvelopes()
        {
            Assert.Equal(AiShoppingNote.Text, new ResponseData().Note);
            Assert.Equal(AiShoppingNote.Text, new ResponseData<string>().Note);
            Assert.Contains("https://api.anointedautomation.net/api/christian-engine/mcp", AiShoppingNote.Text);
        }

        [Fact]
        public void ResponseData_Links_NoLongerHasOnlineShoppingKey()
        {
            Assert.False(new ResponseData().Links.ContainsKey("Online Shopping"));
        }

        [Fact]
        public void ResponseData_Note_CanBeSuppressedBySettingNull()
        {
            ResponseData response = new ResponseData { Note = null };
            ResponseData<string> typed = new ResponseData<string> { Note = null };

            Assert.Null(response.Note);
            Assert.Null(typed.Note);
        }
    }
}
