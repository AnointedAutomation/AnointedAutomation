// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AnointedAutomation.Concepts.Reasoning.Language
{
    /// <summary>
    /// Turns a <see cref="Query"/> into one deterministic English prompt. The prompt asks once for
    /// everything any consumer needs (classification, truth, confidence, explanation, scripture,
    /// candidate sins and suggested assertions) as one JSON object of an exact schema, so a query
    /// is never sent to a paid model twice.
    /// </summary>
    public static class PromptComposer
    {
        /// <summary>
        /// The exact JSON schema the reply must match. <see cref="InterpretationParser"/> enforces it.
        /// </summary>
        public const string ReplySchema =
            "{\n" +
            "  \"type\": \"object\",\n" +
            "  \"additionalProperties\": false,\n" +
            "  \"required\": [\"classification\", \"truth\", \"confidence\", \"explanation\", \"scripture\", \"sinIds\", \"assertions\"],\n" +
            "  \"properties\": {\n" +
            "    \"classification\": { \"type\": \"array\", \"items\": { \"$ref\": \"#/$defs/concept\" } },\n" +
            "    \"truth\": { \"enum\": [\"true\", \"false\", \"unknown\"] },\n" +
            "    \"confidence\": { \"type\": \"number\", \"minimum\": 0, \"maximum\": 1 },\n" +
            "    \"explanation\": { \"type\": \"string\", \"minLength\": 1 },\n" +
            "    \"scripture\": { \"type\": \"array\", \"items\": { \"type\": \"string\" } },\n" +
            "    \"sinIds\": { \"type\": \"array\", \"items\": { \"type\": \"string\" } },\n" +
            "    \"assertions\": {\n" +
            "      \"type\": \"array\",\n" +
            "      \"items\": {\n" +
            "        \"type\": \"object\",\n" +
            "        \"additionalProperties\": false,\n" +
            "        \"required\": [\"subject\", \"relation\", \"object\"],\n" +
            "        \"properties\": {\n" +
            "          \"subject\": { \"$ref\": \"#/$defs/concept\" },\n" +
            "          \"relation\": { \"type\": \"string\" },\n" +
            "          \"object\": { \"$ref\": \"#/$defs/concept\" }\n" +
            "        }\n" +
            "      }\n" +
            "    }\n" +
            "  },\n" +
            "  \"$defs\": {\n" +
            "    \"concept\": {\n" +
            "      \"type\": \"object\",\n" +
            "      \"additionalProperties\": false,\n" +
            "      \"required\": [\"kind\", \"id\"],\n" +
            "      \"properties\": { \"kind\": { \"type\": \"string\", \"minLength\": 1 }, \"id\": { \"type\": \"string\", \"minLength\": 1 } }\n" +
            "    }\n" +
            "  }\n" +
            "}";

        /// <summary>
        /// Composes the prompt for a query. Equal queries always give the same text and id.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The prompt and its hash-derived id.</returns>
        public static ComposedPrompt Compose(Query query)
        {
            ReasoningGuard.NotNull(query, nameof(query));
            Assertion assertion = query.Assertion;
            StringBuilder text = new StringBuilder();
            text.Append("You are interpreting a moral question against Scripture for the AnointedAutomation.Concepts reasoning engine.\n\n");
            text.Append("Question: does this assertion hold?\n");
            text.Append("Subject: ").Append(assertion.Subject).Append('\n');
            text.Append("Relation: ").Append(assertion.Relation).Append('\n');
            text.Append("Object: ").Append(assertion.Object).Append("\n\n");

            text.Append("Scripture already tied to these concepts:\n");
            AppendScripture(text, assertion.Subject);
            AppendScripture(text, assertion.Object);
            text.Append('\n');

            text.Append("Context:\n");
            if (query.Context.Count == 0)
            {
                text.Append("- none\n");
            }

            foreach (Circumstance circumstance in query.Context)
            {
                text.Append("- ").Append(circumstance.Name).Append('\n');
            }

            text.Append('\n');
            text.Append("Answer everything below in this one reply. Reply with ONLY one JSON object, with no prose and no code fences, ");
            text.Append("matching this JSON schema exactly. A missing, extra or invalid field rejects the whole reply.\n");
            text.Append(ReplySchema).Append("\n\n");
            text.Append("Rules:\n");
            text.Append("- \"classification\": every concept the situation falls under, as {\"kind\", \"id\"}; \"kind\" is one of: ")
                .Append(string.Join(", ", InterpretationParser.KnownKinds)).Append(".\n");
            text.Append("- \"truth\": \"true\", \"false\" or \"unknown\". Answer \"unknown\" when Scripture does not settle it; never \"false\" for lack of proof.\n");
            text.Append("- \"confidence\": a number from 0 to 1.\n");
            text.Append("- \"explanation\": the reasoning in plain words.\n");
            text.Append("- \"scripture\": references of the form \"Book chapter:verse\" or \"Book chapter:verse-verse\", for example \"1 Corinthians 13:4\". Cite only verses that exist.\n");
            text.Append("- \"sinIds\": candidate SinId names from the AnointedAutomation sin catalog, for example \"Abortion\".\n");
            text.Append("- \"assertions\": suggested assertions; \"relation\" is one of: ").Append(RelationNames()).Append(".\n");

            string body = text.ToString();
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(body));
            return new ComposedPrompt("prompt-" + Convert.ToHexStringLower(hash).Substring(0, 16), body);
        }

        /// <summary>
        /// Writes the scripture the catalog ties to a concept, or "none recorded".
        /// </summary>
        private static void AppendScripture(StringBuilder text, ConceptRef concept)
        {
            text.Append("- ").Append(concept).Append(": ");
            List<string> references = new List<string>();
            if (concept.Kind.Equals(ConceptRef.SinKind, StringComparison.Ordinal)
                && InterpretationParser.TryCataloguedSin(concept.Id, out SinId sin))
            {
                foreach (ScriptureReference reference in SinCatalog.Get(sin).References)
                {
                    references.Add(reference.ToString());
                }
            }

            text.Append(references.Count == 0 ? "none recorded" : string.Join("; ", references)).Append('\n');
        }

        /// <summary>
        /// The relation names a suggestion may use, in declaration order, without None.
        /// </summary>
        private static string RelationNames()
        {
            List<string> names = new List<string>();
            foreach (RelationKind kind in Enum.GetValues<RelationKind>())
            {
                if (kind != RelationKind.None)
                {
                    names.Add(kind.ToString());
                }
            }

            return string.Join(", ", names);
        }
    }
}
