// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;

namespace AnointedAutomation.Concepts.Reasoning
{
    /// <summary>
    /// Shared argument checks for the reasoning contracts. Required means required: no defaults.
    /// </summary>
    internal static class ReasoningGuard
    {
        internal static T NotNull<T>(T value, string parameterName)
            where T : class
        {
            if (value == null)
            {
                throw new System.ArgumentNullException(parameterName);
            }

            return value;
        }

        internal static string NotBlank(string value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new System.ArgumentException("A non-blank value is required.", parameterName);
            }

            return value;
        }

        internal static TEnum Defined<TEnum>(TEnum value, string parameterName)
            where TEnum : struct, System.Enum
        {
            if (!System.Enum.IsDefined(typeof(TEnum), value))
            {
                throw new System.ArgumentOutOfRangeException(parameterName, value, "Undefined enum value.");
            }

            return value;
        }

        internal static TEnum DefinedAndSet<TEnum>(TEnum value, string parameterName)
            where TEnum : struct, System.Enum
        {
            Defined(value, parameterName);
            if (System.Convert.ToInt64(value, System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                throw new System.ArgumentOutOfRangeException(
                    parameterName,
                    value,
                    "The unset (0) member is not a valid value here.");
            }

            return value;
        }

        internal static IReadOnlyList<T> CopyOf<T>(IEnumerable<T> items, string parameterName)
        {
            if (items == null)
            {
                throw new System.ArgumentNullException(parameterName);
            }

            List<T> copy = new List<T>(items);
            foreach (T item in copy)
            {
                if (item == null)
                {
                    throw new System.ArgumentException("The list must not contain null entries.", parameterName);
                }
            }

            return copy.AsReadOnly();
        }
    }
}
