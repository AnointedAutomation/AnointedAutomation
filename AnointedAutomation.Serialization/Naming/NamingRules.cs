// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The ONE home for member-name casing rules. The System.Text.Json casing convention, the Mongo hybrid
// element-name convention and the Mongo snake_case convention all delegate here, so the JSON wire and the
// database follow the same rule by construction.

using System;
using System.Text;
using System.Text.Json;

namespace AnointedAutomation.Serialization.Naming
{
    /// <summary>How the camelCase half of the hybrid rule lower-cases a name.</summary>
    public enum CamelStyle
    {
        /// <summary>Lower-case the first character only ("URLValue" -> "uRLValue"). The Mongo convention uses this.</summary>
        FirstChar = 0,

        /// <summary>System.Text.Json's acronym-aware camel case ("URLValue" -> "urlValue"). The JSON wire uses this.</summary>
        SystemTextJson = 1,
    }

    /// <summary>
    /// Casing rules shared by every Anointed serializer.
    /// Hybrid rule: a member declared on a struct, or whose (nullable-unwrapped) type is a value type or enum,
    /// is camelCase; every other member (reference types) is PascalCase.
    /// </summary>
    public static class NamingRules
    {
        /// <summary>Lower-cases the first character only. Null/empty or already-lower first char returns the input unchanged.</summary>
        public static string ToCamel(string name) =>
            string.IsNullOrEmpty(name) || char.IsLower(name[0]) ? name : char.ToLowerInvariant(name[0]) + name.Substring(1);

        /// <summary>System.Text.Json's camel case (<see cref="JsonNamingPolicy.CamelCase"/>), which lower-cases a leading acronym run. Null/empty unchanged.</summary>
        public static string ToCamelJson(string name) =>
            string.IsNullOrEmpty(name) ? name : JsonNamingPolicy.CamelCase.ConvertName(name);

        /// <summary>Upper-cases the first character only. Null/empty or already-upper first char returns the input unchanged.</summary>
        public static string ToPascal(string name) =>
            string.IsNullOrEmpty(name) || char.IsUpper(name[0]) ? name : char.ToUpperInvariant(name[0]) + name.Substring(1);

        /// <summary>
        /// PascalCase/camelCase to snake_case (LineItems -> line_items, HTTPServer -> http_server). Leaves null/empty,
        /// a leading '_' (e.g. "_id"), dotted paths and names without any upper-case letter unchanged.
        /// </summary>
        public static string ToSnake(string s)
        {
            if (string.IsNullOrEmpty(s) || s[0] == '_' || s.IndexOf('.') >= 0)
            {
                return s;
            }

            bool hasUpper = false;
            for (int i = 0; i < s.Length; i++)
            {
                if (char.IsUpper(s[i]))
                {
                    hasUpper = true;
                    break;
                }
            }

            if (!hasUpper)
            {
                return s;
            }

            StringBuilder sb = new StringBuilder(s.Length + 4);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (char.IsUpper(c))
                {
                    bool boundary = i > 0
                        && (char.IsLower(s[i - 1]) || char.IsDigit(s[i - 1])
                            || (i + 1 < s.Length && char.IsLower(s[i + 1])));
                    if (boundary && sb.Length > 0 && sb[sb.Length - 1] != '_')
                    {
                        sb.Append('_');
                    }

                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// True when the hybrid rule wants camelCase: the declaring type is a struct, or the member type
        /// (nullable unwrapped) is a value type (enums included).
        /// </summary>
        public static bool IsCamelMember(Type declaringType, Type memberType)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException(nameof(declaringType));
            }

            if (memberType == null)
            {
                throw new ArgumentNullException(nameof(memberType));
            }

            Type underlying = Nullable.GetUnderlyingType(memberType) ?? memberType;
            return declaringType.IsValueType || underlying.IsValueType;
        }

        /// <summary>
        /// THE hybrid rule as one call: returns <paramref name="memberName"/> camelCased (in <paramref name="camelStyle"/>)
        /// when <see cref="IsCamelMember"/> is true, otherwise PascalCased.
        /// </summary>
        public static string ToHybrid(string memberName, Type declaringType, Type memberType, CamelStyle camelStyle)
        {
            if (IsCamelMember(declaringType, memberType))
            {
                return camelStyle == CamelStyle.SystemTextJson ? ToCamelJson(memberName) : ToCamel(memberName);
            }

            return ToPascal(memberName);
        }

        /// <summary>
        /// True when <paramref name="elementName"/> is just a casing variant of <paramref name="memberName"/> (identical,
        /// first-char camel, or first-char Pascal). Conventions only re-case such names and leave deliberate renames alone.
        /// </summary>
        public static bool IsPureCasingVariant(string elementName, string memberName) =>
            string.Equals(elementName, memberName, StringComparison.Ordinal)
            || string.Equals(elementName, ToCamel(memberName), StringComparison.Ordinal)
            || string.Equals(elementName, ToPascal(memberName), StringComparison.Ordinal);
    }
}
