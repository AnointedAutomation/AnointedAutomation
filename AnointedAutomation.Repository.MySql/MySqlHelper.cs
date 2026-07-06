// Copyright 2024 Anointed Automation, LLC. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me
// Stewarded by Alexander Fields

using AnointedAutomation.Optimization.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Repository.MySql
{
    /// <summary>
    /// Helper class for MySQL operations using Entity Framework Core.
    /// Provides generic CRUD operations and connection management.
    /// </summary>
    public class MySqlHelper : IMySqlHelper
    {
        private static readonly ConcurrentQueue<LogMessage> mysqlLogs = new ConcurrentQueue<LogMessage>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MySqlHelper()
        {
        }

        /// <summary>
        /// Constructor that initializes with a connection string.
        /// </summary>
        /// <param name="connectionString">The MySQL connection string.</param>
        public MySqlHelper(string connectionString)
        {
            ConnectionString = connectionString;
            Database = CreateDbContext(connectionString);
        }

        /// <summary>
        /// Finalizer for cleanup.
        /// </summary>
        ~MySqlHelper()
        {
            ClearLogs();
        }

        /// <summary>
        /// Event triggered when a log message is added.
        /// </summary>
        public static event EventHandler<LogMessageEventArgs> LogAdded;

        /// <summary>
        /// Event triggered when logs are cleared.
        /// </summary>
        public static event EventHandler LogCleared;

        /// <summary>
        /// Gets or sets the Entity Framework DbContext instance.
        /// </summary>
        public DbContext Database { get; set; }

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Clears all MySQL operation logs and triggers the LogCleared event.
        /// </summary>
        public static void ClearLogs()
        {
            mysqlLogs?.Clear();
            LogCleared?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Gets all MySQL operation logs.
        /// </summary>
        /// <returns>A list of all log messages.</returns>
        public static IList<LogMessage> GetLogs()
        {
            return mysqlLogs.ToArray();
        }

        /// <summary>
        /// Builds a MySQL connection string from components.
        /// </summary>
        /// <param name="server">The server hostname or IP.</param>
        /// <param name="database">The database name.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="port">The port (default 3306).</param>
        /// <returns>A MySQL connection string.</returns>
        public static string ConnectionStringBuilder(
            string server,
            string database,
            string username,
            string password,
            int port = 3306
        )
        {
            return $"Server={server};Port={port};Database={database};User={username};Password={password};";
        }

        /// <summary>
        /// Helper for connecting to database.
        /// </summary>
        /// <param name="mySqlHelper">Instance of the MySQL helper that will connect.</param>
        /// <param name="connectionString">The MySQL connection string.</param>
        /// <returns>The configured MySQL helper.</returns>
        public static IMySqlHelper MySqlHelperConnector(IMySqlHelper mySqlHelper, string connectionString)
        {
            mySqlHelper.Database = mySqlHelper.CreateDbContext(connectionString);
            mySqlHelper.ConnectionString = connectionString;

            try
            {
                bool connected = mySqlHelper.TestConnection();
                if (!connected)
                {
                    AddLog(LogMessage.Error("Failed to connect to MySQL database"));
                }
            }
            catch (Exception ex)
            {
                AddLog(LogMessage.Error(ex.Message));
            }

            return mySqlHelper;
        }

        /// <summary>
        /// Creates a new entity in the database.
        /// </summary>
        public async Task<T> CreateAsync<T>(T entity) where T : class
        {
            Database.Set<T>().Add(entity);
            await Database.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Gets an entity by its primary key.
        /// </summary>
        public async Task<T> GetByIdAsync<T>(object id) where T : class
        {
            return await Database.Set<T>().FindAsync(id);
        }

        /// <summary>
        /// Gets all entities of the specified type.
        /// </summary>
        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await Database.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Gets entities that match the specified predicate.
        /// </summary>
        public async Task<List<T>> GetFilteredAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await Database.Set<T>().Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        public async Task<T> UpdateAsync<T>(T entity) where T : class
        {
            Database.Set<T>().Update(entity);
            await Database.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Deletes an entity by its primary key.
        /// </summary>
        public async Task<bool> DeleteAsync<T>(object id) where T : class
        {
            T entity = await Database.Set<T>().FindAsync(id);
            if (entity == null)
            {
                return false;
            }

            Database.Set<T>().Remove(entity);
            await Database.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        public async Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await Database.Set<T>().AnyAsync(predicate);
        }

        /// <summary>
        /// Counts entities that match the specified predicate.
        /// </summary>
        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return await Database.Set<T>().CountAsync();
            }
            return await Database.Set<T>().CountAsync(predicate);
        }

        /// <summary>
        /// Gets the first entity matching the predicate, or null if none matches.
        /// </summary>
        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await Database.Set<T>().FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Gets a page of entities: filter (optional), order by a key, then skip/take.
        /// </summary>
        public async Task<List<T>> GetPagedAsync<T, TKey>(Expression<Func<T, bool>> predicate, Expression<Func<T, TKey>> orderBy, bool descending, int skip, int take) where T : class
        {
            IQueryable<T> query = Database.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            return await query.Skip(skip).Take(take).ToListAsync();
        }

        /// <summary>
        /// Gets entities (optionally filtered) ordered by a key.
        /// </summary>
        public async Task<List<T>> GetOrderedAsync<T, TKey>(Expression<Func<T, TKey>> orderBy, bool descending, Expression<Func<T, bool>> predicate = null) where T : class
        {
            IQueryable<T> query = Database.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            query = descending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);
            return await query.ToListAsync();
        }

        /// <summary>
        /// Projects matching entities to a smaller shape, selecting only what you need.
        /// </summary>
        public async Task<List<TResult>> SelectAsync<T, TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector) where T : class
        {
            IQueryable<T> query = Database.Set<T>();
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            return await query.Select(selector).ToListAsync();
        }

        /// <summary>
        /// Returns true if any entity exists (optionally matching the predicate).
        /// </summary>
        public async Task<bool> AnyAsync<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            if (predicate == null)
            {
                return await Database.Set<T>().AnyAsync();
            }
            return await Database.Set<T>().AnyAsync(predicate);
        }

        /// <summary>
        /// Executes a raw, parameterized non-query SQL statement (INSERT/UPDATE/DELETE/DDL).
        /// </summary>
        public async Task<int> ExecuteRawSqlAsync(string sql, params object[] parameters)
        {
            return await Database.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <summary>
        /// Runs the supplied work inside a single database transaction, committing on success and
        /// rolling back if it throws.
        /// </summary>
        public async Task ExecuteInTransactionAsync(Func<Task> action)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction =
                await Database.Database.BeginTransactionAsync())
            {
                try
                {
                    await action();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates a new DbContext for MySQL.
        /// </summary>
        public DbContext CreateDbContext(string connectionString)
        {
            DbContextOptionsBuilder<DynamicDbContext> optionsBuilder = new DbContextOptionsBuilder<DynamicDbContext>();

            // Use Pomelo MySQL provider
            ServerVersion serverVersion = ServerVersion.AutoDetect(connectionString);
            optionsBuilder.UseMySql(connectionString, serverVersion);

            DynamicDbContext context = new DynamicDbContext(optionsBuilder.Options);
            return context;
        }

        /// <summary>
        /// Tests the connection to the database.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                bool canConnect = Database.Database.CanConnect();
                if (canConnect)
                {
                    AddLog(LogMessage.Success("Successfully connected to MySQL database"));
                }
                return canConnect;
            }
            catch (Exception ex)
            {
                AddLog(LogMessage.Error("Was unable to properly connect to the database! " + ex.ToString()));
                return false;
            }
        }

        /// <summary>
        /// Gets a list of all table names in the database.
        /// </summary>
        public List<string> GetTableNames()
        {
            try
            {
                // Query MySQL information_schema for table names
                List<string> tableNames = new List<string>();
                Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade db = Database.Database;

                using (System.Data.Common.DbCommand command = db.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = DATABASE()";
                    db.OpenConnection();

                    using (System.Data.Common.DbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tableNames.Add(reader.GetString(0));
                        }
                    }
                }

                return tableNames;
            }
            catch (Exception ex)
            {
                AddLog(LogMessage.Error("Failed to get table names: " + ex.ToString()));
                return null;
            }
        }

        /// <summary>
        /// Adds a log message to the MySQL operation logs.
        /// </summary>
        protected static void AddLog(LogMessage logMessage)
        {
            mysqlLogs.Enqueue(logMessage);
            LogAdded?.Invoke(null, new LogMessageEventArgs(logMessage));
        }
    }

    /// <summary>
    /// Dynamic DbContext that can be used without predefined entity configurations.
    /// For production use, inherit from this class and add your DbSet properties.
    /// </summary>
    public class DynamicDbContext : DbContext
    {
        /// <summary>
        /// Constructor with options.
        /// </summary>
        public DynamicDbContext(DbContextOptions<DynamicDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Override to configure the model.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
