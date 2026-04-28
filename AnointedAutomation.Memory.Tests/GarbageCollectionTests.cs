// Copyright © Anointed Automation, LLC., 2025. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me on 2025-06-08 13:27:40
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
using System;
using Xunit;
using AnointedAutomation.Optimization.Memory;

namespace AnointedAutomation.Memory.Tests
{
    /// <summary>
    /// Tests for the GarbageCollection class.
    /// </summary>
    public class GarbageCollectionTests
    {
        [Fact]
        /// <summary>
        /// Verifies that the GarbageCollection constructor creates a valid instance.
        /// </summary>
        public void Constructor_CreatesInstance()
        {
            // Act
            var gc = new GarbageCollection();

            // Assert
            Assert.NotNull(gc);
        }

        [Fact]
        /// <summary>
        /// Verifies that PerformGarbageCollection executes without throwing exceptions.
        /// </summary>
        public void PerformGarbageCollection_DoesNotThrowException()
        {
            // Arrange
            var gc = new GarbageCollection();
            object state = null;

            // Act & Assert
            var exception = Record.Exception(() => gc.PerformGarbageCollection(state));
            Assert.Null(exception);
        }

        [Fact]
        /// <summary>
        /// Verifies that PerformGarbageCollection handles non-null state parameters correctly.
        /// </summary>
        public void PerformGarbageCollection_WithNonNullState_DoesNotThrowException()
        {
            // Arrange
            var gc = new GarbageCollection();
            object state = new object();

            // Act & Assert
            var exception = Record.Exception(() => gc.PerformGarbageCollection(state));
            Assert.Null(exception);
        }

        [Fact]
        /// <summary>
        /// Verifies that PerformGarbageCollection can be called multiple times without errors.
        /// </summary>
        public void PerformGarbageCollection_CanBeCalledMultipleTimes()
        {
            // Arrange
            var gc = new GarbageCollection();
            object state = null;

            // Act & Assert
            var exception1 = Record.Exception(() => gc.PerformGarbageCollection(state));
            var exception2 = Record.Exception(() => gc.PerformGarbageCollection(state));
            var exception3 = Record.Exception(() => gc.PerformGarbageCollection(state));

            Assert.Null(exception1);
            Assert.Null(exception2);
            Assert.Null(exception3);
        }

        [Fact]
        /// <summary>
        /// Tests that PerformGarbageCollection actually triggers garbage collection.
        /// Verifies GC ran by checking collection count increases.
        /// </summary>
        public void GarbageCollection_TriggersGCCollection()
        {
            // Arrange
            var gc = new GarbageCollection();
            object state = null;

            // Get initial GC collection counts for all generations
            int gen0Before = GC.CollectionCount(0);
            int gen1Before = GC.CollectionCount(1);
            int gen2Before = GC.CollectionCount(2);

            // Act
            gc.PerformGarbageCollection(state);

            // Get collection counts after
            int gen0After = GC.CollectionCount(0);
            int gen1After = GC.CollectionCount(1);
            int gen2After = GC.CollectionCount(2);

            // Assert - At least one generation should have been collected
            // GC.Collect() triggers a full collection (all generations)
            bool gcRan = (gen0After > gen0Before) ||
                         (gen1After > gen1Before) ||
                         (gen2After > gen2Before);
            Assert.True(gcRan, "GC.Collect() should trigger at least one garbage collection");
        }
    }
}
