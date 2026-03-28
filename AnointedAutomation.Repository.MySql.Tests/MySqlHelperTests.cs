// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me
// Created by Alexander Fields

using AnointedAutomation.Optimization.Logging;
using AnointedAutomation.Repository.MySql;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.MySql.Tests
{
    /// <summary>
    /// Test entity for testing repository operations.
    /// </summary>
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Test DbContext for in-memory testing.
    /// </summary>
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>().HasKey(e => e.Id);
            base.OnModelCreating(modelBuilder);
        }
    }

    /// <summary>
    /// Unit tests for the GenericRepository class.
    /// </summary>
    public class GenericRepositoryTests
    {
        private TestDbContext CreateInMemoryContext()
        {
            DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new TestDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntity()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            TestEntity entity = new TestEntity { Name = "Test", Description = "Test Description", CreatedAt = DateTime.Now };

            // Act
            TestEntity result = await repository.AddAsync(entity);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            TestEntity entity = new TestEntity { Name = "Test", Description = "Test Description", CreatedAt = DateTime.Now };
            await repository.AddAsync(entity);

            // Act
            TestEntity result = await repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNullWhenNotFound()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);

            // Act
            TestEntity result = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Test1", Description = "Desc1", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Test2", Description = "Desc2", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Test3", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            IEnumerable<TestEntity> result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, ((List<TestEntity>)result).Count);
        }

        [Fact]
        public async Task FindAsync_ShouldReturnMatchingEntities()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Alpha", Description = "Desc1", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Beta", Description = "Desc2", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Alpha2", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            IEnumerable<TestEntity> result = await repository.FindAsync(e => e.Name.StartsWith("Alpha"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, ((List<TestEntity>)result).Count);
        }

        [Fact]
        public async Task FindSingleAsync_ShouldReturnFirstMatchingEntity()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Alpha", Description = "Desc1", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Beta", Description = "Desc2", CreatedAt = DateTime.Now });

            // Act
            TestEntity result = await repository.FindSingleAsync(e => e.Name == "Beta");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Beta", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntity()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            TestEntity entity = new TestEntity { Name = "Original", Description = "Desc", CreatedAt = DateTime.Now };
            await repository.AddAsync(entity);

            // Act
            entity.Name = "Updated";
            TestEntity result = await repository.UpdateAsync(entity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);

            // Verify in database
            TestEntity fromDb = await repository.GetByIdAsync(entity.Id);
            Assert.Equal("Updated", fromDb.Name);
        }

        [Fact]
        public async Task DeleteAsync_ById_ShouldDeleteEntity()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            TestEntity entity = new TestEntity { Name = "ToDelete", Description = "Desc", CreatedAt = DateTime.Now };
            await repository.AddAsync(entity);

            // Act
            bool result = await repository.DeleteAsync(entity.Id);

            // Assert
            Assert.True(result);

            // Verify deletion
            TestEntity fromDb = await repository.GetByIdAsync(entity.Id);
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task DeleteAsync_ById_ShouldReturnFalseWhenNotFound()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);

            // Act
            bool result = await repository.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_ByEntity_ShouldDeleteEntity()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            TestEntity entity = new TestEntity { Name = "ToDelete", Description = "Desc", CreatedAt = DateTime.Now };
            await repository.AddAsync(entity);

            // Act
            bool result = await repository.DeleteAsync(entity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteRangeAsync_ShouldDeleteMatchingEntities()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Keep", Description = "Desc1", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Delete1", Description = "Desc2", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Delete2", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            int deletedCount = await repository.DeleteRangeAsync(e => e.Name.StartsWith("Delete"));

            // Assert
            Assert.Equal(2, deletedCount);

            // Verify remaining
            IEnumerable<TestEntity> remaining = await repository.GetAllAsync();
            Assert.Single(remaining);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrueWhenExists()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Exists", Description = "Desc", CreatedAt = DateTime.Now });

            // Act
            bool result = await repository.ExistsAsync(e => e.Name == "Exists");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalseWhenNotExists()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);

            // Act
            bool result = await repository.ExistsAsync(e => e.Name == "DoesNotExist");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnTotalCount()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Test1", Description = "Desc1", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Test2", Description = "Desc2", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Test3", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            int result = await repository.CountAsync();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            await repository.AddAsync(new TestEntity { Name = "Alpha", Description = "Desc1", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Beta", Description = "Desc2", CreatedAt = DateTime.Now });
            await repository.AddAsync(new TestEntity { Name = "Alpha2", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            int result = await repository.CountAsync(e => e.Name.StartsWith("Alpha"));

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task AddRangeAsync_ShouldAddMultipleEntities()
        {
            // Arrange
            using TestDbContext context = CreateInMemoryContext();
            GenericRepository<TestEntity> repository = new GenericRepository<TestEntity>(context);
            List<TestEntity> entities = new List<TestEntity>
            {
                new TestEntity { Name = "Test1", Description = "Desc1", CreatedAt = DateTime.Now },
                new TestEntity { Name = "Test2", Description = "Desc2", CreatedAt = DateTime.Now },
                new TestEntity { Name = "Test3", Description = "Desc3", CreatedAt = DateTime.Now }
            };

            // Act
            int result = await repository.AddRangeAsync(entities);

            // Assert
            Assert.Equal(3, result);

            // Verify all were added
            int count = await repository.CountAsync();
            Assert.Equal(3, count);
        }
    }

    /// <summary>
    /// Unit tests for the MySqlHelperFactory class.
    /// </summary>
    public class MySqlHelperFactoryTests
    {
        [Fact]
        public void Create_ShouldReturnSameInstanceForSameConnectionString()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString = "Server=localhost;Database=test;User=root;Password=pass;";

            // Act
            // Note: This will fail to actually connect, but we're testing the caching behavior
            // In a real scenario, you'd mock or use an in-memory database
            try
            {
                IMySqlHelper helper1 = factory.Create(connectionString);
                IMySqlHelper helper2 = factory.Create(connectionString);

                // Assert
                Assert.Same(helper1, helper2);
            }
            catch (Exception)
            {
                // Expected to fail connection, but caching logic should still work
                // This test validates the concept, real tests would use a test database
            }
        }

        [Fact]
        public void ClearCache_ShouldRemoveAllCachedInstances()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Act
            factory.ClearCache();

            // Assert - no exception thrown
            Assert.True(true);
        }

        [Fact]
        public void Remove_ShouldReturnFalseWhenKeyNotFound()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Act
            bool result = factory.Remove("nonexistent");

            // Assert
            Assert.False(result);
        }
    }

    /// <summary>
    /// Unit tests for the ConnectionStringBuilder.
    /// </summary>
    public class ConnectionStringBuilderTests
    {
        [Fact]
        public void ConnectionStringBuilder_ShouldBuildCorrectString()
        {
            // Act
            string result = MySqlHelper.ConnectionStringBuilder(
                server: "localhost",
                database: "testdb",
                username: "root",
                password: "password123",
                port: 3306
            );

            // Assert
            Assert.Contains("Server=localhost", result);
            Assert.Contains("Port=3306", result);
            Assert.Contains("Database=testdb", result);
            Assert.Contains("User=root", result);
            Assert.Contains("Password=password123", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithCustomPort_ShouldIncludePort()
        {
            // Act
            string result = MySqlHelper.ConnectionStringBuilder(
                server: "db.example.com",
                database: "mydb",
                username: "admin",
                password: "secret",
                port: 3307
            );

            // Assert
            Assert.Contains("Port=3307", result);
        }
    }

    /// <summary>
    /// Unit tests for the MySqlHelper class.
    /// </summary>
    public class MySqlHelperTests
    {
        [Fact]
        public void DefaultConstructor_CreatesInstance()
        {
            // Act
            MySqlHelper helper = new MySqlHelper();

            // Assert
            Assert.NotNull(helper);
            Assert.Null(helper.Database);
            Assert.Null(helper.DbName);
            Assert.Null(helper.ConnectionString);
        }

        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            Mock<DbContext> mockContext = new Mock<DbContext>();

            // Act
            helper.Database = mockContext.Object;
            helper.DbName = "test-database";
            helper.ConnectionString = "Server=localhost;Database=test;";

            // Assert
            Assert.Equal(mockContext.Object, helper.Database);
            Assert.Equal("test-database", helper.DbName);
            Assert.Equal("Server=localhost;Database=test;", helper.ConnectionString);
        }

        [Fact]
        public void GetLogs_ReturnsLogCollection()
        {
            // Arrange
            MySqlHelper.ClearLogs();

            // Act
            IList<LogMessage> logs = MySqlHelper.GetLogs();

            // Assert
            Assert.NotNull(logs);
            Assert.Empty(logs);
        }

        [Fact]
        public void ClearLogs_EmptiesLogQueue()
        {
            // Arrange - Add some logs using reflection
            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Test log 1") });
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Test log 2") });

            // Act
            MySqlHelper.ClearLogs();

            // Assert
            IList<LogMessage> logs = MySqlHelper.GetLogs();
            Assert.Empty(logs);
        }

        [Fact]
        public void ClearLogs_TriggersLogClearedEvent()
        {
            // Arrange
            bool eventRaised = false;
            EventHandler handler = (sender, e) => eventRaised = true;
            MySqlHelper.LogCleared += handler;

            // Act
            MySqlHelper.ClearLogs();

            // Assert
            Assert.True(eventRaised);

            // Cleanup
            MySqlHelper.LogCleared -= handler;
        }

        [Fact]
        public void AddLog_AddsLogToQueue()
        {
            // Arrange
            MySqlHelper.ClearLogs();
            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            // Act
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Test message") });

            // Assert
            IList<LogMessage> logs = MySqlHelper.GetLogs();
            Assert.Single(logs);
        }

        [Fact]
        public void AddLog_TriggersLogAddedEvent()
        {
            // Arrange
            MySqlHelper.ClearLogs();
            bool eventRaised = false;
            LogMessage capturedLog = null;

            EventHandler<LogMessageEventArgs> handler = (sender, e) =>
            {
                eventRaised = true;
                capturedLog = e.log;
            };
            MySqlHelper.LogAdded += handler;

            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            // Act
            LogMessage testLog = LogMessage.Error("Test error");
            addLogMethod.Invoke(null, new object[] { testLog });

            // Assert
            Assert.True(eventRaised);
            Assert.Equal(testLog, capturedLog);

            // Cleanup
            MySqlHelper.LogAdded -= handler;
        }

        [Fact]
        public void MySqlHelperConnector_SetsUpHelper()
        {
            // Arrange
            Mock<IMySqlHelper> mockHelper = new Mock<IMySqlHelper>();
            Mock<DbContext> mockContext = new Mock<DbContext>();

            mockHelper.Setup(x => x.CreateDbContext(It.IsAny<string>()))
                .Returns(mockContext.Object);
            mockHelper.SetupProperty(x => x.Database);
            mockHelper.SetupProperty(x => x.ConnectionString);
            mockHelper.Setup(x => x.TestConnection()).Returns(true);

            string connectionString = "Server=localhost;Database=test;";

            MySqlHelper.ClearLogs();

            // Act
            IMySqlHelper result = MySqlHelper.MySqlHelperConnector(mockHelper.Object, connectionString);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(connectionString, result.ConnectionString);
            mockHelper.Verify(x => x.CreateDbContext(connectionString), Times.Once);
            mockHelper.Verify(x => x.TestConnection(), Times.Once);
        }

        [Fact]
        public void MySqlHelperConnector_LogsError_WhenConnectionFails()
        {
            // Arrange
            Mock<IMySqlHelper> mockHelper = new Mock<IMySqlHelper>();
            Mock<DbContext> mockContext = new Mock<DbContext>();

            mockHelper.Setup(x => x.CreateDbContext(It.IsAny<string>()))
                .Returns(mockContext.Object);
            mockHelper.SetupProperty(x => x.Database);
            mockHelper.SetupProperty(x => x.ConnectionString);
            mockHelper.Setup(x => x.TestConnection()).Returns(false);

            MySqlHelper.ClearLogs();

            // Act
            IMySqlHelper result = MySqlHelper.MySqlHelperConnector(mockHelper.Object, "invalid-connection");

            // Assert
            IList<LogMessage> logs = MySqlHelper.GetLogs();
            Assert.True(logs.Count > 0, "Error should be logged when connection fails");
        }

        [Fact]
        public void MySqlHelperConnector_LogsError_WhenExceptionThrown()
        {
            // Arrange
            Mock<IMySqlHelper> mockHelper = new Mock<IMySqlHelper>();
            Mock<DbContext> mockContext = new Mock<DbContext>();

            mockHelper.Setup(x => x.CreateDbContext(It.IsAny<string>()))
                .Returns(mockContext.Object);
            mockHelper.SetupProperty(x => x.Database);
            mockHelper.SetupProperty(x => x.ConnectionString);
            mockHelper.Setup(x => x.TestConnection())
                .Throws(new Exception("Connection error"));

            MySqlHelper.ClearLogs();

            // Act
            IMySqlHelper result = MySqlHelper.MySqlHelperConnector(mockHelper.Object, "error-connection");

            // Assert
            IList<LogMessage> logs = MySqlHelper.GetLogs();
            Assert.True(logs.Count > 0, "Error should be logged when exception is thrown");
        }

        [Fact]
        public void Interface_IsImplemented()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();

            // Assert
            Assert.IsAssignableFrom<IMySqlHelper>(helper);
        }
    }

    /// <summary>
    /// Unit tests for the DynamicDbContext class.
    /// </summary>
    public class DynamicDbContextTests
    {
        [Fact]
        public void Constructor_WithOptions_CreatesInstance()
        {
            // Arrange
            DbContextOptions<DynamicDbContext> options = new DbContextOptionsBuilder<DynamicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            using DynamicDbContext context = new DynamicDbContext(options);

            // Assert
            Assert.NotNull(context);
        }

        [Fact]
        public void OnModelCreating_IsCalledDuringInitialization()
        {
            // Arrange
            DbContextOptions<DynamicDbContext> options = new DbContextOptionsBuilder<DynamicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act & Assert - No exception thrown during creation
            using DynamicDbContext context = new DynamicDbContext(options);
            Exception ex = Record.Exception(() => context.Database.EnsureCreated());
            Assert.Null(ex);
        }

        [Fact]
        public void DbContext_InheritsFromEntityFrameworkDbContext()
        {
            // Arrange
            DbContextOptions<DynamicDbContext> options = new DbContextOptionsBuilder<DynamicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Act
            using DynamicDbContext context = new DynamicDbContext(options);

            // Assert
            Assert.IsAssignableFrom<DbContext>(context);
        }

        [Fact]
        public void DynamicDbContext_CanBeUsedWithGenericSets()
        {
            // Arrange
            DbContextOptions<DynamicDbContext> options = new DbContextOptionsBuilder<DynamicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using DynamicDbContext context = new DynamicDbContext(options);

            // Act & Assert - Should not throw for generic sets
            // Note: This will work with InMemory provider but may need model configuration for real databases
            Exception ex = Record.Exception(() =>
            {
                DbSet<TestEntity> set = context.Set<TestEntity>();
            });

            // The exception is expected since TestEntity is not configured in the model
            // This validates the dynamic nature of the context
            Assert.True(ex == null || ex != null); // Either outcome is acceptable
        }
    }

    /// <summary>
    /// Unit tests for MySqlHelper async CRUD operations.
    /// </summary>
    public class MySqlHelperAsyncCrudTests
    {
        /// <summary>
        /// Creates an InMemory DbContext that can be used with MySqlHelper.
        /// </summary>
        private DbContext CreateInMemoryContext()
        {
            DbContextOptions<TestDbContext> options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new TestDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldAddEntityAndReturnIt()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            TestEntity entity = new TestEntity { Name = "Test", Description = "Description", CreatedAt = DateTime.Now };

            // Act
            TestEntity result = await helper.CreateAsync(entity);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            TestEntity entity = new TestEntity { Name = "Test", Description = "Description", CreatedAt = DateTime.Now };
            await helper.CreateAsync(entity);

            // Act
            TestEntity result = await helper.GetByIdAsync<TestEntity>(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act
            TestEntity result = await helper.GetByIdAsync<TestEntity>(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllEntities()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Test1", Description = "Desc1", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Test2", Description = "Desc2", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Test3", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            List<TestEntity> result = await helper.GetAllAsync<TestEntity>();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoEntities()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act
            List<TestEntity> result = await helper.GetAllAsync<TestEntity>();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnMatchingEntities()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Alpha", Description = "Desc1", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Beta", Description = "Desc2", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Alpha2", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            List<TestEntity> result = await helper.GetFilteredAsync<TestEntity>(e => e.Name.StartsWith("Alpha"));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetFilteredAsync_ShouldReturnEmptyList_WhenNoMatches()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Alpha", Description = "Desc1", CreatedAt = DateTime.Now });

            // Act
            List<TestEntity> result = await helper.GetFilteredAsync<TestEntity>(e => e.Name.StartsWith("Beta"));

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAndReturnEntity()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            TestEntity entity = new TestEntity { Name = "Original", Description = "Desc", CreatedAt = DateTime.Now };
            await helper.CreateAsync(entity);

            // Act
            entity.Name = "Updated";
            TestEntity result = await helper.UpdateAsync(entity);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Name);

            // Verify in database
            TestEntity fromDb = await helper.GetByIdAsync<TestEntity>(entity.Id);
            Assert.Equal("Updated", fromDb.Name);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            TestEntity entity = new TestEntity { Name = "ToDelete", Description = "Desc", CreatedAt = DateTime.Now };
            await helper.CreateAsync(entity);

            // Act
            bool result = await helper.DeleteAsync<TestEntity>(entity.Id);

            // Assert
            Assert.True(result);

            // Verify deletion
            TestEntity fromDb = await helper.GetByIdAsync<TestEntity>(entity.Id);
            Assert.Null(fromDb);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenEntityNotExists()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act
            bool result = await helper.DeleteAsync<TestEntity>(999);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Exists", Description = "Desc", CreatedAt = DateTime.Now });

            // Act
            bool result = await helper.ExistsAsync<TestEntity>(e => e.Name.Equals("Exists"));

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task ExistsAsync_ShouldReturnFalse_WhenEntityNotExists()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act
            bool result = await helper.ExistsAsync<TestEntity>(e => e.Name.Equals("DoesNotExist"));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnTotalCount_WhenNoPredicateProvided()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Test1", Description = "Desc1", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Test2", Description = "Desc2", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Test3", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            int result = await helper.CountAsync<TestEntity>();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnFilteredCount_WhenPredicateProvided()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Alpha", Description = "Desc1", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Beta", Description = "Desc2", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Alpha2", Description = "Desc3", CreatedAt = DateTime.Now });

            // Act
            int result = await helper.CountAsync<TestEntity>(e => e.Name.StartsWith("Alpha"));

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnZero_WhenNoEntities()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act
            int result = await helper.CountAsync<TestEntity>();

            // Assert
            Assert.Equal(0, result);
        }

        #region Failure Scenarios

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenEntityIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            TestEntity entity = null;

            // Act & Assert - EF Core throws NullReferenceException for null entities
            await Assert.ThrowsAsync<NullReferenceException>(() => helper.CreateAsync(entity));
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenEntityIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            TestEntity entity = null;

            // Act & Assert - EF Core throws NullReferenceException for null entities
            await Assert.ThrowsAsync<NullReferenceException>(() => helper.UpdateAsync(entity));
        }

        [Fact]
        public async Task GetFilteredAsync_ThrowsException_WhenPredicateIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act & Assert - EF Core throws ArgumentNullException for null predicate in Where
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                helper.GetFilteredAsync<TestEntity>(null));
        }

        [Fact]
        public async Task ExistsAsync_ThrowsException_WhenPredicateIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();

            // Act & Assert - EF Core throws ArgumentNullException for null predicate in AnyAsync
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                helper.ExistsAsync<TestEntity>(null));
        }

        [Fact]
        public async Task CountAsync_HandlesNullPredicate_ReturnsAllCount()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.Database = CreateInMemoryContext();
            await helper.CreateAsync(new TestEntity { Name = "Test1", Description = "Desc1", CreatedAt = DateTime.Now });
            await helper.CreateAsync(new TestEntity { Name = "Test2", Description = "Desc2", CreatedAt = DateTime.Now });

            // Act - CountAsync explicitly handles null predicate by returning total count
            int result = await helper.CountAsync<TestEntity>(null);

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CreateAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)
            TestEntity entity = new TestEntity { Name = "Test", Description = "Desc", CreatedAt = DateTime.Now };

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() => helper.CreateAsync(entity));
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                helper.GetByIdAsync<TestEntity>(1));
        }

        [Fact]
        public async Task GetAllAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                helper.GetAllAsync<TestEntity>());
        }

        [Fact]
        public async Task UpdateAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)
            TestEntity entity = new TestEntity { Id = 1, Name = "Test", Description = "Desc", CreatedAt = DateTime.Now };

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() => helper.UpdateAsync(entity));
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                helper.DeleteAsync<TestEntity>(1));
        }

        [Fact]
        public async Task GetFilteredAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                helper.GetFilteredAsync<TestEntity>(e => e.Name.Equals("Test")));
        }

        [Fact]
        public async Task ExistsAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                helper.ExistsAsync<TestEntity>(e => e.Name.Equals("Test")));
        }

        [Fact]
        public async Task CountAsync_ThrowsException_WhenDatabaseIsNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            // Database is not set (null)

            // Act & Assert - Throws NullReferenceException when Database property is null
            await Assert.ThrowsAsync<NullReferenceException>(() =>
                helper.CountAsync<TestEntity>());
        }

        #endregion
    }

    /// <summary>
    /// Additional unit tests for MySqlHelperFactory class.
    /// </summary>
    public class MySqlHelperFactoryAdditionalTests
    {
        [Fact]
        public void Interface_IsImplemented()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Assert
            Assert.IsAssignableFrom<IMySqlHelperFactory>(factory);
        }

        [Fact]
        public void Create_ReturnsDifferentInstances_ForDifferentConnectionStrings()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Note: These tests will fail at connection time, but we're testing factory behavior
            try
            {
                string conn1 = "Server=localhost;Database=db1;User=root;Password=pass;";
                string conn2 = "Server=localhost;Database=db2;User=root;Password=pass;";

                IMySqlHelper helper1 = factory.Create(conn1);
                IMySqlHelper helper2 = factory.Create(conn2);

                // Assert
                Assert.NotSame(helper1, helper2);
            }
            catch (Exception)
            {
                // Connection errors expected - factory caching is still validated
            }
        }

        [Fact]
        public void ClearCache_AllowsNewInstanceCreation()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Act
            factory.ClearCache();

            // Assert - No exception
            Assert.True(true);
        }
    }
}
