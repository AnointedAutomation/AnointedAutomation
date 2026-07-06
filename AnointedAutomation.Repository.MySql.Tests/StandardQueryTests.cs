// Copyright 2026 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Unit tests for the additive standard query methods on GenericRepository and MySqlHelper, exercised against
// the EF Core InMemory provider. Raw-SQL (ExecuteRawSqlAsync) and transactions (ExecuteInTransactionAsync)
// are intentionally NOT covered here: the InMemory provider supports neither, so they require a live MySQL
// (CI/integration) to test. Reuses TestEntity / TestDbContext from MySqlHelperTests.cs (same namespace).

using AnointedAutomation.Repository.MySql;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.MySql.Tests
{
    public class StandardQueryTests
    {
        private static TestDbContext NewContext() =>
            new TestDbContext(new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options);

        private static async Task<TestDbContext> SeededContextAsync()
        {
            TestDbContext context = NewContext();
            context.TestEntities.AddRange(
                new TestEntity { Id = 1, Name = "b", Description = "x" },
                new TestEntity { Id = 2, Name = "a", Description = "x" },
                new TestEntity { Id = 3, Name = "c", Description = "y" },
                new TestEntity { Id = 4, Name = "d", Description = "y" });
            await context.SaveChangesAsync();
            return context;
        }

        // ---- GenericRepository ----------------------------------------------

        [Fact]
        public async Task GenericRepository_GetPagedAsync_OrdersThenSkipsAndTakes()
        {
            using TestDbContext context = await SeededContextAsync();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);

            List<TestEntity> page = (await repo.GetPagedAsync(null, e => e.Id, false, 1, 2)).ToList();

            Assert.Equal(2, page.Count);
            Assert.Equal(2, page[0].Id);
            Assert.Equal(3, page[1].Id);
        }

        [Fact]
        public async Task GenericRepository_GetPagedAsync_WithPredicate_Filters()
        {
            using TestDbContext context = await SeededContextAsync();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);

            List<TestEntity> page = (await repo.GetPagedAsync(e => e.Description == "y", e => e.Id, false, 0, 10)).ToList();

            Assert.Equal(2, page.Count);
            Assert.All(page, e => Assert.Equal("y", e.Description));
        }

        [Fact]
        public async Task GenericRepository_GetOrderedAsync_Descending()
        {
            using TestDbContext context = await SeededContextAsync();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);

            List<TestEntity> ordered = (await repo.GetOrderedAsync(e => e.Name, true)).ToList();

            Assert.Equal(new[] { "d", "c", "b", "a" }, ordered.Select(e => e.Name).ToArray());
        }

        [Fact]
        public async Task GenericRepository_SelectAsync_ProjectsShape()
        {
            using TestDbContext context = await SeededContextAsync();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);

            List<string> names = (await repo.SelectAsync(e => e.Description == "x", e => e.Name)).ToList();

            Assert.Equal(2, names.Count);
            Assert.Contains("a", names);
            Assert.Contains("b", names);
        }

        [Fact]
        public async Task GenericRepository_AnyAsync_NoPredicate_AndPredicate()
        {
            using TestDbContext context = await SeededContextAsync();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);

            Assert.True(await repo.AnyAsync());
            Assert.True(await repo.AnyAsync(e => e.Name == "a"));
            Assert.False(await repo.AnyAsync(e => e.Name == "zzz"));
        }

        [Fact]
        public async Task GenericRepository_AnyAsync_EmptySet_IsFalse()
        {
            using TestDbContext context = NewContext();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);
            Assert.False(await repo.AnyAsync());
        }

        [Fact]
        public async Task GenericRepository_UpdateRangeAsync_UpdatesAll()
        {
            using TestDbContext context = await SeededContextAsync();
            GenericRepository<TestEntity> repo = new GenericRepository<TestEntity>(context);

            List<TestEntity> all = (await repo.GetAllAsync()).ToList();
            foreach (TestEntity e in all)
            {
                e.Description = "updated";
            }
            int written = await repo.UpdateRangeAsync(all);

            Assert.Equal(4, written);
            Assert.Equal(4, await repo.CountAsync(e => e.Description == "updated"));
        }

        // ---- MySqlHelper (Database set to an InMemory context) --------------

        private static async Task<MySqlHelper> SeededHelperAsync()
        {
            MySqlHelper helper = new MySqlHelper { Database = await SeededContextAsync() };
            return helper;
        }

        [Fact]
        public async Task MySqlHelper_FirstOrDefaultAsync_ReturnsMatch_OrNull()
        {
            MySqlHelper helper = await SeededHelperAsync();

            TestEntity match = await helper.FirstOrDefaultAsync<TestEntity>(e => e.Name == "a");
            TestEntity none = await helper.FirstOrDefaultAsync<TestEntity>(e => e.Name == "zzz");

            Assert.NotNull(match);
            Assert.Equal(2, match.Id);
            Assert.Null(none);
        }

        [Fact]
        public async Task MySqlHelper_GetPagedAsync_OrdersThenSkipsAndTakes()
        {
            MySqlHelper helper = await SeededHelperAsync();

            List<TestEntity> page = await helper.GetPagedAsync<TestEntity, int>(null, e => e.Id, false, 2, 2);

            Assert.Equal(2, page.Count);
            Assert.Equal(3, page[0].Id);
            Assert.Equal(4, page[1].Id);
        }

        [Fact]
        public async Task MySqlHelper_GetOrderedAsync_AscendingByName()
        {
            MySqlHelper helper = await SeededHelperAsync();

            List<TestEntity> ordered = await helper.GetOrderedAsync<TestEntity, string>(e => e.Name, false);

            Assert.Equal(new[] { "a", "b", "c", "d" }, ordered.Select(e => e.Name).ToArray());
        }

        [Fact]
        public async Task MySqlHelper_SelectAsync_Projects()
        {
            MySqlHelper helper = await SeededHelperAsync();

            List<int> ids = await helper.SelectAsync<TestEntity, int>(e => e.Description == "y", e => e.Id);

            Assert.Equal(2, ids.Count);
            Assert.Contains(3, ids);
            Assert.Contains(4, ids);
        }

        [Fact]
        public async Task MySqlHelper_AnyAsync_NoPredicate_AndPredicate()
        {
            MySqlHelper helper = await SeededHelperAsync();

            Assert.True(await helper.AnyAsync<TestEntity>());
            Assert.True(await helper.AnyAsync<TestEntity>(e => e.Name == "c"));
            Assert.False(await helper.AnyAsync<TestEntity>(e => e.Name == "zzz"));
        }
    }
}
