// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using AnointedAutomation.Objects.Facebook;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class FacebookTypesTests
    {
        private const string MeJson = @"{""id"":""10221234567890"",""name"":""Jane Q Doe"",""first_name"":""Jane"",""last_name"":""Doe"",""email"":""jane@example.com"",""token_for_business"":""AbXyBizToken123"",""timezone"":-4.5,""verified"":true,""age_range"":{""min"":21},""picture"":{""data"":{""height"":200,""width"":200,""is_silhouette"":false,""url"":""https://platform-lookaside.fbsbx.com/x.jpg""}}}";

        private const string DebugJson = @"{""app_id"":""138483919580948"",""type"":""USER"",""application"":""Anointed"",""data_access_expires_at"":1700000000,""expires_at"":1690000000,""is_valid"":true,""issued_at"":1680000000,""scopes"":[""email"",""public_profile""],""granular_scopes"":[{""scope"":""pages_show_list"",""target_ids"":[""123"",""456""]}],""user_id"":""10221234567890""}";

        [Fact]
        public void FacebookUser_DeserializesGraphMe()
        {
            FacebookUser u = JsonConvert.DeserializeObject<FacebookUser>(MeJson);
            Assert.Equal("10221234567890", u.Id);
            Assert.Equal("Jane", u.FirstName);
            Assert.Equal("Doe", u.LastName);
            Assert.Equal("jane@example.com", u.Email);
            Assert.Equal("AbXyBizToken123", u.TokenForBusiness);
            Assert.Equal(-4.5f, u.Timezone);
            Assert.True(u.Verified);
            Assert.Equal(21, u.AgeRange.Min);
            Assert.Null(u.AgeRange.Max);
            Assert.Equal(200, u.Picture.Data.Height);
            Assert.False(u.Picture.Data.IsSilhouette);
            Assert.Equal("https://platform-lookaside.fbsbx.com/x.jpg", u.Picture.Data.Url);
        }

        [Fact]
        public void FacebookUser_SerializesSnakeCaseNames()
        {
            JObject o = JObject.FromObject(JsonConvert.DeserializeObject<FacebookUser>(MeJson));
            Assert.Equal("AbXyBizToken123", (string)o["token_for_business"]);
            Assert.Equal("Jane", (string)o["first_name"]);
            Assert.False((bool)o["picture"]["data"]["is_silhouette"]);
        }

        [Fact]
        public void FacebookDebugTokenData_DeserializesAllFields()
        {
            FacebookDebugTokenData d = JsonConvert.DeserializeObject<FacebookDebugTokenData>(DebugJson);
            Assert.Equal("138483919580948", d.AppId);
            Assert.Equal("USER", d.Type);
            Assert.Equal("Anointed", d.Application);
            Assert.Equal(1690000000L, d.ExpiresAt);
            Assert.Equal(1700000000L, d.DataAccessExpiresAt);
            Assert.Equal(1680000000L, d.IssuedAt);
            Assert.True(d.IsValid);
            Assert.Equal(new[] { "email", "public_profile" }, d.Scopes);
            Assert.Equal("10221234567890", d.UserId);
            Assert.Single(d.GranularScopes);
            Assert.Equal("pages_show_list", d.GranularScopes[0].Scope);
            Assert.Equal(new[] { "123", "456" }, d.GranularScopes[0].TargetIds);
        }
    }
}
