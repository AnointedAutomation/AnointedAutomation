// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using Xunit;
using AnointedAutomation.Optimization.Logging;

namespace AnointedAutomation.Logging.Tests
{
    public class MessageTypeTests
    {
        [Fact]
        public void MessageType_HasCorrectValues()
        {
            // Assert
            Assert.Equal(0, (int)MessageType.Error);
            Assert.Equal(1, (int)MessageType.Warning);
            Assert.Equal(2, (int)MessageType.Success);
            Assert.Equal(3, (int)MessageType.Informational);
            Assert.Equal(4, (int)MessageType.Message);
            Assert.Equal(5, (int)MessageType.Critical);
            Assert.Equal(6, (int)MessageType.Celebrate);
        }
    }
}
