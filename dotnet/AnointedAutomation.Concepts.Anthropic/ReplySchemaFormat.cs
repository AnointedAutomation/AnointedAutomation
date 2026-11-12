// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using AnointedAutomation.Concepts.Reasoning.Language;

namespace AnointedAutomation.Concepts.Anthropic
{
    /// <summary>
    /// Turns the core <see cref="PromptComposer.ReplySchema"/> into the JSON schema sent as Claude's structured output
    /// format. Structured outputs do not accept numeric or string length bounds, so those keywords are removed here;
    /// the core <see cref="InterpretationParser"/> still enforces them on the reply.
    /// </summary>
    internal static class ReplySchemaFormat
    {
        internal static readonly string[] UnsupportedKeywords = { "minimum", "maximum", "minLength" };

        internal static Dictionary<string, JsonElement> Create()
        {
            JsonObject root = JsonNode.Parse(PromptComposer.ReplySchema).AsObject();
            Strip(root);
            Dictionary<string, JsonElement> schema = new Dictionary<string, JsonElement>();
            foreach (KeyValuePair<string, JsonNode> property in root)
            {
                schema[property.Key] = JsonSerializer.SerializeToElement(property.Value);
            }

            return schema;
        }

        private static void Strip(JsonNode node)
        {
            JsonObject obj = node as JsonObject;
            if (obj != null)
            {
                foreach (string keyword in UnsupportedKeywords)
                {
                    obj.Remove(keyword);
                }

                foreach (JsonNode child in obj.Select(p => p.Value).Where(v => v != null).ToList())
                {
                    Strip(child);
                }

                return;
            }

            JsonArray array = node as JsonArray;
            if (array != null)
            {
                foreach (JsonNode child in array.Where(v => v != null).ToList())
                {
                    Strip(child);
                }
            }
        }
    }
}
