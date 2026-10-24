// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using Owin;
using System;
using System.Collections.Generic;

namespace AnointedAutomation.SSO.Owin.Tests
{
    /// <summary>An app builder that records the middleware types registered on it.</summary>
    internal sealed class FakeAppBuilder : IAppBuilder
    {
        /// <summary>The app properties.</summary>
        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();

        /// <summary>Each registered middleware (its type, or the delegate's type).</summary>
        public List<Type> Registered { get; } = new List<Type>();

        /// <summary>Records the middleware.</summary>
        /// <param name="middleware">The middleware.</param>
        /// <param name="args">Its arguments.</param>
        /// <returns>This builder.</returns>
        public IAppBuilder Use(object middleware, params object[] args)
        {
            Type type = middleware as Type;
            Registered.Add(type == null ? middleware.GetType() : type);
            return this;
        }

        /// <summary>Not used.</summary>
        /// <returns>Nothing.</returns>
        public object Build(Type returnType)
        {
            throw new NotSupportedException();
        }

        /// <summary>Not used.</summary>
        /// <returns>Nothing.</returns>
        public IAppBuilder New()
        {
            throw new NotSupportedException();
        }
    }
}
