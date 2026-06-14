// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using Xunit;
using AnointedAutomation.APIMiddleware;
using System.Collections.Generic;

namespace AnointedAutomation.APIMiddlewares.Tests
{
    public class AttemptInfoTests
    {
        [Fact]
        public void Constructor_InitializedWithZeroCount()
        {
            // Act
            var attemptInfo = new AttemptInfo();
            
            // Assert
            Assert.Equal(0, attemptInfo.Count);
            Assert.NotNull(attemptInfo.Paths);
            Assert.Empty(attemptInfo.Paths);
        }

        [Fact]
        public void Paths_AddingItems_UpdatesCollection()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            string path1 = "/path1";
            string path2 = "/path2";
            
            // Act
            attemptInfo.Paths.Add(path1);
            attemptInfo.Paths.Add(path2);
            
            // Assert
            Assert.Equal(2, attemptInfo.Paths.Count);
            Assert.Contains(path1, attemptInfo.Paths);
            Assert.Contains(path2, attemptInfo.Paths);
        }

        [Fact]
        public void Paths_AddingDuplicateItem_DoesNotIncreaseCount()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            string path = "/path";
            
            // Act
            attemptInfo.Paths.Add(path);
            attemptInfo.Paths.Add(path); // Add the same path again
            
            // Assert
            Assert.Single(attemptInfo.Paths);
            Assert.Contains(path, attemptInfo.Paths);
        }

        [Fact]
        public void Count_CanBeIncremented()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            
            // Act
            attemptInfo.Count++;
            attemptInfo.Count++;
            
            // Assert
            Assert.Equal(2, attemptInfo.Count);
        }

        [Fact]
        public void Count_CanBeSet()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();

            // Act
            attemptInfo.Count = 5;

            // Assert
            Assert.Equal(5, attemptInfo.Count);
        }

        // EDGE CASE TESTS - Added per CLAUDE_TESTING.md requirements

        [Fact]
        public void Count_CanBeSetToZero()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            attemptInfo.Count = 10;

            // Act
            attemptInfo.Count = 0;

            // Assert
            Assert.Equal(0, attemptInfo.Count);
        }

        [Fact]
        public void Count_CanBeSetToNegative()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();

            // Act
            attemptInfo.Count = -1;

            // Assert
            Assert.Equal(-1, attemptInfo.Count);
        }

        [Fact]
        public void Count_CanBeSetToMaxInt()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();

            // Act
            attemptInfo.Count = int.MaxValue;

            // Assert
            Assert.Equal(int.MaxValue, attemptInfo.Count);
        }

        [Fact]
        public void Paths_AddingNullPath_AddsNull()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();

            // Act
            attemptInfo.Paths.Add(null);

            // Assert
            Assert.Single(attemptInfo.Paths);
            Assert.Contains(null, attemptInfo.Paths);
        }

        [Fact]
        public void Paths_AddingEmptyStringPath_AddsEmptyString()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();

            // Act
            attemptInfo.Paths.Add("");

            // Assert
            Assert.Single(attemptInfo.Paths);
            Assert.Contains("", attemptInfo.Paths);
        }

        [Fact]
        public void Paths_AddingWhitespacePath_AddsWhitespace()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            string path = "   ";

            // Act
            attemptInfo.Paths.Add(path);

            // Assert
            Assert.Single(attemptInfo.Paths);
            Assert.Contains(path, attemptInfo.Paths);
        }

        [Fact]
        public void Paths_AddingVeryLongPath_AddsSuccessfully()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            string longPath = "/" + new string('a', 10000);

            // Act
            attemptInfo.Paths.Add(longPath);

            // Assert
            Assert.Single(attemptInfo.Paths);
            Assert.Contains(longPath, attemptInfo.Paths);
        }

        [Fact]
        public void Paths_ClearingPaths_EmptiesCollection()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            attemptInfo.Paths.Add("/path1");
            attemptInfo.Paths.Add("/path2");

            // Act
            attemptInfo.Paths.Clear();

            // Assert
            Assert.Empty(attemptInfo.Paths);
        }

        [Fact]
        public void Paths_RemovingPath_RemovesFromCollection()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            string path = "/path";
            attemptInfo.Paths.Add(path);

            // Act
            bool removed = attemptInfo.Paths.Remove(path);

            // Assert
            Assert.True(removed);
            Assert.Empty(attemptInfo.Paths);
        }

        [Fact]
        public void Paths_CanBeReassigned()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();
            var newPaths = new HashSet<string> { "/new1", "/new2" };

            // Act
            attemptInfo.Paths = newPaths;

            // Assert
            Assert.Equal(2, attemptInfo.Paths.Count);
            Assert.Contains("/new1", attemptInfo.Paths);
            Assert.Contains("/new2", attemptInfo.Paths);
        }

        [Fact]
        public void Paths_CanBeSetToNull()
        {
            // Arrange
            var attemptInfo = new AttemptInfo();

            // Act
            attemptInfo.Paths = null;

            // Assert
            Assert.Null(attemptInfo.Paths);
        }
    }
}
