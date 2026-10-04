// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
//
// ONE bootstrap for the hybrid BSON setup every Anointed process (API, tools, test fixtures) needs, in the ONLY safe
// order. The order is load-bearing:
//   1. casing convention   (class maps bake element names in when built, so it must exist first)
//   2. class maps callback (BsonClassMapRegistrar.RegisterClassMaps: the User map binds UserId to _id)
//   3. tolerance loop      (IgnoreExtraElements on everything NOT already mapped; running it before step 2
//                           AutoMaps User without MapIdMember, UserId reads null and every login throws)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MongoDB.Bson.Serialization.Conventions;
using AnointedAutomation.Repository.Mongo;

namespace AnointedAutomation.Repository.Mongo.Bson
{
    /// <summary>Hybrid element-name casing bootstrap for MongoDB.</summary>
    public static class HybridCasing
    {
        /// <summary>The convention-registry name used by the Anointed API and its tools.</summary>
        public const string ConventionName = "hybrid-anointed";

        private static readonly object Gate = new object();
        private static readonly HashSet<string> RegisteredPrefixes = new HashSet<string>(StringComparer.Ordinal);

        /// <summary>
        /// Registers the hybrid convention, runs <paramref name="registerClassMaps"/>, then the tolerance loop, in that
        /// order. The convention applies to every type whose namespace starts with <paramref name="namespacePrefix"/>
        /// (ordinal) and whose simple <c>Type.Name</c> is NOT in <paramref name="denyList"/> (ordinal), exactly like the
        /// API's inline registration. Idempotent per <paramref name="namespacePrefix"/>: a second call with the same
        /// prefix does nothing (the convention is never registered twice).
        /// </summary>
        /// <param name="namespacePrefix">Namespace prefix the convention and tolerance apply to (e.g. "AnointedAutomation").</param>
        /// <param name="denyList">Simple type names that keep their verbatim element names (preserve-blob / facet modellers). May be empty.</param>
        /// <param name="registerClassMaps">Explicit class-map registration, normally <c>BsonClassMapRegistrar.RegisterClassMaps</c>. May be null.</param>
        /// <param name="toleranceAssemblies">Assemblies whose types get IgnoreExtraElements maps. May be empty.</param>
        /// <param name="ignoreExtraElements">
        /// When true, also registers an <see cref="IgnoreExtraElementsConvention"/> for the same type filter, so EVERY map
        /// built later (including ones registered explicitly, and API types outside <paramref name="toleranceAssemblies"/>)
        /// ignores unknown elements. Default false reproduces the API's current behavior.
        /// </param>
        public static void Register(
            string namespacePrefix,
            IEnumerable<string> denyList,
            Action registerClassMaps,
            IEnumerable<Assembly> toleranceAssemblies,
            bool ignoreExtraElements = false)
        {
            if (namespacePrefix == null)
            {
                throw new ArgumentNullException(nameof(namespacePrefix));
            }

            string[] deny = denyList == null ? Array.Empty<string>() : denyList.ToArray();
            Assembly[] assemblies = toleranceAssemblies == null ? Array.Empty<Assembly>() : toleranceAssemblies.ToArray();

            lock (Gate)
            {
                if (RegisteredPrefixes.Contains(namespacePrefix))
                {
                    return;
                }

                RegisterConvention(namespacePrefix, deny, ignoreExtraElements);
                registerClassMaps?.Invoke();
                BsonToleranceRegistrar.RegisterIgnoreExtraElements(assemblies, namespacePrefix);
                RegisteredPrefixes.Add(namespacePrefix);
            }
        }

        /// <summary>The type filter <see cref="Register"/> uses: namespace starts with the prefix and simple name not denied.</summary>
        public static Func<Type, bool> BuildFilter(string namespacePrefix, IEnumerable<string> denyList)
        {
            if (namespacePrefix == null)
            {
                throw new ArgumentNullException(nameof(namespacePrefix));
            }

            string[] deny = denyList == null ? Array.Empty<string>() : denyList.ToArray();
            return t => (t.Namespace ?? string.Empty).StartsWith(namespacePrefix, StringComparison.Ordinal)
                && !deny.Contains(t.Name, StringComparer.Ordinal);
        }

        private static void RegisterConvention(string namespacePrefix, string[] deny, bool ignoreExtraElements)
        {
            ConventionPack pack = new ConventionPack { new HybridElementNameConvention() };
            if (ignoreExtraElements)
            {
                pack.Add(new IgnoreExtraElementsConvention(true));
            }

            // Name kept as the API's ("hybrid-anointed") for the default prefix so diagnostics read the same.
            string name = string.Equals(namespacePrefix, "AnointedAutomation", StringComparison.Ordinal)
                ? ConventionName
                : ConventionName + ":" + namespacePrefix;
            ConventionRegistry.Register(name, pack, BuildFilter(namespacePrefix, deny));
        }
    }
}
