// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// The one stagedUploadsCreate builder: the request, the target reader and the multipart form an upload needs.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using AnointedAutomation.Shopify.GraphQL;

namespace AnointedAutomation.Shopify.Bulk
{
    /// <summary>One staged upload target: POST the parameters as form fields, then the file, to <see cref="Url"/>.</summary>
    public sealed record ShopifyStagedTarget(string Url, string ResourceUrl, IReadOnlyList<KeyValuePair<string, string>> Parameters)
    {
        /// <summary>The "key" form parameter: what bulkOperationRunMutation takes as stagedUploadPath. Throws when missing.</summary>
        public string Key
        {
            get
            {
                foreach (KeyValuePair<string, string> p in Parameters)
                {
                    if (string.Equals(p.Key, "key", StringComparison.Ordinal) && !string.IsNullOrEmpty(p.Value))
                    {
                        return p.Value;
                    }
                }
                throw new InvalidOperationException("The staged upload target has no 'key' parameter.");
            }
        }
    }

    /// <summary>stagedUploadsCreate request builder and target reader.</summary>
    public static class ShopifyStagedUploads
    {
        /// <summary>The payload field.</summary>
        public const string Field = "stagedUploadsCreate";

        /// <summary>The mutation text.</summary>
        public const string Mutation =
            "mutation StagedUploadsCreate($input: [StagedUploadInput!]!) { stagedUploadsCreate(input: $input) { stagedTargets { url resourceUrl parameters { name value } } userErrors { field message } } }";

        /// <summary>Resource for a bulk mutation's JSONL variables file.</summary>
        public const string BulkMutationVariables = "BULK_MUTATION_VARIABLES";

        /// <summary>MIME type of a JSONL file.</summary>
        public const string JsonlMimeType = "text/jsonl";

        /// <summary>Build a request for one target. <paramref name="fileSize"/> is required by some resources (VIDEO,
        /// MODEL_3D) and may be null otherwise.</summary>
        public static ShopifyGraphQlRequest Build(string resource, string filename, string mimeType, string httpMethod, long? fileSize)
        {
            Require(resource, nameof(resource));
            Require(filename, nameof(filename));
            Require(mimeType, nameof(mimeType));
            Require(httpMethod, nameof(httpMethod));
            Dictionary<string, object> input = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["resource"] = resource,
                ["filename"] = filename,
                ["mimeType"] = mimeType,
                ["httpMethod"] = httpMethod,
            };
            if (fileSize.HasValue)
            {
                if (fileSize.Value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(fileSize), fileSize, "File size must not be negative.");
                }
                input["fileSize"] = fileSize.Value.ToString(CultureInfo.InvariantCulture);
            }
            return new ShopifyGraphQlRequest(Mutation, new Dictionary<string, object>(StringComparer.Ordinal)
            {
                ["input"] = new[] { input },
            });
        }

        /// <summary>The request for one bulk mutation JSONL file (POST).</summary>
        public static ShopifyGraphQlRequest ForBulkMutationVariables(string filename) =>
            Build(BulkMutationVariables, filename, JsonlMimeType, "POST", null);

        /// <summary>The first target of a stagedUploadsCreate payload. Throws when there is none or it is malformed.</summary>
        public static ShopifyStagedTarget ReadTarget(JsonElement payload)
        {
            if (payload.ValueKind != JsonValueKind.Object
                || !payload.TryGetProperty("stagedTargets", out JsonElement targets) || targets.ValueKind != JsonValueKind.Array
                || targets.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("stagedUploadsCreate returned no staged target.");
            }
            JsonElement t = targets[0];
            string url = Str(t, "url");
            if (string.IsNullOrEmpty(url))
            {
                throw new InvalidOperationException("stagedUploadsCreate returned a target without a url.");
            }
            List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>();
            if (t.TryGetProperty("parameters", out JsonElement ps) && ps.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement p in ps.EnumerateArray())
                {
                    string name = Str(p, "name");
                    if (string.IsNullOrEmpty(name))
                    {
                        continue;
                    }
                    string value = Str(p, "value");
                    if (value == null)
                    {
                        throw new InvalidOperationException("stagedUploadsCreate target " + url + " has parameter '" + name
                            + "' without a string value.");
                    }
                    parameters.Add(new KeyValuePair<string, string>(name, value));
                }
            }
            return new ShopifyStagedTarget(url, Str(t, "resourceUrl"), parameters);
        }

        internal static string Str(JsonElement obj, string name) =>
            obj.ValueKind == JsonValueKind.Object && obj.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.String
                ? v.GetString()
                : null;

        private static void Require(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException(name + " is required.", name);
            }
        }
    }
}
