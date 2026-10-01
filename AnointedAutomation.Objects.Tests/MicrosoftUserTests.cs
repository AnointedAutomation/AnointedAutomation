// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//Stewarded by Alexander Fields

using AnointedAutomation.Objects.Microsoft;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class MicrosoftUserTests
    {
        private static MicrosoftUser Sample()
        {
            return new MicrosoftUser
            {
                Id = "oid-1",
                DisplayName = "Alex Fields",
                Mail = "alex@outlook.com",
                UserPrincipalName = "alex@outlook.com",
                GivenName = "Alex",
                Surname = "Fields",
                TenantId = "9188040d-6c67-4c5b-b112-36a304b66dad",
                IdentityProvider = "live.com"
            };
        }

        private static void AssertSame(MicrosoftUser expected, MicrosoftUser actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.DisplayName, actual.DisplayName);
            Assert.Equal(expected.Mail, actual.Mail);
            Assert.Equal(expected.UserPrincipalName, actual.UserPrincipalName);
            Assert.Equal(expected.GivenName, actual.GivenName);
            Assert.Equal(expected.Surname, actual.Surname);
            Assert.Equal(expected.TenantId, actual.TenantId);
            Assert.Equal(expected.IdentityProvider, actual.IdentityProvider);
        }

        [Fact]
        public void MicrosoftUser_BsonRoundTrip_PreservesEveryMember()
        {
            MicrosoftUser original = Sample();
            BsonDocument doc = original.ToBsonDocument();
            MicrosoftUser back = BsonSerializer.Deserialize<MicrosoftUser>(doc);
            AssertSame(original, back);
        }

        [Fact]
        public void MicrosoftUser_JsonUsesGraphMemberNames()
        {
            string json = JsonConvert.SerializeObject(Sample());
            Assert.Contains("\"userPrincipalName\"", json);
            Assert.Contains("\"identityProvider\"", json);
            AssertSame(Sample(), JsonConvert.DeserializeObject<MicrosoftUser>(json));
        }
    }
}
