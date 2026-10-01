// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// Makes every type we own tolerate unknown BSON elements. BsonClassMapRegistrar sets IgnoreExtraElements on User but
// NOT on the types nested inside it, so one undeclared key in a nested document (GoogleTokenInfo.expiresIn) failed the
// WHOLE read and took down login. A stray field must degrade to "ignore that field". Moved from the Anointed API
// (AnointedAutomation.API.Configuration.BsonToleranceRegistrar) and parameterized.

using System;
using System.Collections.Generic;
using System.Reflection;
using AnointedAutomation.Optimization.Logging;
using MongoDB.Bson.Serialization;

namespace AnointedAutomation.Repository.Mongo.Bson
{
    /// <summary>Registers permissive (IgnoreExtraElements) class maps for every type in the given assemblies.</summary>
    public static class BsonToleranceRegistrar
    {
        /// <summary>
        /// AutoMaps every concrete, non-generic-definition, non-compiler-generated class in <paramref name="assemblies"/>
        /// whose namespace starts with <paramref name="namespacePrefix"/> (ordinal) with IgnoreExtraElements, skipping
        /// any type that already has a class map so nothing configured is overridden. A type the driver cannot AutoMap
        /// is logged as a warning and skipped. Idempotent.
        /// MUST run AFTER the casing convention (the maps bake in element names) AND AFTER
        /// <see cref="BsonClassMapRegistrar.RegisterClassMaps"/> (User's special map binds UserId to _id; a plain
        /// AutoMap of User here would win and break every login). <see cref="HybridCasing.Register"/> enforces that order.
        /// </summary>
        public static void RegisterIgnoreExtraElements(IEnumerable<Assembly> assemblies, string namespacePrefix = "AnointedAutomation")
        {
            if (assemblies == null)
            {
                throw new ArgumentNullException(nameof(assemblies));
            }

            if (namespacePrefix == null)
            {
                throw new ArgumentNullException(nameof(namespacePrefix));
            }

            foreach (Assembly assembly in assemblies)
            {
                RegisterAssembly(assembly, namespacePrefix);
            }
        }

        private static void RegisterAssembly(Assembly assembly, string namespacePrefix)
        {
            foreach (Type mapped in assembly.GetTypes())
            {
                if (!mapped.IsClass || mapped.IsAbstract || mapped.IsGenericTypeDefinition
                    || mapped.Name.Contains('<', StringComparison.Ordinal)
                    || !(mapped.Namespace ?? string.Empty).StartsWith(namespacePrefix, StringComparison.Ordinal)
                    || BsonClassMap.IsClassMapRegistered(mapped))
                {
                    continue;
                }

                try
                {
                    BsonClassMap cm = new BsonClassMap(mapped);
                    cm.AutoMap();
                    cm.SetIgnoreExtraElements(true);
                    BsonClassMap.RegisterClassMap(cm);
                }
                catch (Exception ex)
                {
                    LogMessage.Warning($"BsonToleranceRegistrar.RegisterIgnoreExtraElements: skipping type {mapped.FullName} that could not be automapped: {ex.Message}");
                }
            }
        }
    }
}
