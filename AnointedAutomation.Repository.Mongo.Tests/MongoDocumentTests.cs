// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
using System;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests for MongoDocument and the timestamp document hierarchy.
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

        private class TestCreatedDocument : CreatedMongoDocument
        {
        }

        private class TestTimestampedDocument : TimestampedMongoDocument
        {
            public string Description { get; set; }
        }

        private class TestHistoriedDocument : HistoriedMongoDocument
        {
        }

        #region MongoDocument Tests

        [Fact]
        public void MongoDocument_EmptyId_IsLeftOutOfTheBsonDocument()
        {
            TestMongoDocument doc = new TestMongoDocument { Name = "n" };

            MongoDB.Bson.BsonDocument bson = MongoDB.Bson.BsonExtensionMethods.ToBsonDocument(doc);

            Assert.False(bson.Contains("_id"));
        }

        [Fact]
        public void MongoDocument_SetId_IsWrittenAsObjectId()
        {
            TestMongoDocument doc = new TestMongoDocument { Id = "507f1f77bcf86cd799439011" };

            MongoDB.Bson.BsonDocument bson = MongoDB.Bson.BsonExtensionMethods.ToBsonDocument(doc);

            Assert.Equal(MongoDB.Bson.BsonType.ObjectId, bson["_id"].BsonType);
        }

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

        #region Timestamp hierarchy Tests

        [Fact]
        public void Hierarchy_EachLevelExtendsThePreviousOne()
        {
            Assert.IsAssignableFrom<MongoDocument>(new TestCreatedDocument());
            Assert.IsAssignableFrom<CreatedMongoDocument>(new TestTimestampedDocument());
            Assert.IsAssignableFrom<TimestampedMongoDocument>(new TestHistoriedDocument());
        }

        [Fact]
        public void TimestampedDocument_Timestamps_DefaultToDefaultDateTime()
        {
            TestTimestampedDocument doc = new TestTimestampedDocument();

            Assert.Equal(default(DateTime), doc.CreatedAt);
            Assert.Equal(default(DateTime), doc.UpdatedAt);
        }

        [Fact]
        public void TimestampedDocument_Timestamps_CanBeSet()
        {
            DateTime created = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime updated = new DateTime(2026, 3, 28, 0, 0, 0, DateTimeKind.Utc);
            TestTimestampedDocument doc = new TestTimestampedDocument
            {
                Id = "audit-doc-id",
                Description = "Test Description",
                CreatedAt = created,
                UpdatedAt = updated,
            };

            Assert.Equal("audit-doc-id", doc.Id);
            Assert.Equal(created, doc.CreatedAt);
            Assert.Equal(updated, doc.UpdatedAt);
        }

        [Fact]
        public void HistoriedDocument_UpdateHistory_StartsEmptyNotNull()
        {
            TestHistoriedDocument doc = new TestHistoriedDocument();

            Assert.NotNull(doc.UpdateHistory);
            Assert.Empty(doc.UpdateHistory);
        }

        [Fact]
        public void UpdateStamp_Defaults_AreEmptyNotNull()
        {
            UpdateStamp stamp = new UpdateStamp();

            Assert.Equal(string.Empty, stamp.By);
            Assert.NotNull(stamp.Fields);
            Assert.Empty(stamp.Fields);
        }

        #endregion
    }
}
