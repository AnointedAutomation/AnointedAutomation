// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using System;
using System.Threading.Tasks;
using Xunit;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.Logging.Tests
{
    public class LogMessageTests
    {
        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            // Act
            var logMessage = new LogMessage();

            // Assert
            Assert.NotNull(logMessage);
        }

        [Fact]
        public void Constructor_WithMessageType_SetsProperties()
        {
            // Arrange
            var messageType = MessageType.Informational;
            var messageText = "Test message";
            
            // Act
            var logMessage = new LogMessage(messageType, messageText);

            // Assert
            Assert.Equal(messageType, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
            Assert.NotNull(logMessage.localOperationName);
            Assert.NotEqual(default(DateTime), logMessage.timeStamp);
            Assert.Equal(LogMessage.MessageSourceSetter, logMessage.messageSource);
        }

        [Fact]
        public void Constructor_WithIdAndMessageType_SetsProperties()
        {
            // Arrange
            int id = 42;
            var messageType = MessageType.Warning;
            var messageText = "Test warning message";
            
            // Act
            var logMessage = new LogMessage(id, messageType, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(messageType, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
            Assert.NotNull(logMessage.localOperationName);
            Assert.NotEqual(default(DateTime), logMessage.timeStamp);
            Assert.Equal(LogMessage.MessageSourceSetter, logMessage.messageSource);
        }

        [Fact]
        public void Celebrate_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Celebration message";
            
            // Act
            var logMessage = LogMessage.Celebrate(messageText);

            // Assert
            Assert.Equal(MessageType.Celebrate, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Celebrate_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 1;
            var messageText = "Celebration message with id";
            
            // Act
            var logMessage = LogMessage.Celebrate(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Celebrate, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Critical_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Critical message";
            
            // Act
            var logMessage = LogMessage.Critical(messageText);

            // Assert
            Assert.Equal(MessageType.Critical, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Critical_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 2;
            var messageText = "Critical message with id";
            
            // Act
            var logMessage = LogMessage.Critical(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Critical, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Error_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Error message";
            
            // Act
            var logMessage = LogMessage.Error(messageText);

            // Assert
            Assert.Equal(MessageType.Error, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Error_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 3;
            var messageText = "Error message with id";
            
            // Act
            var logMessage = LogMessage.Error(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Error, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Informational_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Informational message";
            
            // Act
            var logMessage = LogMessage.Informational(messageText);

            // Assert
            Assert.Equal(MessageType.Informational, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Informational_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 4;
            var messageText = "Informational message with id";
            
            // Act
            var logMessage = LogMessage.Informational(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Informational, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Message_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Regular message";
            
            // Act
            var logMessage = LogMessage.Message(messageText);

            // Assert
            Assert.Equal(MessageType.Message, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Message_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 5;
            var messageText = "Regular message with id";
            
            // Act
            var logMessage = LogMessage.Message(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Message, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Success_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Success message";
            
            // Act
            var logMessage = LogMessage.Success(messageText);

            // Assert
            Assert.Equal(MessageType.Success, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Success_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 6;
            var messageText = "Success message with id";
            
            // Act
            var logMessage = LogMessage.Success(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Success, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Warning_CreatesMessage_WithCorrectType()
        {
            // Arrange
            var messageText = "Warning message";
            
            // Act
            var logMessage = LogMessage.Warning(messageText);

            // Assert
            Assert.Equal(MessageType.Warning, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void Warning_WithId_CreatesMessage_WithCorrectTypeAndId()
        {
            // Arrange
            int id = 7;
            var messageText = "Warning message with id";
            
            // Act
            var logMessage = LogMessage.Warning(id, messageText);

            // Assert
            Assert.Equal(id, logMessage.id);
            Assert.Equal(MessageType.Warning, logMessage.messageType);
            Assert.Equal(messageText, logMessage.message);
        }

        [Fact]
        public void LogMessageEventArgs_Constructor_SetsLogProperty()
        {
            // Arrange
            var logMessage = new LogMessage(MessageType.Informational, "Test message");
            
            // Act
            var args = new LogMessageEventArgs(logMessage);
            
            // Assert
            Assert.Same(logMessage, args.log);
        }

        [Fact]
        public void MessageSourceSetter_CanBeModified()
        {
            // Arrange
            string originalValue = LogMessage.MessageSourceSetter;
            string newValue = "TestApplication";
            
            // Act
            LogMessage.MessageSourceSetter = newValue;
            var logMessage = new LogMessage(MessageType.Informational, "Test message");
            
            // Cleanup (restore original value)
            LogMessage.MessageSourceSetter = originalValue;
            
            // Assert
            Assert.Equal(newValue, logMessage.messageSource);
        }

        [Fact]
        public async Task AsyncMethod_LogsCorrectOperationName()
        {
            // Act
            LogMessage logMessage = null;
            await TestAsyncMethodAsync(() => 
            {
                logMessage = new LogMessage(MessageType.Informational, "Test async logging");
            });

            // Assert
            Assert.NotNull(logMessage);
            // Log the actual operation name for debugging
            Console.WriteLine($"Actual operation name: {logMessage.localOperationName}");
            Assert.NotNull(logMessage.localOperationName);
            Assert.DoesNotContain("MoveNext", logMessage.localOperationName);
        }

        private async Task TestAsyncMethodAsync(Action createLog)
        {
            await Task.Delay(1); // Simulate async work
            createLog();
        }

        [Fact]
        public void AsyncMethodDirectCall_LogsCorrectOperationName()
        {
            // Act
            LogMessage logMessage = null;
            Task.Run(async () =>
            {
                await Task.Delay(1);
                logMessage = new LogMessage(MessageType.Informational, "Test from async context");
            }).Wait();

            // Assert
            Assert.NotNull(logMessage);
            // Since this is from an anonymous async lambda, it should still capture something meaningful
            Assert.NotNull(logMessage.localOperationName);
            Assert.DoesNotContain("MoveNext", logMessage.localOperationName);
        }

        // EDGE CASE TESTS - Added per CLAUDE_TESTING.md requirements

        [Fact]
        public void Constructor_WithNullMessage_CreatesInstance()
        {
            // Arrange
            MessageType messageType = MessageType.Error;
            string message = null;

            // Act
            LogMessage logMessage = new LogMessage(messageType, message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(messageType, logMessage.messageType);
        }

        [Fact]
        public void Constructor_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            MessageType messageType = MessageType.Warning;
            string message = "";

            // Act
            LogMessage logMessage = new LogMessage(messageType, message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(messageType, logMessage.messageType);
        }

        [Fact]
        public void Constructor_WithVeryLongMessage_CreatesInstance()
        {
            // Arrange
            MessageType messageType = MessageType.Informational;
            string message = new string('A', 10000); // 10,000 character message

            // Act
            LogMessage logMessage = new LogMessage(messageType, message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal(message, logMessage.message);
            Assert.Equal(messageType, logMessage.messageType);
        }

        [Fact]
        public void Constructor_WithSpecialCharacters_CreatesInstance()
        {
            // Arrange
            MessageType messageType = MessageType.Message;
            string message = "Test with special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?\n\r\t";

            // Act
            LogMessage logMessage = new LogMessage(messageType, message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal(message, logMessage.message);
        }

        [Fact]
        public void Constructor_WithUnicodeCharacters_CreatesInstance()
        {
            // Arrange
            MessageType messageType = MessageType.Message;
            string message = "Unicode test: 你好世界 🌍 ñ é ü";

            // Act
            LogMessage logMessage = new LogMessage(messageType, message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal(message, logMessage.message);
        }

        [Fact]
        public void Critical_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Critical(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Critical, logMessage.messageType);
        }

        [Fact]
        public void Critical_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Critical(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Critical, logMessage.messageType);
        }

        [Fact]
        public void Error_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Error(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Error, logMessage.messageType);
        }

        [Fact]
        public void Error_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Error(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Error, logMessage.messageType);
        }

        [Fact]
        public void Warning_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Warning(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Warning, logMessage.messageType);
        }

        [Fact]
        public void Warning_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Warning(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Warning, logMessage.messageType);
        }

        [Fact]
        public void Informational_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Informational(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Informational, logMessage.messageType);
        }

        [Fact]
        public void Informational_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Informational(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Informational, logMessage.messageType);
        }

        [Fact]
        public void Success_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Success(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Success, logMessage.messageType);
        }

        [Fact]
        public void Success_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Success(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Success, logMessage.messageType);
        }

        [Fact]
        public void Celebrate_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Celebrate(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Celebrate, logMessage.messageType);
        }

        [Fact]
        public void Celebrate_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Celebrate(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Celebrate, logMessage.messageType);
        }

        [Fact]
        public void Message_WithNullMessage_CreatesInstance()
        {
            // Arrange
            string message = null;

            // Act
            LogMessage logMessage = LogMessage.Message(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Null(logMessage.message);
            Assert.Equal(MessageType.Message, logMessage.messageType);
        }

        [Fact]
        public void Message_WithEmptyMessage_CreatesInstance()
        {
            // Arrange
            string message = "";

            // Act
            LogMessage logMessage = LogMessage.Message(message);

            // Assert
            Assert.NotNull(logMessage);
            Assert.Equal("", logMessage.message);
            Assert.Equal(MessageType.Message, logMessage.messageType);
        }

        [Fact]
        public void Constructor_RaisesLogAddedEvent()
        {
            // Arrange
            bool eventRaised = false;
            LogMessage capturedLog = null;
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                eventRaised = true;
                capturedLog = e.log;
            };
            LogMessage.LogAdded += handler;

            string testMessage = "Event test message";

            try
            {
                // Act
                LogMessage logMessage = new LogMessage(MessageType.Informational, testMessage);

                // Assert
                Assert.True(eventRaised, "LogAdded event should be raised");
                Assert.NotNull(capturedLog);
                Assert.Equal(testMessage, capturedLog.message);
                Assert.Equal(MessageType.Informational, capturedLog.messageType);
            }
            finally
            {
                // Cleanup
                LogMessage.LogAdded -= handler;
            }
        }

        [Fact]
        public void ConstructorWithId_RaisesLogAddedEvent()
        {
            // Arrange
            bool eventRaised = false;
            LogMessage capturedLog = null;
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                eventRaised = true;
                capturedLog = e.log;
            };
            LogMessage.LogAdded += handler;

            int testId = 999;
            string testMessage = "Event test message with ID";

            try
            {
                // Act
                LogMessage logMessage = new LogMessage(testId, MessageType.Critical, testMessage);

                // Assert
                Assert.True(eventRaised, "LogAdded event should be raised");
                Assert.NotNull(capturedLog);
                Assert.Equal(testId, capturedLog.id);
                Assert.Equal(testMessage, capturedLog.message);
                Assert.Equal(MessageType.Critical, capturedLog.messageType);
            }
            finally
            {
                // Cleanup
                LogMessage.LogAdded -= handler;
            }
        }

        [Fact]
        public void FactoryMethod_RaisesLogAddedEvent()
        {
            // Arrange
            bool eventRaised = false;
            LogMessage capturedLog = null;
            LogMessage.LogAddedEventHandler handler = (sender, e) =>
            {
                eventRaised = true;
                capturedLog = e.log;
            };
            LogMessage.LogAdded += handler;

            string testMessage = "Factory method event test";

            try
            {
                // Act
                LogMessage logMessage = LogMessage.Error(testMessage);

                // Assert
                Assert.True(eventRaised, "LogAdded event should be raised from factory method");
                Assert.NotNull(capturedLog);
                Assert.Equal(testMessage, capturedLog.message);
                Assert.Equal(MessageType.Error, capturedLog.messageType);
            }
            finally
            {
                // Cleanup
                LogMessage.LogAdded -= handler;
            }
        }

        [Fact]
        public void Constructor_SetsTimestampToCurrentTime()
        {
            // Arrange
            DateTime before = DateTime.Now;

            // Act
            LogMessage logMessage = new LogMessage(MessageType.Informational, "Timestamp test");
            DateTime after = DateTime.Now;

            // Assert
            Assert.True(logMessage.timeStamp >= before, "Timestamp should be >= time before creation");
            Assert.True(logMessage.timeStamp <= after, "Timestamp should be <= time after creation");
        }

        [Fact]
        public void Constructor_WithIdZero_SetsIdCorrectly()
        {
            // Arrange
            int id = 0;

            // Act
            LogMessage logMessage = new LogMessage(id, MessageType.Informational, "Test");

            // Assert
            Assert.Equal(0, logMessage.id);
        }

        [Fact]
        public void Constructor_WithNegativeId_SetsIdCorrectly()
        {
            // Arrange
            int id = -1;

            // Act
            LogMessage logMessage = new LogMessage(id, MessageType.Informational, "Test");

            // Assert
            Assert.Equal(-1, logMessage.id);
        }

        [Fact]
        public void Constructor_WithMaxIntId_SetsIdCorrectly()
        {
            // Arrange
            int id = int.MaxValue;

            // Act
            LogMessage logMessage = new LogMessage(id, MessageType.Informational, "Test");

            // Assert
            Assert.Equal(int.MaxValue, logMessage.id);
        }

        [Fact]
        public void LogMessageEventArgs_WithNullLog_StoresNull()
        {
            // Arrange
            LogMessage logMessage = null;

            // Act
            LogMessageEventArgs args = new LogMessageEventArgs(logMessage);

            // Assert
            Assert.Null(args.log);
        }
    }
}
