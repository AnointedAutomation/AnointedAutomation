// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//Stewarded by Alexander Fields

using System.IO;
using System.Linq;
using AnointedAutomation.Objects.Account;
using AnointedAutomation.Objects.Apple;
using AnointedAutomation.Objects.Facebook;
using AnointedAutomation.Objects.Google;
using AnointedAutomation.Objects.Microsoft;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class SSOTests
    {
        private static AnointedAutomation.Shopify.Customer LoadShopifyFixture()
        {
            string json = File.ReadAllText(Path.Combine("Fixtures", "shopify_customer_rest.json"));
                        return JObject.Parse(json)["customers"][0].ToObject<AnointedAutomation.Shopify.Customer>();
        }

        [Fact]
        public void SSO_ProviderSlotsDefaultToNull()
        {
            SSO sso = new SSO();

            Assert.Null(sso.Google);
            Assert.Null(sso.Microsoft);
            Assert.Null(sso.Apple);
            Assert.Null(sso.Facebook);
            Assert.Null(sso.Shopify);
        }

        [Fact]
        public void SSO_HoldsEachProviderRealObjectIndependently()
        {
            SSO sso = new SSO
            {
                Google = new GoogleObjects(new GoogleTokenInfo { sub = "g-tok" }, new UserProfile { id = "g-1", name = "G" }),
                Microsoft = new MicrosoftUser { Id = "ms-oid", DisplayName = "M", TenantId = "tid-1", IdentityProvider = "live.com" },
                Apple = new AppleSignIn { IdToken = new AppleIdTokenClaims { Sub = "apple-sub" } },
                Facebook = new FacebookLogin { User = new FacebookUser { Id = "fb-id", Name = "F" } },
                Shopify = new AnointedAutomation.Shopify.Customer { Id = 123456, Email = "s@x.com" }
            };

            Assert.Equal("g-1", sso.Google.UserProfile.id);
            Assert.Equal("ms-oid", sso.Microsoft.Id);
            Assert.Equal("live.com", sso.Microsoft.IdentityProvider);
            Assert.Equal("apple-sub", sso.Apple.IdToken.Sub);
            Assert.Equal("fb-id", sso.Facebook.User.Id);
            Assert.Equal(123456, sso.Shopify.Id);
        }

        [Fact]
        public void User_SSOIsNullByDefault_AndMetaIsIndependent()
        {
            User user = new User();

            Assert.Null(user.SSO);

            user.SSO = new SSO { Google = new GoogleObjects(new GoogleTokenInfo(), new UserProfile { id = "g-9" }) };
            user.Meta = "{\"ShopifyCustomerId\":\"555\"}";

            Assert.Equal("g-9", user.SSO.Google.UserProfile.id);
            Assert.Equal("{\"ShopifyCustomerId\":\"555\"}", user.Meta);
        }

        [Fact]
        public void ShopifyCustomer_DeserializesRealRestPayload()
        {
            AnointedAutomation.Shopify.Customer customer = LoadShopifyFixture();

            Assert.Equal(23780308549743L, customer.Id);
            Assert.Equal("customer@example.com", customer.Email);
            Assert.Equal(3, customer.OrdersCount);
            Assert.Equal(346.27m, customer.TotalSpent);
            Assert.Equal("gid://shopify/Customer/23780308549743", customer.AdminGraphQLAPIId);
            Assert.Single(customer.Addresses);
            Assert.Equal("US", customer.DefaultAddress.CountryCode);
            Assert.Equal("subscribed", customer.EmailMarketingConsent.State);
        }

        [Fact]
        public void ShopifyCustomer_BsonRoundTrip_PreservesRealPayload()
        {
            AnointedAutomation.Shopify.Customer customer = LoadShopifyFixture();

            BsonDocument doc = customer.ToBsonDocument();
            AnointedAutomation.Shopify.Customer back = BsonSerializer.Deserialize<AnointedAutomation.Shopify.Customer>(doc);

            Assert.Equal(customer.Id, back.Id);
            Assert.Equal(customer.Email, back.Email);
            Assert.Equal(customer.CreatedAt, back.CreatedAt);
            Assert.Equal(customer.TotalSpent, back.TotalSpent);
            Assert.Equal(customer.Tags, back.Tags);
            Assert.Equal(customer.Addresses.Single().Id, back.Addresses.Single().Id);
            Assert.Equal(customer.DefaultAddress.City, back.DefaultAddress.City);
            Assert.Equal(customer.EmailMarketingConsent.OptInLevel, back.EmailMarketingConsent.OptInLevel);
        }

        [Fact]
        public void AppleIdTokenClaims_AcceptsBooleansAsStringsOrBools()
        {
            string asStrings = "{\"iss\":\"https://appleid.apple.com\",\"aud\":\"com.x\",\"exp\":1700000600,\"iat\":1700000000,\"sub\":\"001.abc\",\"email\":\"r@privaterelay.appleid.com\",\"email_verified\":\"true\",\"is_private_email\":\"true\",\"nonce_supported\":\"false\",\"real_user_status\":2,\"auth_time\":1700000000,\"c_hash\":\"ch\",\"at_hash\":\"ah\",\"nonce\":\"n\",\"transfer_sub\":\"t\"}";
            string asBools = "{\"sub\":\"001.abc\",\"email_verified\":true,\"is_private_email\":false,\"nonce_supported\":true,\"real_user_status\":1}";

            AppleIdTokenClaims a = JsonConvert.DeserializeObject<AppleIdTokenClaims>(asStrings);
            AppleIdTokenClaims b = JsonConvert.DeserializeObject<AppleIdTokenClaims>(asBools);

            Assert.True(a.EmailVerified);
            Assert.True(a.IsPrivateEmail);
            Assert.False(a.NonceSupported);
            Assert.Equal(AppleRealUserStatus.LikelyReal, a.RealUserStatus);
            Assert.Equal(1700000600L, a.Exp);
            Assert.Equal("ch", a.CHash);
            Assert.Equal("t", a.TransferSub);
            Assert.True(b.EmailVerified);
            Assert.False(b.IsPrivateEmail);
            Assert.True(b.NonceSupported);
            Assert.Equal(AppleRealUserStatus.Unknown, b.RealUserStatus);
        }

        [Fact]
        public void AppleUser_DeserializesFirstConsentShape()
        {
            AppleUser user = JsonConvert.DeserializeObject<AppleUser>("{\"name\":{\"firstName\":\"Ada\",\"lastName\":\"Lovelace\"},\"email\":\"ada@x.com\"}");

            Assert.Equal("Ada", user.Name.FirstName);
            Assert.Equal("Lovelace", user.Name.LastName);
            Assert.Equal("ada@x.com", user.Email);
        }

        [Fact]
        public void FacebookUser_DeserializesGraphMeShape_IgnoringUnrequestedFields()
        {
            string me = "{\"id\":\"10229876543210987\",\"name\":\"Ada Lovelace\",\"first_name\":\"Ada\",\"last_name\":\"Lovelace\",\"email\":\"ada@x.com\",\"locale\":\"en_US\",\"picture\":{\"data\":{\"height\":200,\"width\":200,\"is_silhouette\":false,\"url\":\"https://p.example/a.jpg\"}}}";

            FacebookUser user = JsonConvert.DeserializeObject<FacebookUser>(me);

            Assert.Equal("10229876543210987", user.Id);
            Assert.Equal("Ada", user.FirstName);
            Assert.Equal(200, user.Picture.Data.Height);
            Assert.False(user.Picture.Data.IsSilhouette);
            Assert.Equal("https://p.example/a.jpg", user.Picture.Data.Url);
        }

        [Fact]
        public void FacebookDebugTokenData_DeserializesGraphDataShape()
        {
            string json = "{\"data\":{\"app_id\":\"138483919580948\",\"type\":\"USER\",\"application\":\"Anointed\",\"data_access_expires_at\":1700600000,\"expires_at\":0,\"is_valid\":true,\"issued_at\":1700000000,\"scopes\":[\"email\",\"public_profile\"],\"user_id\":\"10229876543210987\"}}";

            FacebookDebugTokenData data = JObject.Parse(json)["data"].ToObject<FacebookDebugTokenData>();

            Assert.Equal("138483919580948", data.AppId);
            Assert.Equal("USER", data.Type);
            Assert.Equal(0L, data.ExpiresAt);
            Assert.True(data.IsValid);
            Assert.Equal(new[] { "email", "public_profile" }, data.Scopes);
            Assert.Equal("10229876543210987", data.UserId);
        }

        [Fact]
        public void FacebookUser_DeserializesSdkFieldSet()
        {
            string me = "{\"id\":\"1\",\"middle_name\":\"Q\",\"short_name\":\"Ada\",\"name_format\":\"{first} {last}\",\"locale\":\"en_US\",\"link\":\"https://fb/x\",\"verified\":true,\"updated_time\":\"2026-01-01T00:00:00+0000\",\"timezone\":-4.5,\"profile_pic\":\"https://p/x\",\"token_for_business\":\"AbTfB\",\"third_party_id\":\"tp\",\"age_range\":{\"min\":21}}";

            FacebookUser user = JsonConvert.DeserializeObject<FacebookUser>(me);

            Assert.Equal("Q", user.MiddleName);
            Assert.Equal("Ada", user.ShortName);
            Assert.Equal("{first} {last}", user.NameFormat);
            Assert.Equal("en_US", user.Locale);
            Assert.True(user.Verified);
            Assert.Equal(-4.5f, user.Timezone);
            Assert.Equal("AbTfB", user.TokenForBusiness);
            Assert.Equal("tp", user.ThirdPartyId);
            Assert.Equal(21, user.AgeRange.Min);
            Assert.Null(user.AgeRange.Max);
        }

        [Fact]
        public void MicrosoftUser_UsesGraphJsonNames()
        {
            MicrosoftUser user = JsonConvert.DeserializeObject<MicrosoftUser>("{\"id\":\"o\",\"displayName\":\"D\",\"mail\":\"m@x.com\",\"userPrincipalName\":\"u@x.com\",\"givenName\":\"G\",\"surname\":\"S\",\"tenantId\":\"t\",\"identityProvider\":\"live.com\"}");

            Assert.Equal("o", user.Id);
            Assert.Equal("m@x.com", user.Mail);
            Assert.Equal("u@x.com", user.UserPrincipalName);
            Assert.Equal("S", user.Surname);
            Assert.Equal("t", user.TenantId);
            Assert.Equal("live.com", user.IdentityProvider);
        }

        [Theory]
        [InlineData("email-disabled", AppleNotificationEventType.EmailDisabled)]
        [InlineData("email-enabled", AppleNotificationEventType.EmailEnabled)]
        [InlineData("consent-revoked", AppleNotificationEventType.ConsentRevoked)]
        [InlineData("account-delete", AppleNotificationEventType.AccountDelete)]
        [InlineData("something-new", AppleNotificationEventType.Unknown)]
        public void AppleNotificationPayload_ParsesEventsJsonString(string wire, AppleNotificationEventType expected)
        {
            string events = "{\"type\":\"" + wire + "\",\"sub\":\"001.abc\",\"event_time\":1700000000123,\"email\":\"r@privaterelay.appleid.com\",\"is_private_email\":\"true\"}";
            string claims = JsonConvert.SerializeObject(new { iss = "https://appleid.apple.com", aud = "net.x", iat = 1700000000, jti = "j1", events });

            AppleNotificationPayload payload = JsonConvert.DeserializeObject<AppleNotificationPayload>(claims);
            AppleNotificationEvent evt = payload.ParseEvent();

            Assert.Equal("j1", payload.Jti);
            Assert.Equal(1700000000L, payload.Iat);
            Assert.Equal(expected, evt.Type);
            Assert.Equal("001.abc", evt.Sub);
            Assert.Equal(1700000000123L, evt.EventTime);
            Assert.True(evt.IsPrivateEmail);
            Assert.Equal("r@privaterelay.appleid.com", evt.Email);
        }

        [Fact]
        public void AppleNotificationEventType_WritesWireString()
        {
            string json = JsonConvert.SerializeObject(new AppleNotificationEvent { Type = AppleNotificationEventType.ConsentRevoked, Sub = "s" });

            Assert.Contains("\"type\":\"consent-revoked\"", json);
            Assert.Null(new AppleNotificationPayload().ParseEvent());
        }

        [Theory]
        [InlineData("true", true)]
        [InlineData("FALSE", false)]
        [InlineData("", null)]
        [InlineData(null, null)]
        [InlineData("yes", null)]
        public void StringOrBoolConverter_Parse(string input, bool? expected)
        {
            Assert.Equal(expected, StringOrBoolConverter.Parse(input));
        }
    }
}
