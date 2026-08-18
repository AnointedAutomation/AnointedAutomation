// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
//
// Vendor-neutral GraphQL response envelope reader. Consolidates the duplicated top-level errors[]
// checks and data.{mutation}.userErrors[] parsers that were hand-rolled across consuming apps
// (the shape matches Shopify Admin GraphQL, but nothing here is Shopify-specific).

using System;
using System.Collections.Generic;
using System.Text.Json;

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
    /// Parsed view over a GraphQL response body. Obtain via <see cref="GraphQlEnvelope.Read(string)"/>.
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
        /// The first error message for the mutation: transport errors first, then user errors.
        /// </summary>
        /// <param name="mutationField">Name of the mutation field under "data"</param>
        /// <returns>The first error message, or null when there are none</returns>
        public string FirstError(string mutationField)
        {
            if (TransportErrors.Count > 0)
            {
                return TransportErrors[0];
            }

            IReadOnlyList<string> userErrors = UserErrors(mutationField);
            return userErrors.Count > 0 ? userErrors[0] : null;
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
            if (string.IsNullOrWhiteSpace(body))
            {
                return new GraphQlResult(
                    default,
                    false,
                    new string[] { "GraphQL response body was empty" },
                    Array.Empty<GraphQlError>());
            }

            JsonDocument document;
            try
            {
                document = JsonDocument.Parse(body);
            }
            catch (JsonException)
            {
                return new GraphQlResult(
                    default,
                    false,
                    new string[] { "GraphQL response body was not valid JSON" },
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
                        new string[] { "GraphQL response body was not a JSON object" },
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
