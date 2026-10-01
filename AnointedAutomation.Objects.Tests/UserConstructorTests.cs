// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AnointedAutomation.Objects.Account;
using MongoDB.Bson;
using Xunit;

namespace AnointedAutomation.Objects.Tests
{
    public class UserConstructorTests
    {
        private static ConstructorInfo FullConstructor()
        {
            ConstructorInfo[] ctors = typeof(User).GetConstructors();
            return ctors.OrderByDescending(c => c.GetParameters().Length).First();
        }

        private static List<PropertyInfo> SettableProperties()
        {
            return typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.SetMethod.IsPublic)
                .ToList();
        }

        [Fact]
        public void FullConstructor_CoversEverySettableProperty_InDeclarationOrder()
        {
            ParameterInfo[] parameters = FullConstructor().GetParameters();
            List<PropertyInfo> props = SettableProperties();

            Assert.Equal(props.Count, parameters.Length);
            for (int i = 0; i < props.Count; i++)
            {
                Assert.True(props[i].Name.Equals(parameters[i].Name, StringComparison.OrdinalIgnoreCase),
                    "Position " + i + ": property " + props[i].Name + " vs parameter " + parameters[i].Name);
                Assert.Equal(props[i].PropertyType, parameters[i].ParameterType);
            }
        }

        private static object SampleValue(Type type, int seed)
        {
            if (type == typeof(string)) return "v" + seed;
            if (type == typeof(bool)) return true;
            if (type == typeof(DateTime)) return new DateTime(2000 + seed, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            if (type == typeof(TimeSpan)) return TimeSpan.FromMinutes(seed + 1);
            return Activator.CreateInstance(type);
        }

        [Fact]
        public void FullConstructor_AssignsEveryValueExactly()
        {
            ConstructorInfo ctor = FullConstructor();
            ParameterInfo[] parameters = ctor.GetParameters();
            object[] args = parameters.Select((p, i) => SampleValue(p.ParameterType, i)).ToArray();

            User user = (User)ctor.Invoke(args);

            List<PropertyInfo> props = SettableProperties();
            for (int i = 0; i < props.Count; i++)
            {
                object actual = props[i].GetValue(user);
                if (props[i].PropertyType.IsValueType || props[i].PropertyType == typeof(string))
                {
                    Assert.Equal(args[i], actual);
                }
                else
                {
                    Assert.Same(args[i], actual);
                }
            }
        }

        [Fact]
        public void FullConstructor_AppliesNoDefaults()
        {
            User user = new User(default, null, null, default, null, null, false, null, null, null, false, default,
                null, null, null, null, null, default, null, default, null, null, default, null, default);

            Assert.Equal(default(DateTime), user.banned);
            Assert.Equal(default(DateTime), user.createdDate);
            Assert.Equal(default(DateTime), user.lastActiveDate);
            Assert.Null(user.IPAddresses);
            Assert.Null(user.Profile);
            Assert.Null(user.Password);
        }

        [Fact]
        public void LegacyConstructor_NullPassword_DoesNotThrow_AndUsesUtcDefaults()
        {
            DateTime before = DateTime.UtcNow;
            User user = new User(default, default, "a@x.com", false, null, default, null, null, "user",
                TimeSpan.Zero, "u1", "name", null, default);
            DateTime after = DateTime.UtcNow;

            Assert.Null(user.Password);
            Assert.Equal(new DateTime(1900, 1, 1), user.banned);
            Assert.Equal(DateTimeKind.Utc, user.createdDate.Kind);
            Assert.Equal(DateTimeKind.Utc, user.lastActiveDate.Kind);
            Assert.InRange(user.createdDate, before, after);
            Assert.InRange(user.lastActiveDate, before, after);
            Assert.NotNull(user.IPAddresses);
            Assert.Empty(user.IPAddresses);
            Assert.NotNull(user.Profile);
        }

        [Fact]
        public void ToBsonDocument_ElementOrder_KeepsEmailsAfterEmail()
        {
            User user = new User(default, null, null, default, "a@x.com",
                new List<UserEmail> { new UserEmail { Address = "a@x.com", IsPrimary = true } }, false, null, null,
                null, false, default, null, null, null, null, null, default, null, default, "u1", null, default, null,
                default);

            List<string> names = user.ToBsonDocument().Names.ToList();
            int email = names.FindIndex(n => n.Equals("Email", StringComparison.Ordinal));
            int emails = names.FindIndex(n => n.Equals("Emails", StringComparison.Ordinal));

            Assert.True(email >= 0, string.Join(",", names));
            Assert.Equal(email + 1, emails);
        }
    }
}
