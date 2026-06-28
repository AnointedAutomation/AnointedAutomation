// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
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
using System.Text;
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
            if (!string.Equals(current, memberName, StringComparison.Ordinal)
                && !string.Equals(current, ToCamel(memberName), StringComparison.Ordinal)
                && !string.Equals(current, ToPascal(memberName), StringComparison.Ordinal))
            {
                return;
            }
            memberMap.SetElementName(ToSnake(memberName));
        }

        // PascalCase/camelCase -> snake_case. Leaves _id/dotted/already-lower keys unchanged.
        public static string ToSnake(string s)
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

        private static string ToCamel(string n) =>
            string.IsNullOrEmpty(n) || char.IsLower(n[0]) ? n : char.ToLowerInvariant(n[0]) + n.Substring(1);
        private static string ToPascal(string n) =>
            string.IsNullOrEmpty(n) || char.IsUpper(n[0]) ? n : char.ToUpperInvariant(n[0]) + n.Substring(1);
    }
}
