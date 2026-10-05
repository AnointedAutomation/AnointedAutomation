// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Probe types live OUTSIDE the AnointedAutomation namespace so registering conventions here never leaks into the
// other tests in this assembly.

using System;
using System.Collections.Generic;
using System.Reflection;
using AnointedAutomation.Repository.Mongo;
using AnointedAutomation.Repository.Mongo.Bson;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Xunit;

namespace HybridCasingProbe.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public string DisplayName { get; set; }
        public int LoginCount { get; set; }
        public Nested Inner { get; set; }
    }

    public class Nested
    {
        public string Label { get; set; }
        public bool IsOn { get; set; }
    }

    public class RawBlob
    {
        public string VendorField { get; set; }
        public int VendorCount { get; set; }
    }

    public abstract class AbstractThing
    {
        public string X { get; set; }
    }
}

namespace HybridCasingProbe
{
    public class HybridCasingTests
    {
        private static int _classMapCalls;
        private static bool _accountMappedBeforeTolerance;

        private static void EnsureRegistered()
        {
            HybridCasing.Register(
                "HybridCasingProbe",
                new[] { "RawBlob" },
                () =>
                {
                    _classMapCalls++;
                    if (!BsonClassMap.IsClassMapRegistered(typeof(Models.Account)))
                    {
                        BsonClassMap.RegisterClassMap<Models.Account>(cm =>
                        {
                            cm.AutoMap();
                            cm.MapIdMember(x => x.AccountId);
                        });
                    }

                    _accountMappedBeforeTolerance = true;
                },
                new[] { typeof(HybridCasingTests).Assembly });
        }

        [Fact]
        public void Register_AppliesCasing_ThenClassMaps_ThenTolerance_Once()
        {
            EnsureRegistered();
            EnsureRegistered();

            Assert.Equal(1, _classMapCalls);
            Assert.True(_accountMappedBeforeTolerance);

            BsonDocument doc = new Models.Account
            {
                AccountId = "a1",
                DisplayName = "d",
                LoginCount = 3,
                Inner = new Models.Nested { Label = "l", IsOn = true },
            }.ToBsonDocument();

            Assert.Equal("a1", doc["_id"].AsString);
            Assert.Equal("d", doc["DisplayName"].AsString);
            Assert.Equal(3, doc["loginCount"].AsInt32);
            Assert.Equal("l", doc["Inner"]["Label"].AsString);
            Assert.True(doc["Inner"]["isOn"].AsBoolean);

            // Class map from the callback (MapIdMember) survived: tolerance did not override it.
            Models.Account back = BsonSerializer.Deserialize<Models.Account>(doc);
            Assert.Equal("a1", back.AccountId);

            // Tolerance: nested type ignores unknown elements.
            Models.Nested nested = BsonSerializer.Deserialize<Models.Nested>(new BsonDocument { { "Label", "x" }, { "surprise", 1 } });
            Assert.Equal("x", nested.Label);

            // Deny-listed type keeps verbatim member names.
            BsonDocument raw = new Models.RawBlob { VendorField = "v", VendorCount = 2 }.ToBsonDocument();
            Assert.True(raw.Contains("VendorCount"));
        }

        [Fact]
        public void Filter_MatchesNamespacePrefixAndDenyList()
        {
            Func<Type, bool> filter = HybridCasing.BuildFilter("HybridCasingProbe", new[] { "RawBlob" });
            Assert.True(filter(typeof(Models.Account)));
            Assert.False(filter(typeof(Models.RawBlob)));
            Assert.False(filter(typeof(string)));
            Assert.Throws<ArgumentNullException>(() => HybridCasing.BuildFilter(null, null));
            Assert.Throws<ArgumentNullException>(() => HybridCasing.Register(null, null, null, null));
        }

        [Fact]
        public void ToleranceRegistrar_ValidatesArguments()
        {
            Assert.Throws<ArgumentNullException>(() => BsonToleranceRegistrar.RegisterIgnoreExtraElements(null));
            Assert.Throws<ArgumentNullException>(() => BsonToleranceRegistrar.RegisterIgnoreExtraElements(new List<Assembly>(), null));
            BsonToleranceRegistrar.RegisterIgnoreExtraElements(new List<Assembly>(), "Nothing");
        }
    }
}
