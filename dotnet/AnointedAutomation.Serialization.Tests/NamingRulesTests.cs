// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Proves NamingRules reproduces, byte for byte, the three private copies it replaced (Objects.API JsonCasingConvention,
// Repository.Mongo HybridElementNameConvention, Repository.Mongo SnakeCaseElementNameConvention). The "Legacy*" methods
// below are verbatim copies of the removed code.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AnointedAutomation.Serialization.Naming;
using Xunit;

namespace AnointedAutomation.Serialization.Tests
{
    public class NamingRulesTests
    {
        public static readonly string[] Names =
        {
            null, "", "a", "A", "_", "_id", "Id", "id", "ID", "URL", "URLValue", "UrlValue", "urlValue", "IOStream",
            "HTTPServerURL", "XMLHttpRequest", "ABC123Def", "Line2", "Address1", "address1", "A1", "a1", "1st", "123",
            "LineItems", "lineItems", "line_items", "Line_Items", "_Private", "__Double", "Already_snake_Case", "snake_case",
            "camelCase", "PascalCase", "SKU", "SkuId", "SKUId", "MyID", "MyIDs", "IDs", "iPhone", "IPhone", "eBay",
            "Order.Name", "a.B", "ÄpfelSaft", "élan", "X", "x", "XY", "xY", "Xy", "V2Api", "ApiV2", "GoogleTokenInfo",
            "IsPrivateEmail", "has Space", "Has-Dash", "Δelta", "ÉCOLE", "TaxID2023Value",
        };

        public static IEnumerable<object[]> NameData()
        {
            foreach (string name in Names)
            {
                yield return new object[] { name };
            }
        }

        // ---- verbatim legacy copies ----
        private static string LegacyJsonToCamel(string name) =>
            string.IsNullOrEmpty(name) ? name : JsonNamingPolicy.CamelCase.ConvertName(name);

        private static string LegacyPascal(string name) =>
            string.IsNullOrEmpty(name) || char.IsUpper(name[0]) ? name : char.ToUpperInvariant(name[0]) + name.Substring(1);

        private static string LegacyMongoCamel(string n) =>
            string.IsNullOrEmpty(n) || char.IsLower(n[0]) ? n : char.ToLowerInvariant(n[0]) + n.Substring(1);

        private static string LegacySnake(string s)
        {
            if (string.IsNullOrEmpty(s) || s[0] == '_' || s.IndexOf('.') >= 0) return s;
            bool hasUpper = false;
            for (int i = 0; i < s.Length; i++) if (char.IsUpper(s[i])) { hasUpper = true; break; }
            if (!hasUpper) return s;
            StringBuilder sb = new StringBuilder(s.Length + 4);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsUpper(c))
                {
                    bool boundary = i > 0 &&
                        (char.IsLower(s[i - 1]) || char.IsDigit(s[i - 1]) ||
                         (i + 1 < s.Length && char.IsLower(s[i + 1])));
                    if (boundary && sb.Length > 0 && sb[sb.Length - 1] != '_') sb.Append('_');
                    sb.Append(char.ToLowerInvariant(c));
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        [Theory]
        [MemberData(nameof(NameData))]
        public void ToCamelJson_MatchesLegacyJsonConvention(string name) =>
            Assert.Equal(LegacyJsonToCamel(name), NamingRules.ToCamelJson(name));

        [Theory]
        [MemberData(nameof(NameData))]
        public void ToCamel_MatchesLegacyMongoConventions(string name) =>
            Assert.Equal(LegacyMongoCamel(name), NamingRules.ToCamel(name));

        [Theory]
        [MemberData(nameof(NameData))]
        public void ToPascal_MatchesLegacy(string name) =>
            Assert.Equal(LegacyPascal(name), NamingRules.ToPascal(name));

        [Theory]
        [MemberData(nameof(NameData))]
        public void ToSnake_MatchesLegacy(string name) =>
            Assert.Equal(LegacySnake(name), NamingRules.ToSnake(name));

        [Theory]
        [MemberData(nameof(NameData))]
        public void IsPureCasingVariant_MatchesLegacyGuard(string name)
        {
            if (name == null)
            {
                return;
            }

            foreach (string candidate in new[] { name, LegacyMongoCamel(name), LegacyPascal(name), LegacySnake(name), name + "x", "_id" })
            {
                bool legacy = string.Equals(candidate, name, StringComparison.Ordinal)
                    || string.Equals(candidate, LegacyMongoCamel(name), StringComparison.Ordinal)
                    || string.Equals(candidate, LegacyPascal(name), StringComparison.Ordinal);
                Assert.Equal(legacy, NamingRules.IsPureCasingVariant(candidate, name));
            }
        }

        [Theory]
        [InlineData("URLValue", "urlValue", "uRLValue")]
        [InlineData("IOStream", "ioStream", "iOStream")]
        [InlineData("ID", "id", "iD")]
        public void CamelStyles_DifferOnAcronyms(string name, string json, string firstChar)
        {
            Assert.Equal(json, NamingRules.ToCamelJson(name));
            Assert.Equal(firstChar, NamingRules.ToCamel(name));
        }

        [Theory]
        [InlineData("LineItems", "line_items")]
        [InlineData("HTTPServer", "http_server")]
        [InlineData("Address1Line", "address1_line")]
        [InlineData("_id", "_id")]
        [InlineData("a.B", "a.B")]
        [InlineData("already", "already")]
        public void ToSnake_Examples(string name, string expected) => Assert.Equal(expected, NamingRules.ToSnake(name));

        private struct SomeStruct
        {
        }

        private enum SomeEnum
        {
            A,
        }

        private class SomeClass
        {
        }

        [Fact]
        public void IsCamelMember_FollowsHybridRule()
        {
            Assert.True(NamingRules.IsCamelMember(typeof(SomeClass), typeof(int)));
            Assert.True(NamingRules.IsCamelMember(typeof(SomeClass), typeof(int?)));
            Assert.True(NamingRules.IsCamelMember(typeof(SomeClass), typeof(SomeEnum)));
            Assert.True(NamingRules.IsCamelMember(typeof(SomeClass), typeof(SomeEnum?)));
            Assert.True(NamingRules.IsCamelMember(typeof(SomeClass), typeof(DateTime)));
            Assert.True(NamingRules.IsCamelMember(typeof(SomeStruct), typeof(string)));
            Assert.False(NamingRules.IsCamelMember(typeof(SomeClass), typeof(string)));
            Assert.False(NamingRules.IsCamelMember(typeof(SomeClass), typeof(List<int>)));
            Assert.Throws<ArgumentNullException>(() => NamingRules.IsCamelMember(null, typeof(int)));
            Assert.Throws<ArgumentNullException>(() => NamingRules.IsCamelMember(typeof(int), null));
        }

        [Theory]
        [MemberData(nameof(NameData))]
        public void ToHybrid_MatchesLegacyFormulas(string name)
        {
            Type[] declaring = { typeof(SomeClass), typeof(SomeStruct) };
            Type[] members = { typeof(string), typeof(int), typeof(long?), typeof(SomeEnum), typeof(SomeClass), typeof(SomeStruct?) };
            foreach (Type d in declaring)
            {
                foreach (Type m in members)
                {
                    Type underlying = Nullable.GetUnderlyingType(m) ?? m;
                    bool camel = d.IsValueType || underlying.IsValueType;
                    Assert.Equal(camel ? LegacyJsonToCamel(name) : LegacyPascal(name), NamingRules.ToHybrid(name, d, m, CamelStyle.SystemTextJson));
                    Assert.Equal(camel ? LegacyMongoCamel(name) : LegacyPascal(name), NamingRules.ToHybrid(name, d, m, CamelStyle.FirstChar));
                }
            }
        }
    }
}
