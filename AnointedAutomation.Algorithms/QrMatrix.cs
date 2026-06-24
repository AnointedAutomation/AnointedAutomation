// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me

namespace AnointedAutomation.Algorithms.QrCode
{
    /// <summary>
    /// A square grid of QR modules. Tracks which modules are dark and which are "function"
    /// modules (finder/timing/alignment/format/version) that data and masking must not touch.
    /// </summary>
    public sealed class QrMatrix
    {
        private readonly bool[,] _dark;
        private readonly bool[,] _function;

        /// <summary>Module count along one side.</summary>
        public int Size { get; }

        internal QrMatrix(int size)
        {
            Size = size;
            _dark = new bool[size, size];
            _function = new bool[size, size];
        }

        /// <summary>True if the module at (x, y) is dark.</summary>
        public bool IsDark(int x, int y)
        {
            return _dark[y, x];
        }

        internal bool IsFunction(int x, int y)
        {
            return _function[y, x];
        }

        internal void Set(int x, int y, bool dark)
        {
            _dark[y, x] = dark;
        }

        internal void SetFunction(int x, int y, bool dark)
        {
            _dark[y, x] = dark;
            _function[y, x] = true;
        }

        internal void Toggle(int x, int y)
        {
            _dark[y, x] = !_dark[y, x];
        }
    }
}
