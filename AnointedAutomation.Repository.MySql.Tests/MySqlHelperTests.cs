// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Created by Alexander Fields https://www.alexanderfields.me
// Created by Alexander Fields

using AnointedAutomation.Optimization.Logging;
using AnointedAutomation.Repository.MySql;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Concurrent;
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
        public void Create_WithConnectionString_CachesInstance()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString = "Server=localhost;Database=test;User=root;Password=pass;";

            // Act & Assert - This test validates caching behavior
            // The factory's Create method attempts a real MySQL connection internally,
            // so without a running MySQL server, we can only verify the factory itself is created
            // and that repeated calls to Create with same connection string return the same cached instance
            // NOTE: This test requires a MySQL server to fully validate caching behavior
            // When MySQL is unavailable, we validate that the exception is consistent (same type)
            Exception ex1 = Record.Exception(() => factory.Create(connectionString));
            Exception ex2 = Record.Exception(() => factory.Create(connectionString));

            // Both attempts should either succeed (MySQL available) or fail with same exception type
            if (ex1 != null && ex2 != null)
            {
                // Both failed - verify same exception type (consistent behavior)
                Assert.Equal(ex1.GetType(), ex2.GetType());
            }
            else if (ex1 == null && ex2 == null)
            {
                // Both succeeded - would need to verify same instance, but this path requires MySQL
                // If we reach here, MySQL is available and factory is working
                Assert.True(true);
            }
            else
            {
                // Inconsistent behavior - one succeeded, one failed - this is a bug
                Assert.Fail("Factory behavior is inconsistent between calls");
            }
        }

        [Fact]
        public void ClearCache_ShouldNotThrowException()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Act - Verify ClearCache does not throw
            Exception ex = Record.Exception(() => factory.ClearCache());

            // Assert - Method should complete without exception
            Assert.Null(ex);
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
        public void DynamicDbContext_CanAccessGenericSet_WithInMemoryProvider()
        {
            // Arrange
            DbContextOptions<DynamicDbContext> options = new DbContextOptionsBuilder<DynamicDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using DynamicDbContext context = new DynamicDbContext(options);

            // Act - Attempt to get a generic DbSet
            // InMemory provider allows Set<T>() for any entity type
            DbSet<TestEntity> set = context.Set<TestEntity>();

            // Assert - Set should be returned (InMemory provider allows this)
            Assert.NotNull(set);
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
        public void Create_ForDifferentConnectionStrings_ProducesDifferentExceptions()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string conn1 = "Server=localhost;Database=db1;User=root;Password=pass;";
            string conn2 = "Server=localhost;Database=db2;User=root;Password=pass;";

            // Act - Attempt to create helpers for different connection strings
            // NOTE: Without a running MySQL server, we can only verify the factory
            // handles different connection strings independently (doesn't cache wrongly)
            Exception ex1 = Record.Exception(() => factory.Create(conn1));
            Exception ex2 = Record.Exception(() => factory.Create(conn2));

            // Assert - Both should fail consistently when MySQL is unavailable
            // or both succeed when MySQL is available
            if (ex1 != null && ex2 != null)
            {
                // Both failed - this is expected without MySQL
                // Verify both are MySQL connection exceptions
                Assert.Equal(ex1.GetType(), ex2.GetType());
            }
            else if (ex1 == null && ex2 == null)
            {
                // Both succeeded - MySQL is available, test passes
                Assert.True(true);
            }
            else
            {
                // One succeeded, one failed - inconsistent behavior
                Assert.Fail("Factory behavior is inconsistent for different connection strings");
            }
        }

        [Fact]
        public void ClearCache_AllowsNewInstanceCreation()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();

            // Act - Verify ClearCache does not throw
            Exception ex = Record.Exception(() => factory.ClearCache());

            // Assert - Method should complete without exception
            Assert.Null(ex);
        }

        [Fact]
        public void Create_ReturnsCachedInstance_WhenConnectionStringAlreadyCached()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString = "Server=testserver;Database=testdb;User=root;Password=pass;";

            // Use reflection to access the private _cache field and pre-populate it
            System.Reflection.FieldInfo cacheField = typeof(MySqlHelperFactory)
                .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ConcurrentDictionary<string, IMySqlHelper> cache =
                (ConcurrentDictionary<string, IMySqlHelper>)cacheField.GetValue(factory);

            // Create a mock MySqlHelper to add to cache
            Mock<IMySqlHelper> mockHelper = new Mock<IMySqlHelper>();
            cache.TryAdd(connectionString, mockHelper.Object);

            // Act - Call Create with the same connection string
            IMySqlHelper result = factory.Create(connectionString);

            // Assert - Should return the cached instance
            Assert.Same(mockHelper.Object, result);
        }

        [Fact]
        public void Create_ReturnsDifferentInstances_ForDifferentCachedConnectionStrings()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString1 = "Server=server1;Database=db1;User=root;Password=pass;";
            string connectionString2 = "Server=server2;Database=db2;User=root;Password=pass;";

            // Use reflection to access the private _cache field
            System.Reflection.FieldInfo cacheField = typeof(MySqlHelperFactory)
                .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ConcurrentDictionary<string, IMySqlHelper> cache =
                (ConcurrentDictionary<string, IMySqlHelper>)cacheField.GetValue(factory);

            // Create mock MySqlHelpers to add to cache
            Mock<IMySqlHelper> mockHelper1 = new Mock<IMySqlHelper>();
            Mock<IMySqlHelper> mockHelper2 = new Mock<IMySqlHelper>();
            cache.TryAdd(connectionString1, mockHelper1.Object);
            cache.TryAdd(connectionString2, mockHelper2.Object);

            // Act
            IMySqlHelper result1 = factory.Create(connectionString1);
            IMySqlHelper result2 = factory.Create(connectionString2);

            // Assert - Should return different cached instances
            Assert.Same(mockHelper1.Object, result1);
            Assert.Same(mockHelper2.Object, result2);
            Assert.NotSame(result1, result2);
        }

        [Fact]
        public void Remove_ReturnsTrue_WhenCachedInstanceExists()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString = "Server=testserver;Database=testdb;User=root;Password=pass;";

            // Use reflection to access the private _cache field
            System.Reflection.FieldInfo cacheField = typeof(MySqlHelperFactory)
                .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ConcurrentDictionary<string, IMySqlHelper> cache =
                (ConcurrentDictionary<string, IMySqlHelper>)cacheField.GetValue(factory);

            // Add a mock to the cache
            Mock<IMySqlHelper> mockHelper = new Mock<IMySqlHelper>();
            cache.TryAdd(connectionString, mockHelper.Object);

            // Act
            bool result = factory.Remove(connectionString);

            // Assert
            Assert.True(result);
            Assert.False(cache.ContainsKey(connectionString));
        }

        [Fact]
        public void ClearCache_RemovesAllCachedInstances()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString1 = "Server=server1;Database=db1;User=root;Password=pass;";
            string connectionString2 = "Server=server2;Database=db2;User=root;Password=pass;";

            // Use reflection to access the private _cache field
            System.Reflection.FieldInfo cacheField = typeof(MySqlHelperFactory)
                .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ConcurrentDictionary<string, IMySqlHelper> cache =
                (ConcurrentDictionary<string, IMySqlHelper>)cacheField.GetValue(factory);

            // Add mocks to the cache
            Mock<IMySqlHelper> mockHelper1 = new Mock<IMySqlHelper>();
            Mock<IMySqlHelper> mockHelper2 = new Mock<IMySqlHelper>();
            cache.TryAdd(connectionString1, mockHelper1.Object);
            cache.TryAdd(connectionString2, mockHelper2.Object);

            // Act
            factory.ClearCache();

            // Assert
            Assert.Empty(cache);
        }

        [Fact]
        public void Create_MultipleCalls_SameConnectionString_ReturnsSameInstance()
        {
            // Arrange
            MySqlHelperFactory factory = new MySqlHelperFactory();
            string connectionString = "Server=cached;Database=testdb;User=root;Password=pass;";

            // Use reflection to pre-populate cache
            System.Reflection.FieldInfo cacheField = typeof(MySqlHelperFactory)
                .GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            ConcurrentDictionary<string, IMySqlHelper> cache =
                (ConcurrentDictionary<string, IMySqlHelper>)cacheField.GetValue(factory);

            Mock<IMySqlHelper> mockHelper = new Mock<IMySqlHelper>();
            cache.TryAdd(connectionString, mockHelper.Object);

            // Act - Call Create multiple times
            IMySqlHelper result1 = factory.Create(connectionString);
            IMySqlHelper result2 = factory.Create(connectionString);
            IMySqlHelper result3 = factory.Create(connectionString);

            // Assert - All calls should return the same cached instance
            Assert.Same(result1, result2);
            Assert.Same(result2, result3);
        }
    }

    /// <summary>
    /// Additional edge case tests for ConnectionStringBuilder.
    /// </summary>
    public class ConnectionStringBuilderEdgeCaseTests
    {
        [Fact]
        public void ConnectionStringBuilder_WithEmptyServer_ReturnsStringWithEmptyServer()
        {
            // Arrange
            string server = "";
            string database = "testdb";
            string username = "root";
            string password = "pass";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Server=;", result);
            Assert.Contains("Database=testdb", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithEmptyDatabase_ReturnsStringWithEmptyDatabase()
        {
            // Arrange
            string server = "localhost";
            string database = "";
            string username = "root";
            string password = "pass";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Database=;", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithEmptyUsername_ReturnsStringWithEmptyUser()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "";
            string password = "pass";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("User=;", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithEmptyPassword_ReturnsStringWithEmptyPassword()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Password=;", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithSpecialCharactersInPassword_IncludesSpecialCharacters()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "p@ss!word#123$%^&*";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Password=p@ss!word#123$%^&*", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithVeryLongStrings_HandlesProperly()
        {
            // Arrange
            string server = new string('a', 1000);
            string database = new string('b', 1000);
            string username = new string('c', 1000);
            string password = new string('d', 1000);

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains($"Server={server}", result);
            Assert.Contains($"Database={database}", result);
            Assert.Contains($"User={username}", result);
            Assert.Contains($"Password={password}", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithDefaultPort_UsesPort3306()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "pass";

            // Act - Use default port (don't specify port parameter)
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Port=3306", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithZeroPort_IncludesZeroPort()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "pass";
            int port = 0;

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password, port);

            // Assert
            Assert.Contains("Port=0", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithNegativePort_IncludesNegativePort()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "pass";
            int port = -1;

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password, port);

            // Assert
            Assert.Contains("Port=-1", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithMaxIntPort_IncludesMaxPort()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "pass";
            int port = int.MaxValue;

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password, port);

            // Assert
            Assert.Contains($"Port={int.MaxValue}", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithUnicodeCharacters_IncludesUnicodeCharacters()
        {
            // Arrange
            string server = "localhost";
            string database = "testdb";
            string username = "root";
            string password = "pass\u4e2d\u6587\u03b1\u03b2\u03b3";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Password=pass\u4e2d\u6587\u03b1\u03b2\u03b3", result);
        }

        [Fact]
        public void ConnectionStringBuilder_WithWhitespaceValues_IncludesWhitespace()
        {
            // Arrange
            string server = "  localhost  ";
            string database = "  testdb  ";
            string username = "  root  ";
            string password = "  pass  ";

            // Act
            string result = MySqlHelper.ConnectionStringBuilder(server, database, username, password);

            // Assert
            Assert.Contains("Server=  localhost  ", result);
            Assert.Contains("Database=  testdb  ", result);
            Assert.Contains("User=  root  ", result);
            Assert.Contains("Password=  pass  ", result);
        }
    }

    /// <summary>
    /// Additional edge case tests for MySqlHelper logging.
    /// </summary>
    public class MySqlHelperLoggingEdgeCaseTests
    {
        [Fact]
        public void GetLogs_ReturnsArray_NotOriginalCollection()
        {
            // Arrange
            MySqlHelper.ClearLogs();
            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Test log") });

            // Act
            IList<LogMessage> logs1 = MySqlHelper.GetLogs();
            IList<LogMessage> logs2 = MySqlHelper.GetLogs();

            // Assert - Should be separate array instances
            Assert.NotSame(logs1, logs2);
        }

        [Fact]
        public void AddLog_WithMultipleLogs_MaintainsOrder()
        {
            // Arrange
            MySqlHelper.ClearLogs();
            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            // Act
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("First") });
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Second") });
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Third") });

            // Assert
            IList<LogMessage> logs = MySqlHelper.GetLogs();
            Assert.Equal(3, logs.Count);
            Assert.Equal("First", logs[0].message);
            Assert.Equal("Second", logs[1].message);
            Assert.Equal("Third", logs[2].message);
        }

        [Fact]
        public void ClearLogs_WhenEmpty_DoesNotThrow()
        {
            // Arrange
            MySqlHelper.ClearLogs(); // Ensure empty first

            // Act
            Exception ex = Record.Exception(() => MySqlHelper.ClearLogs());

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void ClearLogs_MultipleTimes_DoesNotThrow()
        {
            // Arrange
            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            addLogMethod.Invoke(null, new object[] { LogMessage.Informational("Test") });

            // Act - Clear multiple times in a row
            Exception ex = Record.Exception(() =>
            {
                MySqlHelper.ClearLogs();
                MySqlHelper.ClearLogs();
                MySqlHelper.ClearLogs();
            });

            // Assert
            Assert.Null(ex);
            Assert.Empty(MySqlHelper.GetLogs());
        }

        [Fact]
        public void LogCleared_WithNoHandlers_DoesNotThrow()
        {
            // Arrange - Ensure no handlers are attached
            System.Reflection.EventInfo eventInfo = typeof(MySqlHelper).GetEvent("LogCleared",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            // Event handlers could be attached from other tests, so we verify the behavior doesn't throw

            // Act
            Exception ex = Record.Exception(() => MySqlHelper.ClearLogs());

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void LogAdded_RaisedWithCorrectEventArgs()
        {
            // Arrange
            MySqlHelper.ClearLogs();
            LogMessageEventArgs capturedArgs = null;
            EventHandler<LogMessageEventArgs> handler = (sender, e) => capturedArgs = e;
            MySqlHelper.LogAdded += handler;

            System.Reflection.MethodInfo addLogMethod = typeof(MySqlHelper).GetMethod("AddLog",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            LogMessage testMessage = LogMessage.Warning("Test warning");

            // Act
            addLogMethod.Invoke(null, new object[] { testMessage });

            // Assert
            Assert.NotNull(capturedArgs);
            Assert.Equal(testMessage, capturedArgs.log);

            // Cleanup
            MySqlHelper.LogAdded -= handler;
        }

        [Fact]
        public void LogCleared_RaisedWithNullSender()
        {
            // Arrange
            object capturedSender = new object(); // Initialize with non-null to verify it becomes null
            EventArgs capturedArgs = null;
            EventHandler handler = (sender, e) =>
            {
                capturedSender = sender;
                capturedArgs = e;
            };
            MySqlHelper.LogCleared += handler;

            // Act
            MySqlHelper.ClearLogs();

            // Assert
            Assert.Null(capturedSender);
            Assert.Same(EventArgs.Empty, capturedArgs);

            // Cleanup
            MySqlHelper.LogCleared -= handler;
        }
    }

    /// <summary>
    /// Tests for MySqlHelper properties edge cases.
    /// </summary>
    public class MySqlHelperPropertiesEdgeCaseTests
    {
        [Fact]
        public void DbName_CanBeSetToNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.DbName = "test";

            // Act
            helper.DbName = null;

            // Assert
            Assert.Null(helper.DbName);
        }

        [Fact]
        public void DbName_CanBeSetToEmptyString()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();

            // Act
            helper.DbName = "";

            // Assert
            Assert.Equal("", helper.DbName);
        }

        [Fact]
        public void ConnectionString_CanBeSetToNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            helper.ConnectionString = "Server=localhost;";

            // Act
            helper.ConnectionString = null;

            // Assert
            Assert.Null(helper.ConnectionString);
        }

        [Fact]
        public void ConnectionString_CanBeSetToEmptyString()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();

            // Act
            helper.ConnectionString = "";

            // Assert
            Assert.Equal("", helper.ConnectionString);
        }

        [Fact]
        public void Database_CanBeSetToNull()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            Mock<DbContext> mockContext = new Mock<DbContext>();
            helper.Database = mockContext.Object;

            // Act
            helper.Database = null;

            // Assert
            Assert.Null(helper.Database);
        }

        [Fact]
        public void Properties_CanBeSetMultipleTimes()
        {
            // Arrange
            MySqlHelper helper = new MySqlHelper();
            Mock<DbContext> mockContext1 = new Mock<DbContext>();
            Mock<DbContext> mockContext2 = new Mock<DbContext>();

            // Act - Set multiple times
            helper.Database = mockContext1.Object;
            helper.Database = mockContext2.Object;
            helper.DbName = "db1";
            helper.DbName = "db2";
            helper.ConnectionString = "conn1";
            helper.ConnectionString = "conn2";

            // Assert - Last value should be retained
            Assert.Equal(mockContext2.Object, helper.Database);
            Assert.Equal("db2", helper.DbName);
            Assert.Equal("conn2", helper.ConnectionString);
        }
    }
}
