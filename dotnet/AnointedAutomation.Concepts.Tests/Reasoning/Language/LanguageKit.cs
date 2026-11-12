// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AnointedAutomation.Concepts.Reasoning;
using AnointedAutomation.Concepts.Reasoning.Language;
using AnointedAutomation.Concepts.Tests.Reasoning.Canon;
using Microsoft.Extensions.Logging;

namespace AnointedAutomation.Concepts.Tests.Reasoning.Language
{
    /// <summary>
    /// Shared fixtures for the language engine tests: golden files, a fake model and a capturing logger.
    /// No test touches the network.
    /// </summary>
    internal static class LanguageKit
    {
        public const string ModelName = "fake-model-1";

        public static readonly ConceptRef Mercy = new ConceptRef(CanonFacts.AttributeKind, "Mercy");

        public static Query AbortionViolatesLove()
        {
            return CanonKit.Ask(CanonKit.Abortion, RelationKind.Violates, CanonKit.Love);
        }

        public static Query CompassionUpholdsMercyInContext()
        {
            return new Query(
                new Assertion(CanonKit.Compassion, RelationKind.Upholds, Mercy),
                new List<Circumstance> { new Circumstance("A neighbor is hungry"), new Circumstance("It is the Sabbath") });
        }

        /// <summary>Reads a golden file with line endings normalized to "\n".</summary>
        public static string Golden(string name)
        {
            return File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Reasoning", "Language", "Golden", name)).Replace("\r\n", "\n");
        }

        public static InterpretationParser Parser(CapturingLogger<InterpretationParser> logger)
        {
            return new InterpretationParser(logger);
        }

        /// <summary>A model that returns a fixed reply and records every prompt it was sent.</summary>
        internal sealed class FakeLanguageModel : ILanguageModel
        {
            private readonly string reply;

            public FakeLanguageModel(string reply)
            {
                this.reply = reply;
            }

            public List<string> Prompts { get; } = new List<string>();

            public string Name => ModelName;

            public Task<string> Complete(string prompt, CancellationToken ct)
            {
                ct.ThrowIfCancellationRequested();
                Prompts.Add(prompt);
                return Task.FromResult(reply);
            }
        }

        /// <summary>A logger that keeps every entry for assertions.</summary>
        internal sealed class CapturingLogger<T> : ILogger<T>
        {
            public List<(LogLevel Level, string Message, Exception Exception)> Entries { get; } = new List<(LogLevel, string, Exception)>();

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                Entries.Add((logLevel, formatter(state, exception), exception));
            }
        }
    }
}
