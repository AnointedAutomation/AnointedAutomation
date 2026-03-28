// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me
using System;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests for the MongoDocument and AuditableMongoDocument classes.
    /// </summary>
    public class MongoDocumentTests
    {
        /// <summary>
        /// Concrete implementation of MongoDocument for testing.
        /// </summary>
        private class TestMongoDocument : MongoDocument
        {
            public string Name { get; set; }
        }

        /// <summary>
        /// Concrete implementation of AuditableMongoDocument for testing.
        /// </summary>
        private class TestAuditableDocument : AuditableMongoDocument
        {
            public string Description { get; set; }
        }

        #region MongoDocument Tests

        [Fact]
        public void MongoDocument_Id_DefaultsToEmptyString()
        {
            // Arrange & Act
            TestMongoDocument doc = new TestMongoDocument();

            // Assert
            Assert.Equal(string.Empty, doc.Id);
        }

        [Fact]
        public void MongoDocument_Id_CanBeSet()
        {
            // Arrange
            TestMongoDocument doc = new TestMongoDocument();
            string expectedId = "507f1f77bcf86cd799439011";

            // Act
            doc.Id = expectedId;

            // Assert
            Assert.Equal(expectedId, doc.Id);
        }

        [Fact]
        public void MongoDocument_Id_CanBeSetToNull()
        {
            // Arrange
            TestMongoDocument doc = new TestMongoDocument();

            // Act
            doc.Id = null;

            // Assert
            Assert.Null(doc.Id);
        }

        [Fact]
        public void MongoDocument_AdditionalProperties_Work()
        {
            // Arrange
            TestMongoDocument doc = new TestMongoDocument();

            // Act
            doc.Id = "test-id";
            doc.Name = "Test Name";

            // Assert
            Assert.Equal("test-id", doc.Id);
            Assert.Equal("Test Name", doc.Name);
        }

        #endregion

        #region AuditableMongoDocument Tests

        [Fact]
        public void AuditableMongoDocument_InheritsFromMongoDocument()
        {
            // Arrange & Act
            TestAuditableDocument doc = new TestAuditableDocument();

            // Assert
            Assert.IsAssignableFrom<MongoDocument>(doc);
        }

        [Fact]
        public void AuditableMongoDocument_CreatedAt_DefaultsToDefaultDateTime()
        {
            // Arrange & Act
            TestAuditableDocument doc = new TestAuditableDocument();

            // Assert
            Assert.Equal(default(DateTime), doc.createdAt);
        }

        [Fact]
        public void AuditableMongoDocument_CreatedAt_CanBeSet()
        {
            // Arrange
            TestAuditableDocument doc = new TestAuditableDocument();
            DateTime expectedDate = new DateTime(2026, 3, 28, 12, 0, 0, DateTimeKind.Utc);

            // Act
            doc.createdAt = expectedDate;

            // Assert
            Assert.Equal(expectedDate, doc.createdAt);
        }

        [Fact]
        public void AuditableMongoDocument_UpdatedAt_DefaultsToDefaultDateTime()
        {
            // Arrange & Act
            TestAuditableDocument doc = new TestAuditableDocument();

            // Assert
            Assert.Equal(default(DateTime), doc.updatedAt);
        }

        [Fact]
        public void AuditableMongoDocument_UpdatedAt_CanBeSet()
        {
            // Arrange
            TestAuditableDocument doc = new TestAuditableDocument();
            DateTime expectedDate = new DateTime(2026, 3, 28, 14, 30, 0, DateTimeKind.Utc);

            // Act
            doc.updatedAt = expectedDate;

            // Assert
            Assert.Equal(expectedDate, doc.updatedAt);
        }

        [Fact]
        public void AuditableMongoDocument_Id_CanBeSetIndependentOfTimestamps()
        {
            // Arrange
            TestAuditableDocument doc = new TestAuditableDocument();
            string expectedId = "507f1f77bcf86cd799439012";
            DateTime createdDate = DateTime.UtcNow.AddDays(-1);
            DateTime updatedDate = DateTime.UtcNow;

            // Act
            doc.Id = expectedId;
            doc.createdAt = createdDate;
            doc.updatedAt = updatedDate;

            // Assert
            Assert.Equal(expectedId, doc.Id);
            Assert.Equal(createdDate, doc.createdAt);
            Assert.Equal(updatedDate, doc.updatedAt);
        }

        [Fact]
        public void AuditableMongoDocument_AdditionalProperties_Work()
        {
            // Arrange
            TestAuditableDocument doc = new TestAuditableDocument();

            // Act
            doc.Id = "audit-doc-id";
            doc.Description = "Test Description";
            doc.createdAt = DateTime.UtcNow;
            doc.updatedAt = DateTime.UtcNow;

            // Assert
            Assert.Equal("audit-doc-id", doc.Id);
            Assert.Equal("Test Description", doc.Description);
        }

        [Fact]
        public void AuditableMongoDocument_UpdatedAt_CanBeNewerThanCreatedAt()
        {
            // Arrange
            TestAuditableDocument doc = new TestAuditableDocument();
            DateTime createdDate = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime updatedDate = new DateTime(2026, 3, 28, 0, 0, 0, DateTimeKind.Utc);

            // Act
            doc.createdAt = createdDate;
            doc.updatedAt = updatedDate;

            // Assert
            Assert.True(doc.updatedAt > doc.createdAt);
        }

        #endregion
    }
}
