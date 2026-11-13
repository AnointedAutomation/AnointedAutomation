// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-13 Jesus is King ✝️
// Stewarded by Alexander Fields
using System;
using System.Collections.Generic;
using System.Text.Json;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.Concepts.Reasoning.Language.FreeText
{
    /// <summary>
    /// Strictly parses a reply to a free-text question. Any deviation from <see cref="FreeTextPromptComposer.ReplySchema"/>
    /// (malformed JSON, a missing, extra or duplicate field, a verdict or language outside its list, a confidence outside
    /// 0..1, a blank answer or word field, or a verse that does not parse or does not exist) rejects the whole reply with
    /// <see cref="InterpretationRejected"/>, logged in full at Error through LogMessage. A reply is never patched.
    /// </summary>
    public sealed class FreeTextAnswerParser
    {
        private static readonly string[] RootFields = { "answer", "verdict", "confidence", "scripture", "originalLanguageWords" };
        private static readonly string[] WordFields = { "language", "lemma", "transliteration", "gloss", "reference" };

        /// <summary>
        /// Parses a reply.
        /// </summary>
        /// <param name="reply">The raw reply text.</param>
        /// <param name="promptId">The prompt id the reply answers.</param>
        /// <param name="model">The model name.</param>
        /// <returns>The validated answer.</returns>
        /// <exception cref="InterpretationRejected">The reply is not a valid answer.</exception>
        public FreeTextAnswer Parse(string reply, string promptId, string model)
        {
            try
            {
                return ParseStrict(reply, promptId, model);
            }
            catch (InterpretationRejected ex)
            {
                LogRejected(ex, promptId, model);
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

        private static FreeTextAnswer ParseStrict(string reply, string promptId, string model)
        {
            if (string.IsNullOrWhiteSpace(reply))
            {
                throw new InterpretationRejected("the reply is empty.", reply, null);
            }

            using (JsonDocument document = JsonDocument.Parse(reply))
            {
                JsonElement root = document.RootElement;
                InterpretationParser.RequireExactFields(root, "reply", RootFields, reply);

                string answer = RequireNonBlank(root, "answer", reply);
                TruthValue verdict = InterpretationParser.ReadTruth(InterpretationParser.RequireString(root, "verdict", reply), reply);

                JsonElement confidenceElement = root.GetProperty("confidence");
                if (confidenceElement.ValueKind != JsonValueKind.Number || !confidenceElement.TryGetDouble(out double confidence)
                    || !double.IsFinite(confidence) || confidence < 0.0 || confidence > 1.0)
                {
                    throw new InterpretationRejected("confidence must be a number from 0 to 1, got " + confidenceElement.GetRawText() + ".", reply, null);
                }

                List<ScriptureReference> scripture = new List<ScriptureReference>();
                foreach (JsonElement item in InterpretationParser.RequireArray(root, "scripture", reply).EnumerateArray())
                {
                    scripture.Add(ScriptureReference.Parse(InterpretationParser.RequireStringValue(item, "scripture item", reply)));
                }

                List<OriginalLanguageWord> words = new List<OriginalLanguageWord>();
                foreach (JsonElement item in InterpretationParser.RequireArray(root, "originalLanguageWords", reply).EnumerateArray())
                {
                    InterpretationParser.RequireExactFields(item, "original-language word", WordFields, reply);
                    words.Add(new OriginalLanguageWord(
                        ReadLanguage(InterpretationParser.RequireString(item, "language", reply), reply),
                        RequireNonBlank(item, "lemma", reply),
                        RequireNonBlank(item, "transliteration", reply),
                        RequireNonBlank(item, "gloss", reply),
                        ScriptureReference.Parse(RequireNonBlank(item, "reference", reply))));
                }

                return new FreeTextAnswer(answer, verdict, confidence, scripture, words, promptId, model);
            }
        }

        private static string RequireNonBlank(JsonElement element, string field, string reply)
        {
            string value = InterpretationParser.RequireString(element, field, reply);
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InterpretationRejected(field + " is blank.", reply, null);
            }

            return value;
        }

        private static OriginalLanguage ReadLanguage(string text, string reply)
        {
            if (text.Equals("hebrew", StringComparison.Ordinal))
            {
                return OriginalLanguage.Hebrew;
            }

            if (text.Equals("aramaic", StringComparison.Ordinal))
            {
                return OriginalLanguage.Aramaic;
            }

            if (text.Equals("greek", StringComparison.Ordinal))
            {
                return OriginalLanguage.Greek;
            }

            throw new InterpretationRejected("language must be hebrew, aramaic or greek, got '" + text + "'.", reply, null);
        }

        private static InterpretationRejected Rejected(string reason, string reply, Exception inner, string promptId, string model)
        {
            InterpretationRejected rejected = new InterpretationRejected(reason, reply, inner);
            LogRejected(rejected, promptId, model);
            return rejected;
        }

        private static void LogRejected(InterpretationRejected rejected, string promptId, string model)
        {
            LogMessage.Error(
                "Free-text reply rejected for prompt " + promptId + " from model " + model + ": " + rejected.Reason + ". Reply: " + rejected.Reply,
                rejected);
        }
    }
}
