// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-28 Jesus is King ✝️
using System;
using System.Collections.Generic;
using System.Data;
using AnointedAutomation.Optimization;
using AnointedAutomation.Optimization.Logging;
using Xunit;

namespace AnointedAutomation.Optimization.Tests
{
    /// <summary>2026-10-28 logging audit: a value Utility drops is logged with the property it was meant for.</summary>
    public class DroppedValueLoggingTests
    {
        public class Holder
        {
            public int Count { get; set; }
            public string? Label { get; set; }
        }

        [Fact]
        public void GetObjectFromDataRow_UnconvertibleValue_LogsWarningNamingTheProperty()
        {
            List<LogMessage> seen = new List<LogMessage>();
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                if (e.log.message != null && e.log.message.Contains("Holder.Count", StringComparison.Ordinal))
                {
                    lock (seen)
                    {
                        seen.Add(e.log);
                    }
                }
            };
            LogMessage.LogAdded += handler;
            try
            {
                DataTable table = new DataTable();
                table.Columns.Add("Count", typeof(string));
                table.Columns.Add("Label", typeof(string));
                table.Rows.Add("not a number", "kept");

                Holder holder = Utility.GetObjectFromDataRow<Holder>(table.Rows[0]);

                Assert.Equal(0, holder.Count);
                Assert.Equal("kept", holder.Label);
                LogMessage warning = Assert.Single(seen);
                Assert.Equal(MessageType.Warning, warning.messageType);
                Assert.Equal(typeof(FormatException).FullName, warning.exceptionType);
                Assert.DoesNotContain("not a number", warning.message.Split(Environment.NewLine)[0]);
            }
            finally
            {
                LogMessage.LogAdded -= handler;
            }
        }
    }
}
