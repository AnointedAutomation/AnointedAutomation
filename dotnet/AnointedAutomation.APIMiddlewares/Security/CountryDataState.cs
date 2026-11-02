// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-02 00:00:00 Jesus is King ✝️
using System.Net;
using System.Threading;

namespace AnointedAutomation.APIMiddleware.Security
{
    /// <summary>
    /// The live country data: an <see cref="ICountryLookup"/> that delegates to the last good snapshot, which is
    /// swapped atomically by <see cref="CountryDataRefresher"/>. Unknown for everything until the first load.
    /// </summary>
    public sealed class CountryDataState : ICountryLookup
    {
        private ICountryLookup _current;

        /// <summary>Gets whether any data has loaded.</summary>
        public bool HasData
        {
            get { return Volatile.Read(ref _current) != null; }
        }

        /// <summary>
        /// Swaps in a new snapshot.
        /// </summary>
        /// <param name="lookup">The new lookup.</param>
        public void Replace(ICountryLookup lookup)
        {
            Volatile.Write(ref _current, lookup);
        }

        /// <inheritdoc />
        public string Lookup(IPAddress address)
        {
            ICountryLookup current = Volatile.Read(ref _current);
            return current == null ? null : current.Lookup(address);
        }
    }
}
