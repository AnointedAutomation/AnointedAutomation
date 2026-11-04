// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace AnointedAutomation.Repository.Mongo.Tests
{
    /// <summary>
    /// Tests for DelegatingMongoHelper. The reflection tests walk every IMongoHelper member, so a member added to the
    /// interface must be forwarded by DelegatingMongoHelper in the same change or these tests fail.
    /// </summary>
    public class DelegatingMongoHelperTests
    {
        private sealed class TestHelper : DelegatingMongoHelper
        {
            private readonly IMongoHelper _inner;
            public int CurrentCalls { get; private set; }
            public TestHelper(IMongoHelper inner) { _inner = inner; }
            protected override IMongoHelper Current() { CurrentCalls++; return _inner; }
        }

        private sealed class CustomMessageHelper : DelegatingMongoHelper
        {
            protected override IMongoHelper Current() => throw new InvalidOperationException("Current must not be called by a setter");
            protected override string NeverSetMessage => "custom never-set message";
        }

        /// <summary>Records the last call made to it and returns a marker value of the method's return type.</summary>
        public class RecordingProxy : DispatchProxy
        {
            public MethodInfo LastMethod { get; private set; }
            public object[] LastArgs { get; private set; }
            public object ReturnValue { get; set; }

            protected override object Invoke(MethodInfo targetMethod, object[] args)
            {
                LastMethod = targetMethod;
                LastArgs = args;
                return ReturnValue;
            }
        }

        private static IEnumerable<MethodInfo> InterfaceMethods() =>
            typeof(IMongoHelper).GetMethods().Where(m => !m.IsSpecialName);

        public static IEnumerable<object[]> MemberNames() =>
            InterfaceMethods().Select(m => new object[] { m.ToString() });

        private static MethodInfo Close(MethodInfo method) =>
            method.IsGenericMethodDefinition
                ? method.MakeGenericMethod(method.GetGenericArguments().Select(_ => typeof(object)).ToArray())
                : method;

        private static object SampleArg(Type type, int index)
        {
            if (type == typeof(string)) return "arg" + index;
            if (type == typeof(int)) return 100 + index;
            if (type == typeof(bool)) return index % 2 == 0;
            return null;
        }

        private static object SampleReturn(Type type)
        {
            if (type == typeof(void)) return null;
            if (type == typeof(Task)) return Task.CompletedTask;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
            {
                Type result = type.GetGenericArguments()[0];
                object value = result.IsValueType ? Activator.CreateInstance(result) : null;
                return typeof(Task).GetMethod(nameof(Task.FromResult)).MakeGenericMethod(result).Invoke(null, new[] { value });
            }
            if (type == typeof(List<string>)) return new List<string> { "marker" };
            return null;
        }

        [Theory]
        [MemberData(nameof(MemberNames))]
        public void EveryInterfaceMethod_PassesThroughToCurrent(string member)
        {
            MethodInfo method = InterfaceMethods().Single(m => m.ToString() == member);
            MethodInfo closed = Close(method);
            IMongoHelper inner = DispatchProxy.Create<IMongoHelper, RecordingProxy>();
            RecordingProxy recorder = (RecordingProxy)(object)inner;
            object expectedReturn = SampleReturn(closed.ReturnType);
            recorder.ReturnValue = expectedReturn;
            TestHelper helper = new TestHelper(inner);
            object[] args = closed.GetParameters().Select((p, i) => SampleArg(p.ParameterType, i)).ToArray();

            object actual = closed.Invoke(helper, args);

            Assert.Equal(1, helper.CurrentCalls);
            Assert.NotNull(recorder.LastMethod);
            Assert.Equal(closed.ToString(), recorder.LastMethod.ToString());
            Assert.Equal(args, recorder.LastArgs);
            Assert.Same(expectedReturn, actual);
        }

        [Fact]
        public void EveryInterfaceMember_IsImplementedByDelegatingMongoHelperAsVirtual()
        {
            InterfaceMapping map = typeof(DelegatingMongoHelper).GetInterfaceMap(typeof(IMongoHelper));
            Assert.Equal(typeof(IMongoHelper).GetMethods().Length, map.TargetMethods.Length);
            foreach (MethodInfo target in map.TargetMethods)
            {
                Assert.Equal(typeof(DelegatingMongoHelper), target.DeclaringType);
                Assert.True(target.IsVirtual && !target.IsFinal, target + " must be overridable");
            }
        }

        [Fact]
        public void PropertyGetters_ReadFromCurrent()
        {
            Mock<IMongoDatabase> database = new Mock<IMongoDatabase>();
            Mock<IMongoHelper> inner = new Mock<IMongoHelper>();
            inner.SetupGet(h => h.database).Returns(database.Object);
            inner.SetupGet(h => h.dbName).Returns("tenant-db");
            TestHelper helper = new TestHelper(inner.Object);

            Assert.Same(database.Object, helper.database);
            Assert.Equal("tenant-db", helper.dbName);
            Assert.Equal(2, helper.CurrentCalls);
        }

        [Fact]
        public void Setters_ThrowDefaultMessage_AndNeverTouchCurrent()
        {
            Mock<IMongoHelper> inner = new Mock<IMongoHelper>(MockBehavior.Strict);
            TestHelper helper = new TestHelper(inner.Object);

            InvalidOperationException dbError = Assert.Throws<InvalidOperationException>(() => helper.database = new Mock<IMongoDatabase>().Object);
            InvalidOperationException nameError = Assert.Throws<InvalidOperationException>(() => helper.dbName = "x");

            Assert.Contains("TestHelper", dbError.Message);
            Assert.Equal(dbError.Message, nameError.Message);
            Assert.Equal(0, helper.CurrentCalls);
        }

        [Fact]
        public void Setters_UseOverriddenMessage()
        {
            CustomMessageHelper helper = new CustomMessageHelper();

            Assert.Equal("custom never-set message", Assert.Throws<InvalidOperationException>(() => helper.database = null).Message);
            Assert.Equal("custom never-set message", Assert.Throws<InvalidOperationException>(() => helper.dbName = null).Message);
        }

        [Fact]
        public async Task Current_IsResolvedOnEveryCall()
        {
            Mock<IMongoHelper> first = new Mock<IMongoHelper>();
            Mock<IMongoHelper> second = new Mock<IMongoHelper>();
            first.Setup(h => h.CountAsync("c", It.IsAny<FilterDefinition<object>>())).ReturnsAsync(1L);
            second.Setup(h => h.CountAsync("c", It.IsAny<FilterDefinition<object>>())).ReturnsAsync(2L);
            SwitchingHelper helper = new SwitchingHelper(first.Object);

            long a = await helper.CountAsync("c", FilterDefinition<object>.Empty);
            helper.Target = second.Object;
            long b = await helper.CountAsync("c", FilterDefinition<object>.Empty);

            Assert.Equal(1L, a);
            Assert.Equal(2L, b);
        }

        private sealed class SwitchingHelper : DelegatingMongoHelper
        {
            public IMongoHelper Target { get; set; }
            public SwitchingHelper(IMongoHelper target) { Target = target; }
            protected override IMongoHelper Current() => Target;
        }
    }
}
