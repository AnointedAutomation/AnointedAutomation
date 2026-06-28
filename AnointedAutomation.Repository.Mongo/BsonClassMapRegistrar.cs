// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using System;
using AnointedAutomation.Objects.Account;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using Newtonsoft.Json.Linq;

namespace AnointedAutomation.Repository.Mongo
{
    /// <summary>
    /// Registers BSON class maps for proper MongoDB serialization.
    /// Call <see cref="RegisterClassMaps"/> once at application startup before any MongoDB operations.
    /// </summary>
    public static class BsonClassMapRegistrar
    {
        private static bool _isRegistered = false;
        private static readonly object _lock = new object();

        /// <summary>
        /// Registers all BSON class maps for the AnointedAutomation.Objects hierarchy.
        /// Thread-safe and idempotent - calling multiple times has no effect.
        /// </summary>
        public static void RegisterClassMaps()
        {
            if (_isRegistered)
            {
                return;
            }

            lock (_lock)
            {
                if (_isRegistered)
                {
                    return;
                }

                RegisterGlobalSerializers();
                RegisterUserClassMaps();

                _isRegistered = true;
            }
        }

        /// <summary>
        /// OPT-IN (not auto-applied): registers the hybrid casing convention (value/enum/struct -> camelCase,
        /// reference -> PascalCase) for types under <paramref name="namespacePrefix"/>. Publishing this package
        /// changes NOTHING until a service calls this. Call ONCE at startup BEFORE <see cref="RegisterClassMaps"/>
        /// / any Mongo use, and ONLY after that service's data has been migrated to hybrid keys — the convention
        /// does not override explicit [BsonElement], so pure-casing [BsonElement] attributes must be removed for
        /// it to take full effect.
        /// </summary>
        /// <param name="namespacePrefix">Restrict to a namespace (e.g. one API) to stage rollout. Default: all AnointedAutomation types.</param>
        public static void RegisterHybridCasingConvention(string namespacePrefix = "AnointedAutomation")
        {
            ConventionPack pack = new ConventionPack { new HybridElementNameConvention() };
            ConventionRegistry.Register(
                "AnointedHybridElementName",
                pack,
                t => t.FullName != null && t.FullName.StartsWith(namespacePrefix, StringComparison.Ordinal));
        }

        /// <summary>
        /// OPT-IN (not auto-applied): registers the snake_case element-name convention ("Anointed Styling":
        /// structural Mongo keys snake_case) for types under <paramref name="namespacePrefix"/>. Publishing this
        /// package changes NOTHING until a service calls this. Call ONCE at startup BEFORE
        /// <see cref="RegisterClassMaps"/> / any Mongo use, and ONLY after that service's data has been migrated
        /// to snake_case keys — the convention does not override explicit [BsonElement], so pure-casing
        /// [BsonElement] attributes must be removed/snaked for it to take full effect.
        /// </summary>
        /// <param name="namespacePrefix">Restrict to a namespace (e.g. one API) to stage rollout. Default: all AnointedAutomation types.</param>
        public static void RegisterSnakeCasingConvention(string namespacePrefix = "AnointedAutomation")
        {
            ConventionPack pack = new ConventionPack { new SnakeCaseElementNameConvention() };
            ConventionRegistry.Register(
                "AnointedSnakeElementName",
                pack,
                t => t.FullName != null && t.FullName.StartsWith(namespacePrefix, StringComparison.Ordinal));
        }

        private static void RegisterGlobalSerializers()
        {
            // Register JObjectSerializer globally for all JObject properties
            // This handles Newtonsoft.Json JObject <-> MongoDB BSON conversion
            BsonSerializer.RegisterSerializer(typeof(JObject), new JObjectSerializer());
        }

        private static void RegisterUserClassMaps()
        {
            // Register User class with UserId as the document ID
            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIsRootClass(true);
                    cm.SetIgnoreExtraElements(true);
                    cm.MapIdMember(x => x.UserId);
                });
            }
        }

        /// <summary>
        /// Registers a derived user class (e.g., PerilousUser) with BSON class maps.
        /// Call this after <see cref="RegisterClassMaps"/> for each derived user type.
        /// </summary>
        /// <typeparam name="T">The derived user type.</typeparam>
        public static void RegisterDerivedUser<T>() where T : User
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
            {
                BsonClassMap.RegisterClassMap<T>(cm =>
                {
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                });
            }
        }
    }
}
