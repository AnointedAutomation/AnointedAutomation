// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System;
using AnointedAutomation.Objects.Account;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class SsoTests
    {
        [Fact]
        public void SsoIdentity_ConstructorAssignsEveryField()
        {
            DateTime linkedAt = new DateTime(2026, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            SsoIdentity identity = new SsoIdentity("sub-123", "user@example.com", "Jane Doe", "https://pic/x.png", linkedAt);

            Assert.Equal("sub-123", identity.Subject);
            Assert.Equal("user@example.com", identity.Email);
            Assert.Equal("Jane Doe", identity.Name);
            Assert.Equal("https://pic/x.png", identity.Picture);
            Assert.Equal(linkedAt, identity.LinkedAt);
        }

        [Fact]
        public void SsoIdentity_RoundTripsThroughJson()
        {
            SsoIdentity identity = new SsoIdentity("sub-abc", "a@b.com", "A B", null, new DateTime(2026, 5, 6, 0, 0, 0, DateTimeKind.Utc));

            string json = JsonConvert.SerializeObject(identity);
            SsoIdentity back = JsonConvert.DeserializeObject<SsoIdentity>(json);

            Assert.Equal(identity.Subject, back.Subject);
            Assert.Equal(identity.Email, back.Email);
            Assert.Equal(identity.Name, back.Name);
            Assert.Null(back.Picture);
            Assert.Equal(identity.LinkedAt, back.LinkedAt);
        }

        [Fact]
        public void Sso_ProviderSlotsDefaultToNull()
        {
            Sso sso = new Sso();

            Assert.Null(sso.Google);
            Assert.Null(sso.Microsoft);
            Assert.Null(sso.Apple);
            Assert.Null(sso.Facebook);
            Assert.Null(sso.Shopify);
        }

        [Fact]
        public void Sso_HoldsEachProviderIndependently()
        {
            Sso sso = new Sso
            {
                Google = new SsoIdentity("g-1", "g@x.com", "G", null, DateTime.UtcNow),
                Shopify = new SsoIdentity("123456", null, null, null, DateTime.UtcNow)
            };

            Assert.NotNull(sso.Google);
            Assert.Equal("g-1", sso.Google.Value.Subject);
            Assert.NotNull(sso.Shopify);
            Assert.Equal("123456", sso.Shopify.Value.Subject);
            Assert.Null(sso.Microsoft);
        }

        [Fact]
        public void User_SsoIsNullByDefault_AndDoesNotDisturbGoogleOrMeta()
        {
            User user = new User();

            Assert.Null(user.Sso);

            user.Sso = new Sso { Google = new SsoIdentity("g-9", "g@x.com", "G", null, DateTime.UtcNow) };
            user.Meta = "{\"ShopifyCustomerId\":\"555\"}";

            Assert.NotNull(user.Sso);
            Assert.Equal("g-9", user.Sso.Google.Value.Subject);
            Assert.Equal("{\"ShopifyCustomerId\":\"555\"}", user.Meta);
        }
    }
}
