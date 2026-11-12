// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// One summary produced by <see cref="LogThrottle"/> for a key that suppressed entries.
    /// </summary>
    public sealed class LogThrottleSummary
    {
        internal LogThrottleSummary(string category, MessageType level, int suppressed, string text)
        {
            Category = category;
            Level = level;
            Suppressed = suppressed;
            Text = text;
        }

        /// <summary>The category of the suppressed entries.</summary>
        public string Category { get; }

        /// <summary>The level of the suppressed entries (persist the summary itself as a Warning).</summary>
        public MessageType Level { get; }

        /// <summary>How many entries were suppressed.</summary>
        public int Suppressed { get; }

        /// <summary>Human-readable summary with the template and a few sample texts.</summary>
        public string Text { get; }
    }

    /// <summary>
    /// Shared log throttle so every path into a log store behaves the same. Per window: at most
    /// <see cref="MaxIdenticalPerWindow"/> rows with the same rendered text (category + level), and per (category, level,
    /// template) at most <see cref="MaxErrorsPerTemplatePerWindow"/> Errors or <see cref="MaxWarningsPerTemplatePerWindow"/>
    /// of any other level. Critical is never throttled. Every key that suppressed anything yields ONE
    /// <see cref="LogThrottleSummary"/> when its window ends (or when forced), collected with <see cref="TakeSummaries"/>.
    /// Thread safe.
    /// </summary>
    public sealed class LogThrottle
    {
        /// <summary>Most rows with the same rendered text (category + level) per window.</summary>
        public const int MaxIdenticalPerWindow = 5;

        /// <summary>Most non-Error rows one (category, level, template) may persist per window.</summary>
        public const int MaxWarningsPerTemplatePerWindow = 50;

        /// <summary>Most Error rows one (category, template) may persist per window.</summary>
        public const int MaxErrorsPerTemplatePerWindow = 200;

        /// <summary>Distinct keys kept before every pending count is summarized and the table is reset (bounds memory).</summary>
        public const int MaxKeys = 5000;

        /// <summary>Sample texts kept per suppressing key for its summary.</summary>
        public const int MaxSamplesPerKey = 5;

        /// <summary>Default window length.</summary>
        public static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(10);

        private readonly ConcurrentDictionary<string, State> _states = new ConcurrentDictionary<string, State>(StringComparer.Ordinal);
        private readonly ConcurrentQueue<LogThrottleSummary> _ready = new ConcurrentQueue<LogThrottleSummary>();
        private readonly Func<DateTime> _utcNow;
        private readonly TimeSpan _window;
        private readonly object _resetSync = new object();

        /// <summary>
        /// Creates a throttle with the <see cref="DefaultWindow"/> and the system UTC clock.
        /// </summary>
        public LogThrottle()
            : this(DefaultWindow, () => DateTime.UtcNow)
        {
        }

        /// <summary>
        /// Creates a throttle with an explicit window and clock (tests inject the clock).
        /// </summary>
        /// <param name="window">Window length; must be positive.</param>
        /// <param name="utcNow">The UTC clock.</param>
        public LogThrottle(TimeSpan window, Func<DateTime> utcNow)
        {
            if (window <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(window), window, "The throttle window must be positive.");
            }
            _window = window;
            _utcNow = utcNow ?? throw new ArgumentNullException(nameof(utcNow));
        }

        /// <summary>
        /// Decides whether one entry may be persisted, counting it either way.
        /// </summary>
        /// <param name="category">The entry's category.</param>
        /// <param name="level">The entry's level; Critical always passes.</param>
        /// <param name="template">The message template (ILogger format) or null to use the rendered text.</param>
        /// <param name="rendered">The final message text.</param>
        /// <returns>True to persist, false when suppressed.</returns>
        public bool TryPass(string category, MessageType level, string template, string rendered)
        {
            if (level == MessageType.Critical)
            {
                return true;
            }
            if (rendered == null)
            {
                throw new ArgumentNullException(nameof(rendered));
            }
            DateTime now = _utcNow();
            if (_states.Count > MaxKeys)
            {
                SummarizeAll(now, force: true, clear: true);
            }
            string identicalKey = "I|" + category + "|" + level + "|" + rendered;
            if (!Pass(identicalKey, category, rendered, level, rendered, MaxIdenticalPerWindow, now))
            {
                return false;
            }
            string shape = template == null ? rendered : template;
            string templateKey = "T|" + category + "|" + level + "|" + shape;
            int cap = level == MessageType.Error ? MaxErrorsPerTemplatePerWindow : MaxWarningsPerTemplatePerWindow;
            return Pass(templateKey, category, shape, level, rendered, cap, now);
        }

        /// <summary>
        /// Returns the summaries that are ready: windows that rolled over in <see cref="TryPass"/>, plus every key whose
        /// window has ended. With <paramref name="force"/>, every pending count is summarized now (call it at shutdown so
        /// a suppressed count is never lost).
        /// </summary>
        /// <param name="force">Summarize keys whose window is still open too.</param>
        /// <returns>The summaries, possibly empty.</returns>
        public IReadOnlyList<LogThrottleSummary> TakeSummaries(bool force)
        {
            SummarizeAll(_utcNow(), force, clear: false);
            List<LogThrottleSummary> taken = new List<LogThrottleSummary>();
            while (_ready.TryDequeue(out LogThrottleSummary summary))
            {
                taken.Add(summary);
            }
            return taken;
        }

        private void SummarizeAll(DateTime now, bool force, bool clear)
        {
            lock (_resetSync)
            {
                List<string> finished = new List<string>();
                foreach (KeyValuePair<string, State> pair in _states)
                {
                    State state = pair.Value;
                    lock (state)
                    {
                        bool windowOver = now - state.WindowStart >= _window;
                        if (!windowOver && !force)
                        {
                            continue;
                        }
                        if (state.Suppressed > 0)
                        {
                            _ready.Enqueue(Summarize(state, now));
                        }
                        state.Suppressed = 0;
                        state.Samples.Clear();
                        if (windowOver)
                        {
                            finished.Add(pair.Key);
                        }
                    }
                }
                if (clear)
                {
                    // Every pending count was just summarized, so trimming the table loses nothing.
                    _states.Clear();
                    return;
                }
                foreach (string key in finished)
                {
                    _states.TryRemove(key, out State _);
                }
            }
        }

        private bool Pass(string key, string category, string shape, MessageType level, string rendered, int cap, DateTime now)
        {
            State state = _states.GetOrAdd(key, _ => new State(now, category, shape, level));
            lock (state)
            {
                if (now - state.WindowStart >= _window)
                {
                    if (state.Suppressed > 0)
                    {
                        _ready.Enqueue(Summarize(state, now));
                    }
                    state.WindowStart = now;
                    state.Count = 0;
                    state.Suppressed = 0;
                    state.Samples.Clear();
                }
                if (state.Count < cap)
                {
                    state.Count++;
                    return true;
                }
                state.Suppressed++;
                string sample = Clip(rendered, 300);
                if (state.Samples.Count < MaxSamplesPerKey && !state.Samples.Contains(sample))
                {
                    state.Samples.Add(sample);
                }
                return false;
            }
        }

        private static LogThrottleSummary Summarize(State state, DateTime now)
        {
            StringBuilder text = new StringBuilder();
            text.Append(string.Create(CultureInfo.InvariantCulture,
                $"[{state.Suppressed} similar {state.Level} entr(ies) suppressed by the log throttle between {state.WindowStart:u} and {now:u}] "));
            text.Append(state.Category).Append(": '").Append(Clip(state.Template, 300)).Append('\'');
            if (state.Samples.Count > 0)
            {
                text.Append("; suppressed samples: ").Append(string.Join(" | ", state.Samples));
            }
            return new LogThrottleSummary(state.Category, state.Level, state.Suppressed, text.ToString());
        }

        private static string Clip(string text, int max) => text.Length <= max ? text : text.Substring(0, max);

        private sealed class State
        {
            internal State(DateTime windowStart, string category, string template, MessageType level)
            {
                WindowStart = windowStart;
                Category = category;
                Template = template;
                Level = level;
            }

            internal DateTime WindowStart;
            internal int Count;
            internal int Suppressed;
            internal readonly List<string> Samples = new List<string>();
            internal string Category { get; }
            internal string Template { get; }
            internal MessageType Level { get; }
        }
    }
}
