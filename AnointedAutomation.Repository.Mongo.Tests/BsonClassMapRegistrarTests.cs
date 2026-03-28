// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me
using System;
using AnointedAutomation.Objects.Account;
using MongoDB.Bson.Serialization;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests for the BsonClassMapRegistrar class.
    /// Note: These tests work with static state that persists across test runs.
    /// The registrar is designed to be idempotent, so calling it multiple times is safe.
    /// </summary>
    public class BsonClassMapRegistrarTests
    {
        [Fact]
        public void RegisterClassMaps_CanBeCalledMultipleTimes_WithoutException()
        {
            // Arrange & Act - Call multiple times
            Exception exception = Record.Exception(() =>
            {
                BsonClassMapRegistrar.RegisterClassMaps();
                BsonClassMapRegistrar.RegisterClassMaps();
                BsonClassMapRegistrar.RegisterClassMaps();
            });

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void RegisterClassMaps_RegistersUserClassMap()
        {
            // Arrange & Act
            BsonClassMapRegistrar.RegisterClassMaps();

            // Assert
            Assert.True(BsonClassMap.IsClassMapRegistered(typeof(User)));
        }

        [Fact]
        public void RegisterClassMaps_IsIdempotent()
        {
            // Arrange
            BsonClassMapRegistrar.RegisterClassMaps();
            bool firstCallRegistered = BsonClassMap.IsClassMapRegistered(typeof(User));

            // Act - Call again
            BsonClassMapRegistrar.RegisterClassMaps();
            bool secondCallRegistered = BsonClassMap.IsClassMapRegistered(typeof(User));

            // Assert - Should still be registered
            Assert.True(firstCallRegistered);
            Assert.True(secondCallRegistered);
        }

        [Fact]
        public void RegisterDerivedUser_CanBeCalledWithDerivedType()
        {
            // Arrange - Ensure base registration is done first
            BsonClassMapRegistrar.RegisterClassMaps();

            // Act
            Exception exception = Record.Exception(() =>
            {
                BsonClassMapRegistrar.RegisterDerivedUser<TestDerivedUser>();
            });

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void RegisterDerivedUser_IsIdempotent()
        {
            // Arrange
            BsonClassMapRegistrar.RegisterClassMaps();

            // Act - Call multiple times
            Exception exception = Record.Exception(() =>
            {
                BsonClassMapRegistrar.RegisterDerivedUser<TestDerivedUser>();
                BsonClassMapRegistrar.RegisterDerivedUser<TestDerivedUser>();
            });

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public void RegisterDerivedUser_RegistersClassMap()
        {
            // Arrange
            BsonClassMapRegistrar.RegisterClassMaps();

            // Act
            BsonClassMapRegistrar.RegisterDerivedUser<AnotherDerivedUser>();

            // Assert
            Assert.True(BsonClassMap.IsClassMapRegistered(typeof(AnotherDerivedUser)));
        }

        [Fact]
        public void RegisterClassMaps_IsThreadSafe()
        {
            // Arrange
            System.Threading.Tasks.Task[] tasks = new System.Threading.Tasks.Task[10];

            // Act - Call from multiple threads simultaneously
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = System.Threading.Tasks.Task.Run(() =>
                {
                    BsonClassMapRegistrar.RegisterClassMaps();
                });
            }

            Exception exception = Record.Exception(() =>
            {
                System.Threading.Tasks.Task.WaitAll(tasks);
            });

            // Assert - No exceptions from race conditions
            Assert.Null(exception);
        }

        /// <summary>
        /// Test class derived from User for testing RegisterDerivedUser.
        /// </summary>
        private class TestDerivedUser : User
        {
            public string TestProperty { get; set; }
        }

        /// <summary>
        /// Another test class derived from User for testing RegisterDerivedUser.
        /// </summary>
        private class AnotherDerivedUser : User
        {
            public int AnotherProperty { get; set; }
        }
    }
}
