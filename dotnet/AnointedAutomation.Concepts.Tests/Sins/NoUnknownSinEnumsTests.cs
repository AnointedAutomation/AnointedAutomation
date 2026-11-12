// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;
using System.Linq;
using AnointedAutomation.Concepts;
using AnointedAutomation.Enums;
using Xunit;

namespace AnointedAutomation.Concepts.Tests.Sins
{
    /// <summary>
    /// Locks in the deliberate exceptions to the enum-zero convention (value 0 is normally Unknown or None).
    /// A sin is never "unknown" (owner, 2026-11-12), so these enums have no Unknown, None or Default member:
    /// <list type="bullet">
    /// <item><description><see cref="SinId"/>: no member at 0; ids are the checklist numbers 1 to 668.</description></item>
    /// <item><description><see cref="SinCategory"/>: real categories only, numbered from 1.</description></item>
    /// <item><description><c>AnointedAutomation.Enums.Sin</c>: <c>Blasphemy = 0</c> (stored as a ban reason).</description></item>
    /// </list>
    /// "No sin" is expressed with a nullable (<c>SinId?</c>, <c>SinCategory?</c>), never a magic value.
    /// The other known zero exception is <c>MessageType.Error = 0</c> in AnointedAutomation.Logging.
    /// </summary>
    public class NoUnknownSinEnumsTests
    {
        private static readonly string[] ForbiddenNames = { "Unknown", "None", "Default", "Undefined", "NotSet", "Unspecified" };

        [Fact]
        public void SinId_HasNoUnknownOrNoneMember_AndNothingIsZero()
        {
            AssertNoPlaceholderMember(typeof(SinId));
            Assert.DoesNotContain(Enum.GetValues<SinId>(), id => (uint)id == 0u);
            Assert.False(Enum.IsDefined(default(SinId)));
        }

        [Fact]
        public void SinCategory_HasNoUnknownOrNoneMember_AndNothingIsZero()
        {
            AssertNoPlaceholderMember(typeof(SinCategory));
            Assert.DoesNotContain(Enum.GetValues<SinCategory>(), c => (uint)c == 0u);
            Assert.False(Enum.IsDefined(default(SinCategory)));
        }

        [Fact]
        public void CoarseSin_StillStartsAtBlasphemyZero()
        {
            Assert.Equal(0u, (uint)Sin.Blasphemy);
            AssertNoPlaceholderMember(typeof(Sin));
        }

        private static void AssertNoPlaceholderMember(Type enumType)
        {
            string[] names = Enum.GetNames(enumType);
            foreach (string forbidden in ForbiddenNames)
            {
                Assert.False(
                    names.Any(n => string.Equals(n, forbidden, StringComparison.OrdinalIgnoreCase)),
                    enumType.Name + " must not have a member named " + forbidden + ": a sin is never unknown.");
            }
        }
    }
}
