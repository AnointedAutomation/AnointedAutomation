// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//
// Hybrid element-name convention for MongoDB serialization. Mirrors the API's System.Text.Json
// JsonCasingConvention so the DB and the JSON wire follow the SAME rule while C# code keeps idiomatic
// PascalCase property names:
//   - struct-declared member  -> camelCase
//   - value-type / enum member -> camelCase
//   - reference-type member    -> PascalCase
// Implemented as an IMemberMapConvention (the canonical element-naming hook, same interface as the
// driver's CamelCaseElementNameConvention). NOTE: explicit [BsonElement(...)] attributes WIN over any
// convention, so a member that pins its own casing via [BsonElement] is NOT changed here — to make such
// a field follow the hybrid rule, drop its pure-casing [BsonElement]. Intentional non-casing mappings
// (snake_case external-API fields like "fulfillment_channel", "_id", deliberate renames) are preserved
// by the guard below regardless. Register once at startup (before any class map is built) via
// BsonClassMapRegistrar.RegisterClassMaps().

using System;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Sets Mongo element names per the hybrid rule (value/enum/struct -> camelCase, reference -> PascalCase),
    /// preserving intentional non-casing mappings.
    /// </summary>
    public sealed class HybridElementNameConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            string memberName = memberMap.MemberName;
            string current = memberMap.ElementName;

            // Only re-case pure casing variants; preserve snake_case / renamed / "_id" mappings.
            if (!string.Equals(current, memberName, StringComparison.Ordinal)
                && !string.Equals(current, ToCamel(memberName), StringComparison.Ordinal)
                && !string.Equals(current, ToPascal(memberName), StringComparison.Ordinal))
            {
                return;
            }

            Type memberType = Nullable.GetUnderlyingType(memberMap.MemberType) ?? memberMap.MemberType;
            bool declaringIsStruct = memberMap.ClassMap.ClassType.IsValueType;
            bool camel = declaringIsStruct || memberType.IsValueType; // enums are value types
            memberMap.SetElementName(camel ? ToCamel(memberName) : ToPascal(memberName));
        }

        private static string ToCamel(string n) =>
            string.IsNullOrEmpty(n) || char.IsLower(n[0]) ? n : char.ToLowerInvariant(n[0]) + n.Substring(1);

        private static string ToPascal(string n) =>
            string.IsNullOrEmpty(n) || char.IsUpper(n[0]) ? n : char.ToUpperInvariant(n[0]) + n.Substring(1);
    }
}
