// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
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
// BsonClassMapRegistrar.RegisterHybridCasingConvention().

using System;
using AnointedAutomation.Serialization.Naming;
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
            if (!NamingRules.IsPureCasingVariant(current, memberName))
            {
                return;
            }

            memberMap.SetElementName(NamingRules.ToHybrid(memberName, memberMap.ClassMap.ClassType, memberMap.MemberType, CamelStyle.FirstChar));
        }
    }
}
