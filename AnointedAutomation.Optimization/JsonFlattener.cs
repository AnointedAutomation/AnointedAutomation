// Copyright 2024 Anointed Automation, LLC All rights reserved.
// Originally created by Alexander Fields. Ported and modernized (Newtonsoft.Json replaced with the
// dependency-free System.Text.Json) for the AnointedAutomation monorepo.

using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;

namespace AnointedAutomation.Optimization
{
    /// <summary>
    /// Flattens nested JSON into a single-level dictionary of underscore-joined keys, and turns a
    /// dictionary into a single-row <see cref="DataTable"/>.
    /// </summary>
    public static class JsonFlattener
    {
        /// <summary>Builds a single-row <see cref="DataTable"/> from a dictionary.</summary>
        public static DataTable DictionaryToDataTable(Dictionary<string, object?> dict)
        {
            DataTable dataTable = new DataTable();

            foreach (string key in dict.Keys)
            {
                dataTable.Columns.Add(key, dict[key]?.GetType() ?? typeof(object));
            }

            DataRow newRow = dataTable.NewRow();
            foreach (string key in dict.Keys)
            {
                newRow[key] = dict[key] ?? DBNull.Value;
            }
            dataTable.Rows.Add(newRow);

            return dataTable;
        }

        /// <summary>
        /// Flattens a JSON object or array string into a dictionary keyed by underscore-joined paths.
        /// </summary>
        /// <param name="jsonContent">The JSON text.</param>
        /// <param name="changeCase">When true, keys are cased per <paramref name="toUpper"/>.</param>
        /// <param name="toUpper">When <paramref name="changeCase"/> is true, upper-case (true) or lower-case (false).</param>
        /// <exception cref="ArgumentException">Thrown when the string is not a JSON object or array.</exception>
        public static Dictionary<string, object?> FlattenJson(string jsonContent, bool changeCase, bool toUpper)
        {
            Dictionary<string, object?> flattenedData = new Dictionary<string, object?>();

            string trimmedJson = jsonContent.TrimStart();
            bool isObject = trimmedJson.StartsWith("{", StringComparison.Ordinal);
            bool isArray = trimmedJson.StartsWith("[", StringComparison.Ordinal);

            if (!isObject && !isArray)
            {
                throw new ArgumentException("Provided string is not valid JSON");
            }

            using JsonDocument document = JsonDocument.Parse(jsonContent);
            JsonElement root = document.RootElement;

            if (isObject && root.ValueKind == JsonValueKind.Object)
            {
                Flatten(root, flattenedData, null, changeCase, toUpper);
            }
            else if (isArray && root.ValueKind == JsonValueKind.Array)
            {
                int i = 0;
                foreach (JsonElement item in root.EnumerateArray())
                {
                    Flatten(item, flattenedData, $"[{i}]", changeCase, toUpper);
                    i++;
                }
            }
            else
            {
                throw new ArgumentException("Provided string is not valid JSON");
            }

            return flattenedData;
        }

        private static void Flatten(JsonElement token, Dictionary<string, object?> flattenedData, string? prefix, bool changeCase, bool toUpper)
        {
            switch (token.ValueKind)
            {
                case JsonValueKind.Object:
                    if (!string.IsNullOrEmpty(prefix))
                    {
                        flattenedData[ApplyCase(prefix, changeCase, toUpper)] = token.GetRawText();
                    }

                    foreach (JsonProperty property in token.EnumerateObject())
                    {
                        string propertyName = ApplyCase(property.Name, changeCase, toUpper);
                        string childPrefix = prefix != null ? $"{prefix}_{propertyName}" : propertyName;
                        Flatten(property.Value, flattenedData, childPrefix, changeCase, toUpper);
                    }
                    break;

                case JsonValueKind.Array:
                    JsonElement.ArrayEnumerator enumerator = token.EnumerateArray();
                    if (enumerator.MoveNext())
                    {
                        Flatten(enumerator.Current, flattenedData, prefix, changeCase, toUpper);
                    }
                    break;

                default:
                    if (prefix != null)
                    {
                        flattenedData[prefix] = GetScalarValue(token);
                    }
                    break;
            }
        }

        private static string ApplyCase(string value, bool changeCase, bool toUpper)
        {
            if (!changeCase)
            {
                return value;
            }
            return toUpper ? value.ToUpperInvariant() : value.ToLowerInvariant();
        }

        private static object? GetScalarValue(JsonElement token)
        {
            switch (token.ValueKind)
            {
                case JsonValueKind.String:
                    return token.GetString();
                case JsonValueKind.True:
                    return true;
                case JsonValueKind.False:
                    return false;
                case JsonValueKind.Null:
                    return null;
                case JsonValueKind.Number:
                    // Box as long when integral, otherwise as double (a ternary would unify both to double).
                    if (token.TryGetInt64(out long l))
                    {
                        return l;
                    }
                    return token.GetDouble();
                default:
                    return token.GetRawText();
            }
        }
    }
}
