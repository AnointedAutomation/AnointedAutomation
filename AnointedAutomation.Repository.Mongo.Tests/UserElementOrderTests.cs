// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System.Collections.Generic;
using AnointedAutomation.Objects.Account;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// The stored User document keeps Emails directly under Email (BSON element order follows the class map).
    /// </summary>
    public class UserElementOrderTests
    {
        [Fact]
        public void ToBsonDocument_PutsEmailsImmediatelyAfterEmail()
        {
            BsonClassMapRegistrar.RegisterClassMaps();
            User user = new User
            {
                UserId = "u1",
                Email = "a@x.com",
                Emails = new List<UserEmail> { new UserEmail { Address = "a@x.com", Verified = true, IsPrimary = true } },
            };

            BsonDocument doc = user.ToBsonDocument();
            List<string> names = new List<string>(doc.Names);
            int email = names.FindIndex(n => n.Equals("Email", System.StringComparison.Ordinal));
            int emails = names.FindIndex(n => n.Equals("Emails", System.StringComparison.Ordinal));

            Assert.True(email >= 0, string.Join(",", names));
            Assert.Equal(email + 1, emails);
        }

        private static string HybridName(string member)
        {
            BsonClassMap cm = new BsonClassMap(typeof(UserEmail));
            cm.AutoMap();
            BsonMemberMap mm = cm.GetMemberMap(member);
            new HybridElementNameConvention().Apply(mm);
            return mm.ElementName;
        }

        [Fact]
        public void UserEmail_ShopifyLink_WireCasing_IsCamelCase()
        {
            Assert.Equal("shopifyCustomerId", HybridName(nameof(UserEmail.ShopifyCustomerId)));
            Assert.Equal("shopifyLinkedAt", HybridName(nameof(UserEmail.ShopifyLinkedAt)));
            Assert.Equal("Address", HybridName(nameof(UserEmail.Address)));
        }

        [Fact]
        public void UserEmail_ShopifyLink_BsonRoundTrip()
        {
            System.DateTime linked = new System.DateTime(2026, 10, 1, 13, 0, 0, System.DateTimeKind.Utc);
            UserEmail email = new UserEmail { Address = "a@x.com", ShopifyCustomerId = 7712345678901L, ShopifyLinkedAt = linked };

            UserEmail back = BsonSerializer.Deserialize<UserEmail>(email.ToBsonDocument());

            Assert.Equal(7712345678901L, back.ShopifyCustomerId);
            Assert.Equal(linked, back.ShopifyLinkedAt);
            UserEmail none = BsonSerializer.Deserialize<UserEmail>(new UserEmail { Address = "b@x.com" }.ToBsonDocument());
            Assert.Null(none.ShopifyCustomerId);
            Assert.Null(none.ShopifyLinkedAt);
        }
    }
}
