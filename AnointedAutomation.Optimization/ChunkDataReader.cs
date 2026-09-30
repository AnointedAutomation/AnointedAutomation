// Copyright 2024 Anointed Automation, LLC All rights reserved.
// Originally created by Alexander Fields. Ported and modernized for the AnointedAutomation monorepo.

using System;
using System.Collections.Generic;
using System.Data;

namespace AnointedAutomation.Optimization
{
    /// <summary>
    /// An in-memory <see cref="IDataReader"/> over a set of rows, useful for feeding batched data
    /// into APIs that expect a data reader (for example bulk-copy or CSV export helpers).
    /// </summary>
    public class ChunkDataReader : IDataReader
    {
        private readonly List<object?[]> _rows;
        private readonly string[] _columnNames;
        private readonly Type[] _columnTypes;

        private int _currentRowIndex = -1;

        /// <summary>Creates a reader over the supplied column metadata and rows.</summary>
        public ChunkDataReader(string[] columnNames, Type[] columnTypes, List<object?[]> rows)
        {
            _columnNames = columnNames;
            _columnTypes = columnTypes;
            _rows = rows;
        }

        /// <summary>Advances to the next row; returns false when there are no more rows.</summary>
        public bool Read()
        {
            _currentRowIndex++;
            return _currentRowIndex < _rows.Count;
        }

        /// <inheritdoc />
        public int FieldCount => _columnNames.Length;

        /// <inheritdoc />
        public bool IsDBNull(int i)
        {
            object? value = _rows[_currentRowIndex][i];
            return value == null || value == DBNull.Value;
        }

        /// <inheritdoc />
        public object this[int i] => GetValue(i);

        /// <inheritdoc />
        public object this[string name] => GetValue(GetOrdinal(name));

        /// <inheritdoc />
        public object GetValue(int i) => _rows[_currentRowIndex][i]!;

        /// <inheritdoc />
        public int GetValues(object[] values)
        {
            Array.Copy(_rows[_currentRowIndex], values, FieldCount);
            return FieldCount;
        }

        /// <inheritdoc />
        public string GetName(int i) => _columnNames[i];

        /// <inheritdoc />
        public int GetOrdinal(string name)
        {
            for (int index = 0; index < _columnNames.Length; index++)
            {
                if (_columnNames[index].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return index;
                }
            }
            return -1;
        }

        /// <inheritdoc />
        public Type GetFieldType(int i) => _columnTypes[i];

        /// <inheritdoc />
        public void Close()
        {
            // No unmanaged resources to release.
        }

        /// <inheritdoc />
        public bool NextResult() => false;

        /// <inheritdoc />
        public DataTable? GetSchemaTable() => null;

        /// <inheritdoc />
        public int Depth => 0;

        /// <inheritdoc />
        public bool IsClosed => false;

        /// <inheritdoc />
        public int RecordsAffected => -1;

        /// <inheritdoc />
        public void Dispose() => Close();

        /// <inheritdoc />
        public bool GetBoolean(int i) => (bool)GetValue(i)!;

        /// <inheritdoc />
        public byte GetByte(int i) => (byte)GetValue(i)!;

        /// <inheritdoc />
        public char GetChar(int i) => (char)GetValue(i)!;

        /// <inheritdoc />
        public Guid GetGuid(int i) => (Guid)GetValue(i)!;

        /// <inheritdoc />
        public short GetInt16(int i) => (short)GetValue(i)!;

        /// <inheritdoc />
        public int GetInt32(int i) => (int)GetValue(i)!;

        /// <inheritdoc />
        public long GetInt64(int i) => (long)GetValue(i)!;

        /// <inheritdoc />
        public float GetFloat(int i) => (float)GetValue(i)!;

        /// <inheritdoc />
        public double GetDouble(int i) => (double)GetValue(i)!;

        /// <inheritdoc />
        public string GetString(int i) => (string)GetValue(i)!;

        /// <inheritdoc />
        public decimal GetDecimal(int i) => (decimal)GetValue(i)!;

        /// <inheritdoc />
        public DateTime GetDateTime(int i) => (DateTime)GetValue(i)!;

        /// <inheritdoc />
        public string GetDataTypeName(int i) => _columnTypes[i].Name;

        /// <summary>
        /// Reads a stream of bytes from the specified column offset into the buffer. When
        /// <paramref name="buffer"/> is null the total length of the field in bytes is returned.
        /// </summary>
        public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        {
            object? columnValue = _rows[_currentRowIndex][i];
            if (columnValue == null || columnValue == DBNull.Value)
            {
                return 0;
            }

            if (columnValue is not byte[] data)
            {
                throw new InvalidCastException($"Column {i} does not contain a byte[].");
            }

            if (buffer == null)
            {
                return data.Length;
            }

            if (fieldOffset >= data.Length)
            {
                return 0;
            }

            int available = data.Length - (int)fieldOffset;
            if (length > available)
            {
                length = available;
            }

            Array.Copy(data, (int)fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        /// <summary>
        /// Reads a stream of characters from the specified column offset into the buffer. When
        /// <paramref name="buffer"/> is null the total length of the field in chars is returned.
        /// </summary>
        public long GetChars(int i, long fieldOffset, char[]? buffer, int bufferoffset, int length)
        {
            object? columnValue = _rows[_currentRowIndex][i];
            if (columnValue == null || columnValue == DBNull.Value)
            {
                return 0;
            }

            if (columnValue is not string data)
            {
                throw new InvalidCastException($"Column {i} does not contain a string.");
            }

            if (buffer == null)
            {
                return data.Length;
            }

            if (fieldOffset >= data.Length)
            {
                return 0;
            }

            int available = data.Length - (int)fieldOffset;
            if (length > available)
            {
                length = available;
            }

            data.CopyTo((int)fieldOffset, buffer, bufferoffset, length);
            return length;
        }

        /// <inheritdoc />
        public IDataReader GetData(int i) => throw new NotImplementedException();
    }
}
