// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using AnointedAutomation.Objects.Account;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class UserEmailTests
    {
        [Fact]
        public void UserEmail_Defaults()
        {
            UserEmail email = new UserEmail();
            Assert.Null(email.Address);
            Assert.False(email.Verified);
            Assert.Null(email.VerifiedAt);
            Assert.False(email.IsPrimary);
            Assert.Equal(UserEmailSource.Unknown, email.Source);
            Assert.Equal(default(DateTime), email.AddedAt);
            Assert.Null(email.ShopifyCustomerId);
            Assert.Null(email.ShopifyLinkedAt);
        }

        [Fact]
        public void UserEmail_ShopifyLink_JsonRoundTrip()
        {
            DateTime linked = new DateTime(2026, 10, 1, 13, 0, 0, DateTimeKind.Utc);
            UserEmail email = new UserEmail { Address = "a@example.com", ShopifyCustomerId = 7712345678901L, ShopifyLinkedAt = linked };

            UserEmail back = JsonConvert.DeserializeObject<UserEmail>(JsonConvert.SerializeObject(email));

            Assert.Equal(7712345678901L, back.ShopifyCustomerId);
            Assert.Equal(linked, back.ShopifyLinkedAt);
            Assert.Null(JsonConvert.DeserializeObject<UserEmail>("{\"Address\":\"a@example.com\"}").ShopifyCustomerId);
        }

        [Fact]
        public void User_EmailsDefaultsToNull()
        {
            Assert.Null(new User().Emails);
        }

        [Theory]
        [InlineData(UserEmailSource.Unknown, 0)]
        [InlineData(UserEmailSource.Manual, 1)]
        [InlineData(UserEmailSource.Signup, 2)]
        [InlineData(UserEmailSource.Google, 3)]
        [InlineData(UserEmailSource.Microsoft, 4)]
        [InlineData(UserEmailSource.Apple, 5)]
        [InlineData(UserEmailSource.Facebook, 6)]
        [InlineData(UserEmailSource.Shopify, 7)]
        [InlineData(UserEmailSource.Merge, 8)]
        [InlineData(UserEmailSource.Admin, 9)]
        public void UserEmailSource_Values(UserEmailSource source, int expected)
        {
            Assert.Equal(expected, (int)source);
        }

        [Fact]
        public void UserEmailSource_HasTenMembers()
        {
            Assert.Equal(10, Enum.GetValues(typeof(UserEmailSource)).Length);
        }

        [Fact]
        public void User_Emails_JsonRoundTrip()
        {
            DateTime added = new DateTime(2026, 10, 1, 12, 0, 0, DateTimeKind.Utc);
            User user = new User
            {
                Email = "a@example.com",
                Emails = new List<UserEmail>
                {
                    new UserEmail { Address = "a@example.com", Verified = true, VerifiedAt = added, IsPrimary = true, Source = UserEmailSource.Signup, AddedAt = added },
                    new UserEmail { Address = "b@example.com", Source = UserEmailSource.Google, AddedAt = added }
                }
            };

            string json = JsonConvert.SerializeObject(user);
            User back = JsonConvert.DeserializeObject<User>(json);

            Assert.Equal("a@example.com", back.Email);
            Assert.Equal(2, back.Emails.Count);
            Assert.Equal("a@example.com", back.Emails[0].Address);
            Assert.True(back.Emails[0].Verified);
            Assert.Equal(added, back.Emails[0].VerifiedAt);
            Assert.True(back.Emails[0].IsPrimary);
            Assert.Equal(UserEmailSource.Signup, back.Emails[0].Source);
            Assert.Equal(added, back.Emails[0].AddedAt);
            Assert.False(back.Emails[1].Verified);
            Assert.Null(back.Emails[1].VerifiedAt);
            Assert.Equal(UserEmailSource.Google, back.Emails[1].Source);
        }

        [Fact]
        public void User_WithoutEmails_DeserializesNull()
        {
            User back = JsonConvert.DeserializeObject<User>("{\"Email\":\"a@example.com\"}");
            Assert.Null(back.Emails);
        }
    }
}
