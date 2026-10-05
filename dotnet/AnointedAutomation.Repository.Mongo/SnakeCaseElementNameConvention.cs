// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// snake_case element-name convention for MongoDB ("Anointed Styling": structural Mongo keys are snake_case;
// JSON content is hybrid, handled separately by the JSON-wire convention). C# keeps idiomatic PascalCase
// property names; this maps each member to a snake_case Mongo key (LineItems -> line_items). Implemented as
// an IMemberMapConvention (same hook as the driver's CamelCaseElementNameConvention). Explicit [BsonElement]
// attributes WIN over conventions, so a member that pins a non-casing name (already snake, dotted, or an
// external contract) is left alone; to make an attributed member follow this rule, drop/snake its attribute.
// OPT-IN: registered via BsonClassMapRegistrar.RegisterSnakeCasingConvention(namespacePrefix); publishing
// this changes nothing until a service opts in.

using System;
using AnointedAutomation.Serialization.Naming;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>Maps member names to snake_case Mongo element names, preserving intentional non-casing mappings.</summary>
    public sealed class SnakeCaseElementNameConvention : ConventionBase, IMemberMapConvention
    {
        public void Apply(BsonMemberMap memberMap)
        {
            string memberName = memberMap.MemberName;
            string current = memberMap.ElementName;

            // Only re-case pure casing variants of the member name; preserve snake/dotted/renamed mappings.
            if (!NamingRules.IsPureCasingVariant(current, memberName))
            {
                return;
            }
            memberMap.SetElementName(ToSnake(memberName));
        }

        /// <summary>PascalCase/camelCase -> snake_case. Leaves _id/dotted/already-lower keys unchanged. Delegates to <see cref="NamingRules.ToSnake"/>.</summary>
        public static string ToSnake(string s) => NamingRules.ToSnake(s);
    }
}
