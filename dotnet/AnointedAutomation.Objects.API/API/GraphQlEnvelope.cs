// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Vendor-neutral GraphQL response envelope reader. Consolidates the duplicated top-level errors[]
// checks and data.{mutation}.userErrors[] parsers that were hand-rolled across consuming apps
// (the shape matches Shopify Admin GraphQL, but nothing here is Shopify-specific).
// Edited 2026-10-28: errors carry codes, paths, userError fields, HTTP status and a body snippet for log readers.

using System;
using System.Collections.Generic;
using System.Text.Json;
using AnointedAutomation.Serialization.Json;

namespace AnointedAutomation.Objects.API
{
    /// <summary>
    /// A single GraphQL error entry from a top-level errors[] array.
    /// </summary>
    public sealed class GraphQlError
    {
        /// <summary>
        /// Creates a GraphQL error entry.
        /// </summary>
        /// <param name="message">Human-readable error message</param>
        /// <param name="path">Raw JSON of the error's path array, or null when absent</param>
        /// <param name="extensions">Raw JSON of the error's extensions object, or null when absent</param>
        public GraphQlError(string message, string path, string extensions)
        {
            Message = message;
            Path = path;
            Extensions = extensions;
        }

        /// <summary>
        /// Human-readable error message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Raw JSON text of the error's path array, or null when the error carried no path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Raw JSON text of the error's extensions object, or null when the error carried no extensions.
        /// </summary>
        public string Extensions { get; }
    }

    /// <summary>
    /// Parsed view over a GraphQL response body. Obtain via <see cref="GraphQlEnvelope.Read(string, int?)"/>.
    /// </summary>
    public sealed class GraphQlResult
    {
        private static readonly IReadOnlyList<string> EmptyMessages = Array.Empty<string>();

        internal GraphQlResult(JsonElement data, bool hasData, IReadOnlyList<string> transportErrors, IReadOnlyList<GraphQlError> errors)
        {
            Data = data;
            HasData = hasData;
            TransportErrors = transportErrors;
            Errors = errors;
        }

        /// <summary>
        /// The response's top-level "data" element (a clone, safe to hold), or default when absent.
        /// </summary>
        public JsonElement Data { get; }

        /// <summary>
        /// True when the response contained a non-null top-level "data" object.
        /// </summary>
        public bool HasData { get; }

        /// <summary>
        /// Messages from the top-level errors[] array. Also carries a single parse-failure entry when the
        /// body was empty, null, or not valid JSON.
        /// </summary>
        public IReadOnlyList<string> TransportErrors { get; }

        /// <summary>
        /// Structured top-level errors[] entries (message plus raw path/extensions), when present.
        /// </summary>
        public IReadOnlyList<GraphQlError> Errors { get; }

        /// <summary>
        /// Extracts data.{mutationField}.userErrors[].message. Any missing link in that chain yields an
        /// empty list; never throws.
        /// </summary>
        /// <param name="mutationField">Name of the mutation field under "data" (e.g. "productUpdate")</param>
        /// <returns>User error messages, or an empty list when none exist</returns>
        public IReadOnlyList<string> UserErrors(string mutationField)
        {
            JsonElement? payload = Payload(mutationField);
            if (!payload.HasValue || payload.Value.ValueKind != JsonValueKind.Object)
            {
                return EmptyMessages;
            }

            if (!payload.Value.TryGetProperty("userErrors", out JsonElement userErrors)
                || userErrors.ValueKind != JsonValueKind.Array)
            {
                return EmptyMessages;
            }

            List<string> messages = new List<string>();
            foreach (JsonElement error in userErrors.EnumerateArray())
            {
                if (error.ValueKind == JsonValueKind.Object
                    && error.TryGetProperty("message", out JsonElement message)
                    && message.ValueKind == JsonValueKind.String)
                {
                    messages.Add(message.GetString());
                }
            }
            return messages;
        }

        /// <summary>
        /// Like <see cref="UserErrors(string)"/>, but each entry also names the userError's <c>field</c> and
        /// <c>code</c> when present: <c>"Title cannot be blank [field=title] [code=BLANK]"</c>.
        /// </summary>
        /// <param name="mutationField">Name of the mutation field under "data"</param>
        /// <returns>Described user errors, or an empty list when none exist</returns>
        public IReadOnlyList<string> UserErrorDetails(string mutationField)
        {
            JsonElement? payload = Payload(mutationField);
            if (!payload.HasValue || payload.Value.ValueKind != JsonValueKind.Object
                || !payload.Value.TryGetProperty("userErrors", out JsonElement userErrors)
                || userErrors.ValueKind != JsonValueKind.Array)
            {
                return EmptyMessages;
            }

            List<string> described = new List<string>();
            foreach (JsonElement error in userErrors.EnumerateArray())
            {
                if (error.ValueKind != JsonValueKind.Object
                    || !error.TryGetProperty("message", out JsonElement message)
                    || message.ValueKind != JsonValueKind.String)
                {
                    continue;
                }
                string text = message.GetString();
                if (error.TryGetProperty("field", out JsonElement field) && field.ValueKind != JsonValueKind.Null && field.ValueKind != JsonValueKind.Undefined)
                {
                    text += " [field=" + DescribeField(field) + "]";
                }
                if (error.TryGetProperty("code", out JsonElement code) && code.ValueKind == JsonValueKind.String)
                {
                    text += " [code=" + code.GetString() + "]";
                }
                described.Add(text);
            }
            return described;
        }

        /// <summary>
        /// The first error for the mutation, transport errors first, then user errors. The message carries what a
        /// log reader needs to act: the GraphQL <c>extensions.code</c> (THROTTLED, ACCESS_DENIED...) and <c>path</c>
        /// for a transport error, <c>field</c> and <c>code</c> for a userError, and how many more errors there were.
        /// A message with none of those is returned unchanged.
        /// </summary>
        /// <param name="mutationField">Name of the mutation field under "data"</param>
        /// <returns>The first error, or null when there are none</returns>
        public string FirstError(string mutationField)
        {
            if (TransportErrors.Count > 0)
            {
                string first = TransportErrors[0];
                if (Errors.Count > 0)
                {
                    GraphQlError error = Errors[0];
                    string code = ExtensionsCode(error.Extensions);
                    if (code != null)
                    {
                        first += " [code=" + code + "]";
                    }
                    if (error.Path != null)
                    {
                        first += " [path=" + error.Path + "]";
                    }
                }
                return TransportErrors.Count > 1 ? first + " (+" + (TransportErrors.Count - 1) + " more)" : first;
            }

            IReadOnlyList<string> userErrors = UserErrorDetails(mutationField);
            if (userErrors.Count == 0)
            {
                return null;
            }
            return userErrors.Count > 1 ? userErrors[0] + " (+" + (userErrors.Count - 1) + " more)" : userErrors[0];
        }

        /// <summary>A userError field: a JSON array of path segments joined with dots, or the raw text.</summary>
        /// <param name="field">The field element.</param>
        /// <returns>The description.</returns>
        private static string DescribeField(JsonElement field)
        {
            if (field.ValueKind != JsonValueKind.Array)
            {
                return field.ValueKind == JsonValueKind.String ? field.GetString() : field.GetRawText();
            }
            List<string> parts = new List<string>();
            foreach (JsonElement part in field.EnumerateArray())
            {
                parts.Add(part.ValueKind == JsonValueKind.String ? part.GetString() : part.GetRawText());
            }
            return string.Join(".", parts);
        }

        /// <summary>extensions.code from a raw extensions object, or null.</summary>
        /// <param name="extensions">Raw JSON of the extensions object, or null.</param>
        /// <returns>The code, or null.</returns>
        private static string ExtensionsCode(string extensions)
        {
            if (string.IsNullOrEmpty(extensions))
            {
                return null;
            }
            try
            {
                using (JsonDocument document = JsonDocument.Parse(extensions))
                {
                    return document.RootElement.ValueKind == JsonValueKind.Object
                        && document.RootElement.TryGetProperty("code", out JsonElement code)
                        && code.ValueKind == JsonValueKind.String
                            ? code.GetString()
                            : null;
                }
            }
            catch (JsonException)
            {
                // Raw text came from a parsed document, so this cannot happen; no code is the honest answer.
                return null;
            }
        }

        /// <summary>
        /// True when the response has no transport errors and the mutation reported no userErrors.
        /// </summary>
        /// <param name="mutationField">Name of the mutation field under "data"</param>
        /// <returns>True on success</returns>
        public bool Succeeded(string mutationField) =>
            TransportErrors.Count == 0 && UserErrors(mutationField).Count == 0;

        /// <summary>
        /// The data.{mutationField} element when present and non-null, otherwise null.
        /// </summary>
        /// <param name="mutationField">Name of the mutation field under "data"</param>
        /// <returns>The mutation payload element, or null</returns>
        public JsonElement? Payload(string mutationField)
        {
            if (!HasData || string.IsNullOrEmpty(mutationField))
            {
                return null;
            }

            if (Data.ValueKind == JsonValueKind.Object
                && Data.TryGetProperty(mutationField, out JsonElement payload)
                && payload.ValueKind != JsonValueKind.Null
                && payload.ValueKind != JsonValueKind.Undefined)
            {
                return payload;
            }
            return null;
        }
    }

    /// <summary>
    /// Parses GraphQL response bodies into a <see cref="GraphQlResult"/>. Never throws: malformed or
    /// empty bodies become <see cref="GraphQlResult.TransportErrors"/> entries.
    /// </summary>
    public static class GraphQlEnvelope
    {
        /// <summary>
        /// Parses a GraphQL response body. Never throws.
        /// </summary>
        /// <param name="body">Raw HTTP response body; may be null, empty, or malformed</param>
        /// <returns>The parsed result</returns>
        public static GraphQlResult Read(string body)
        {
            return Read(body, null);
        }

        /// <summary>
        /// Parses a GraphQL response body. Never throws. A parse-failure entry names the HTTP status (when given),
        /// the byte where JSON parsing stopped and a short single-line snippet of the body (an HTML 502 or a
        /// Cloudflare page is recognizable from it).
        /// </summary>
        /// <param name="body">Raw HTTP response body; may be null, empty, or malformed</param>
        /// <param name="httpStatus">The HTTP status code of the response, or null when unknown</param>
        /// <returns>The parsed result</returns>
        public static GraphQlResult Read(string body, int? httpStatus)
        {
            string status = httpStatus.HasValue ? " (HTTP " + httpStatus.Value + ")" : string.Empty;
            if (string.IsNullOrWhiteSpace(body))
            {
                return new GraphQlResult(
                    default,
                    false,
                    new string[] { "GraphQL response body was empty" + status },
                    Array.Empty<GraphQlError>());
            }

            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(body);
            }
            catch (JsonException ex)
            {
                return new GraphQlResult(
                    default,
                    false,
                    new string[] { "GraphQL response body was not valid JSON" + status + " at line " + ex.LineNumber + " byte " + ex.BytePositionInLine + ": " + ResponseJson.Snippet(body) },
                    Array.Empty<GraphQlError>());
            }

            using (document)
            {
                JsonElement root = document.RootElement;
                if (root.ValueKind != JsonValueKind.Object)
                {
                    return new GraphQlResult(
                        default,
                        false,
                        new string[] { "GraphQL response body was not a JSON object" + status + ": " + ResponseJson.Snippet(body) },
                        Array.Empty<GraphQlError>());
                }

                JsonElement data = default;
                bool hasData = false;
                if (root.TryGetProperty("data", out JsonElement dataElement)
                    && dataElement.ValueKind != JsonValueKind.Null
                    && dataElement.ValueKind != JsonValueKind.Undefined)
                {
                    data = dataElement.Clone();
                    hasData = true;
                }

                List<string> transportErrors = new List<string>();
                List<GraphQlError> errors = new List<GraphQlError>();
                if (root.TryGetProperty("errors", out JsonElement errorsElement)
                    && errorsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (JsonElement error in errorsElement.EnumerateArray())
                    {
                        if (error.ValueKind != JsonValueKind.Object)
                        {
                            continue;
                        }

                        string message = error.TryGetProperty("message", out JsonElement messageElement)
                            && messageElement.ValueKind == JsonValueKind.String
                                ? messageElement.GetString()
                                : "GraphQL error without a message";
                        string path = error.TryGetProperty("path", out JsonElement pathElement)
                            && pathElement.ValueKind != JsonValueKind.Null
                                ? pathElement.GetRawText()
                                : null;
                        string extensions = error.TryGetProperty("extensions", out JsonElement extensionsElement)
                            && extensionsElement.ValueKind != JsonValueKind.Null
                                ? extensionsElement.GetRawText()
                                : null;

                        transportErrors.Add(message);
                        errors.Add(new GraphQlError(message, path, extensions));
                    }
                }

                return new GraphQlResult(data, hasData, transportErrors, errors);
            }
        }
    }
}
