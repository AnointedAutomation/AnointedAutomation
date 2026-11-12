// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// Strictly parses a language model reply into an <see cref="Interpretation"/>. Any deviation
    /// from <see cref="PromptComposer.ReplySchema"/> (malformed JSON, a missing, extra or duplicate
    /// field, a truth outside true/false/unknown, a confidence outside 0..1, a scripture reference
    /// that does not parse or is out of chapter bounds, or a SinId not in <see cref="SinCatalog"/>)
    /// rejects the whole reply with <see cref="InterpretationRejected"/>, logged in full. A reply is
    /// never patched.
    /// </summary>
    public sealed class InterpretationParser
    {
        private static readonly string[] RootFields = { "classification", "truth", "confidence", "explanation", "scripture", "sinIds", "assertions" };
        private static readonly string[] ConceptFields = { "kind", "id" };
        private static readonly string[] AssertionFields = { "subject", "relation", "object" };

        /// <summary>
        /// The concept kinds a reply may use; any other kind (including a different casing) is rejected.
        /// </summary>
        public static readonly IReadOnlyList<string> KnownKinds = Array.AsReadOnly(new[]
        {
            ConceptRef.SinKind,
            ConceptRef.MoralConceptKind,
            CanonFacts.AttributeKind,
            CanonFacts.CategoryKind,
            CanonFacts.CoarseSinKind,
            CanonFacts.ScriptureKind,
            CanonFacts.CharacterKind,
        });

        private readonly ILogger<InterpretationParser> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InterpretationParser"/> class.
        /// </summary>
        /// <param name="logger">Receives every rejection with the full reply.</param>
        public InterpretationParser(ILogger<InterpretationParser> logger)
        {
            this.logger = ReasoningGuard.NotNull(logger, nameof(logger));
        }

        /// <summary>
        /// Parses a reply.
        /// </summary>
        /// <param name="reply">The raw reply text.</param>
        /// <param name="promptId">The prompt id the reply answers, for the log.</param>
        /// <param name="model">The model name, for the log.</param>
        /// <returns>The validated interpretation.</returns>
        /// <exception cref="InterpretationRejected">The reply is not a valid interpretation.</exception>
        public Interpretation Parse(string reply, string promptId, string model)
        {
            try
            {
                return ParseStrict(reply);
            }
            catch (InterpretationRejected ex)
            {
                logger.LogError(ex, "Language reply rejected for prompt {PromptId} from model {Model}: {Reason}. Reply: {Reply}", promptId, model, ex.Reason, reply);
                throw;
            }
            catch (JsonException ex)
            {
                throw Rejected("malformed JSON: " + ex.Message, reply, ex, promptId, model);
            }
            catch (FormatException ex)
            {
                throw Rejected("invalid value: " + ex.Message, reply, ex, promptId, model);
            }
            catch (ArgumentException ex)
            {
                throw Rejected("value out of range: " + ex.Message, reply, ex, promptId, model);
            }
        }

        /// <summary>
        /// Resolves a SinId name that is catalogued and not omitted. Numeric or combined names are refused.
        /// </summary>
        /// <param name="name">The exact enum member name.</param>
        /// <param name="sin">The sin when found.</param>
        /// <returns>True when the name is a catalogued sin.</returns>
        internal static bool TryCataloguedSin(string name, out SinId sin)
        {
            sin = default(SinId);
            if (string.IsNullOrEmpty(name) || !char.IsLetter(name[0]) || !Enum.TryParse(name, false, out SinId parsed)
                || !parsed.ToString().Equals(name, StringComparison.Ordinal))
            {
                return false;
            }

            if (!SinCatalog.TryGet(parsed, out SinEntry entry) || entry.IsOmitted)
            {
                return false;
            }

            sin = parsed;
            return true;
        }

        /// <summary>
        /// Builds, logs and returns a rejection that wraps a lower-level parse error.
        /// </summary>
        private InterpretationRejected Rejected(string reason, string reply, Exception inner, string promptId, string model)
        {
            InterpretationRejected rejected = new InterpretationRejected(reason, reply, inner);
            logger.LogError(rejected, "Language reply rejected for prompt {PromptId} from model {Model}: {Reason}. Reply: {Reply}", promptId, model, reason, reply);
            return rejected;
        }

        private static Interpretation ParseStrict(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
            {
                throw new InterpretationRejected("the reply is empty.", reply, null);
            }

            using (JsonDocument document = JsonDocument.Parse(reply))
            {
                JsonElement root = document.RootElement;
                RequireExactFields(root, "reply", RootFields, reply);

                List<ConceptRef> classification = new List<ConceptRef>();
                foreach (JsonElement item in RequireArray(root, "classification", reply).EnumerateArray())
                {
                    classification.Add(ReadConcept(item, "classification item", reply));
                }

                TruthValue truth = ReadTruth(RequireString(root, "truth", reply), reply);

                JsonElement confidenceElement = root.GetProperty("confidence");
                if (confidenceElement.ValueKind != JsonValueKind.Number || !confidenceElement.TryGetDouble(out double confidence)
                    || !double.IsFinite(confidence) || confidence < 0.0 || confidence > 1.0)
                {
                    throw new InterpretationRejected("confidence must be a number from 0 to 1, got " + confidenceElement.GetRawText() + ".", reply, null);
                }

                string explanation = RequireString(root, "explanation", reply);
                if (string.IsNullOrWhiteSpace(explanation))
                {
                    throw new InterpretationRejected("explanation is blank.", reply, null);
                }

                List<ScriptureReference> scripture = new List<ScriptureReference>();
                foreach (JsonElement item in RequireArray(root, "scripture", reply).EnumerateArray())
                {
                    scripture.Add(ScriptureReference.Parse(RequireStringValue(item, "scripture item", reply)));
                }

                List<SinId> sinIds = new List<SinId>();
                foreach (JsonElement item in RequireArray(root, "sinIds", reply).EnumerateArray())
                {
                    sinIds.Add(ReadSin(RequireStringValue(item, "sinIds item", reply), reply));
                }

                List<Assertion> assertions = new List<Assertion>();
                foreach (JsonElement item in RequireArray(root, "assertions", reply).EnumerateArray())
                {
                    RequireExactFields(item, "assertion", AssertionFields, reply);
                    assertions.Add(new Assertion(
                        ReadConcept(item.GetProperty("subject"), "assertion subject", reply),
                        ReadRelation(RequireString(item, "relation", reply), reply),
                        ReadConcept(item.GetProperty("object"), "assertion object", reply)));
                }

                return new Interpretation(classification, truth, confidence, explanation, scripture, sinIds, assertions);
            }
        }

        /// <summary>
        /// Requires an object with exactly the given fields, each once.
        /// </summary>
        private static void RequireExactFields(JsonElement element, string where, string[] fields, string reply)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new InterpretationRejected(where + " must be a JSON object.", reply, null);
            }

            HashSet<string> allowed = new HashSet<string>(fields, StringComparer.Ordinal);
            HashSet<string> seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (!allowed.Contains(property.Name))
                {
                    throw new InterpretationRejected(where + " has unknown field '" + property.Name + "'.", reply, null);
                }

                if (!seen.Add(property.Name))
                {
                    throw new InterpretationRejected(where + " repeats field '" + property.Name + "'.", reply, null);
                }
            }

            foreach (string field in fields)
            {
                if (!seen.Contains(field))
                {
                    throw new InterpretationRejected(where + " is missing field '" + field + "'.", reply, null);
                }
            }
        }

        private static JsonElement RequireArray(JsonElement element, string field, string reply)
        {
            JsonElement value = element.GetProperty(field);
            if (value.ValueKind != JsonValueKind.Array)
            {
                throw new InterpretationRejected(field + " must be an array.", reply, null);
            }

            return value;
        }

        private static string RequireString(JsonElement element, string field, string reply)
        {
            return RequireStringValue(element.GetProperty(field), field, reply);
        }

        private static string RequireStringValue(JsonElement value, string where, string reply)
        {
            if (value.ValueKind != JsonValueKind.String)
            {
                throw new InterpretationRejected(where + " must be a string.", reply, null);
            }

            return value.GetString();
        }

        private static ConceptRef ReadConcept(JsonElement element, string where, string reply)
        {
            RequireExactFields(element, where, ConceptFields, reply);
            string kind = RequireString(element, "kind", reply);
            string id = RequireString(element, "id", reply);
            if (string.IsNullOrWhiteSpace(kind) || string.IsNullOrWhiteSpace(id))
            {
                throw new InterpretationRejected(where + " needs a kind and an id.", reply, null);
            }

            if (!KnownKinds.Contains(kind))
            {
                throw new InterpretationRejected(where + " has unknown kind '" + kind + "'.", reply, null);
            }

            if (kind.Equals(ConceptRef.SinKind, StringComparison.Ordinal))
            {
                return ConceptRef.Of(ReadSin(id, reply));
            }

            return new ConceptRef(kind, id);
        }

        private static TruthValue ReadTruth(string text, string reply)
        {
            if (text.Equals("true", StringComparison.Ordinal))
            {
                return TruthValue.True;
            }

            if (text.Equals("false", StringComparison.Ordinal))
            {
                return TruthValue.False;
            }

            if (text.Equals("unknown", StringComparison.Ordinal))
            {
                return TruthValue.Unknown;
            }

            throw new InterpretationRejected("truth must be \"true\", \"false\" or \"unknown\", got \"" + text + "\".", reply, null);
        }

        private static SinId ReadSin(string name, string reply)
        {
            if (!TryCataloguedSin(name, out SinId sin))
            {
                throw new InterpretationRejected("'" + name + "' is not a SinId in the sin catalog.", reply, null);
            }

            return sin;
        }

        private static RelationKind ReadRelation(string name, string reply)
        {
            if (string.IsNullOrEmpty(name) || !char.IsLetter(name[0]) || !Enum.TryParse(name, false, out RelationKind relation)
                || !relation.ToString().Equals(name, StringComparison.Ordinal) || relation == RelationKind.None)
            {
                throw new InterpretationRejected("'" + name + "' is not a relation kind.", reply, null);
            }

            return relation;
        }
    }
}
