// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using System.Collections.Generic;
using AnointedAutomation.Objects.Account;
using MongoDB.Bson;
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
    }
}
