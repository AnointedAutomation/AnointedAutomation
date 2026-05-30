// Copyright © Anointed Automation, LLC., 2024. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

using AnointedAutomation.Objects.Account;
using MongoDB.Bson.Serialization;
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
