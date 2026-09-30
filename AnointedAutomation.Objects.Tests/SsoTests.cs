// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using System;
using AnointedAutomation.Objects.Account;
using AnointedAutomation.Objects.Apple;
using AnointedAutomation.Objects.Facebook;
using AnointedAutomation.Objects.Google;
using AnointedAutomation.Objects.Microsoft;
using AnointedAutomation.Objects.Shopify;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class SsoTests
    {
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
                Microsoft = new MicrosoftObjects { oid = "ms-oid", name = "M" },
                Apple = new AppleObjects { sub = "apple-sub", firstName = "A" },
                Facebook = new FacebookObjects { id = "fb-id", name = "F" },
                Shopify = new ShopifyObjects { customerId = 123456, email = "s@x.com" }
            };

            Assert.Equal("g-1", sso.Google.UserProfile.id);
            Assert.Equal("ms-oid", sso.Microsoft.oid);
            Assert.Equal("apple-sub", sso.Apple.sub);
            Assert.Equal("fb-id", sso.Facebook.id);
            Assert.Equal(123456, sso.Shopify.customerId);
        }

        [Fact]
        public void User_SSOIsNullByDefault_AndMetaIsIndependent()
        {
            User user = new User();

            Assert.Null(user.SSO);

            user.SSO = new SSO { Google = new GoogleObjects(new GoogleTokenInfo(), new UserProfile { id = "g-9" }) };
            user.Meta = "{\"ShopifyCustomerId\":\"555\"}";

            Assert.NotNull(user.SSO);
            Assert.Equal("g-9", user.SSO.Google.UserProfile.id);
            Assert.Equal("{\"ShopifyCustomerId\":\"555\"}", user.Meta);
        }

        [Fact]
        public void ShopifyObjects_ZeroCustomerIdMeansNotLinked()
        {
            ShopifyObjects shopify = new ShopifyObjects();
            Assert.Equal(0, shopify.customerId);
        }
    }
}
