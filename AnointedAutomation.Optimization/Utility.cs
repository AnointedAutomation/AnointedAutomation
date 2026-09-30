// Copyright 2024 Anointed Automation, LLC All rights reserved.
// Originally created by Alexander Fields. Ported and modernized for the AnointedAutomation monorepo.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AnointedAutomation.Optimization
{
    /// <summary>
    /// Reusable helpers for DataTable/DataReader conversion, CSV generation, string utilities,
    /// chunking, and small array helpers.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Converts a <see cref="DataTable"/> to an HTML table.
        /// </summary>
        /// <param name="dataTable">The table to render.</param>
        /// <param name="cellSpacing">HTML cellspacing attribute.</param>
        /// <param name="cellPadding">HTML cellpadding attribute.</param>
        /// <param name="border">HTML border attribute.</param>
        /// <param name="backgroundColor">If null or empty defaults to white.</param>
        /// <returns>An HTML table as a string.</returns>
        public static string ConvertDataTableToHTML(DataTable dataTable, int cellSpacing, int cellPadding, int border, string? backgroundColor)
        {
            if (string.IsNullOrWhiteSpace(backgroundColor))
            {
                backgroundColor = "#ffffff";
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"<table cellspacing={cellSpacing} cellpadding={cellPadding} border={border} bgcolor={backgroundColor}>");

            sb.AppendLine("<tr>");
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.AppendLine($"<td>{dataTable.Columns[i].ColumnName}</td>");
            }
            sb.AppendLine("</tr>");

            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                sb.AppendLine("<tr>");
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    sb.AppendLine($"<td>{dataTable.Rows[i][j]}</td>");
                }
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</table>");
            return sb.ToString();
        }

        /// <summary>Generic converter for a <see cref="DataTable"/> to a list of objects.</summary>
        public static List<T> ConvertDataTableToList<T>(DataTable dataTable)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dataTable.Rows)
            {
                data.Add(GetObjectFromDataRow<T>(row));
            }
            return data;
        }

        /// <summary>Generic converter for an <see cref="IDataReader"/> to a list of objects.</summary>
        public static List<T> ConvertDataReaderToList<T>(IDataReader reader)
        {
            List<T> data = new List<T>();
            while (reader.Read())
            {
                data.Add(GetObjectFromDataReader<T>(reader));
            }
            return data;
        }

        /// <summary>
        /// Parallel converter for a <see cref="DataTable"/> to a list of objects. Faster, but may
        /// construct the list in a different order.
        /// </summary>
        /// <param name="dataTable">The table to convert.</param>
        /// <param name="rowsAtOnce">Maximum number of rows to convert concurrently.</param>
        public static List<T> ConvertDataTableToListFast<T>(DataTable dataTable, int rowsAtOnce)
        {
            List<T> data = new List<T>();
            List<Task<T>> pool = new List<Task<T>>();

            foreach (DataRow row in dataTable.Rows)
            {
                while (pool.Count >= rowsAtOnce)
                {
                    Task.WaitAll(pool.ToArray());
                    foreach (Task<T> tempTask in pool)
                    {
                        data.Add(tempTask.Result);
                    }
                    pool.RemoveAll(tempTask => tempTask.IsCompleted);
                }

                pool.Add(Task.Run(() => GetObjectFromDataRow<T>(row)));
            }

            Task.WaitAll(pool.ToArray());
            foreach (Task<T> task in pool)
            {
                data.Add(task.Result);
            }

            return data;
        }

        /// <summary>Converts a list of objects to a <see cref="DataTable"/>.</summary>
        public static DataTable ConvertListToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object?[] values = new object?[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        /// <summary>Writes a delimited file from a list of objects, including a header row.</summary>
        public static void CreateCsvFromList<T>(IEnumerable<T> data, string csvPath, string delimiter)
        {
            StringBuilder csv = new StringBuilder();
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            csv.AppendLine(string.Join(delimiter, props.Cast<PropertyDescriptor>().Select(p => p.Name)));
            foreach (T item in data)
            {
                csv.AppendLine(string.Join(delimiter, props.Cast<PropertyDescriptor>().Select(p => $"\"{p.GetValue(item)}\"")));
            }

            File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>Streams a delimited file from a list of objects (low heap), including a header row.</summary>
        public static void CreateCsvFromListNoHeap<T>(IEnumerable<T> data, string csvPath, char delimiter)
        {
            using StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            writer.WriteLine(string.Join(delimiter, props.Cast<PropertyDescriptor>().Select(p => p.Name)));
            foreach (T item in data)
            {
                writer.WriteLine(string.Join(delimiter, props.Cast<PropertyDescriptor>().Select(p => $"\"{p.GetValue(item)}\"")));
            }
        }

        /// <summary>Streams a delimited file from a list of objects (low heap) with no header row.</summary>
        public static void CreateCsvFromListNoHeaders<T>(IEnumerable<T> data, string csvPath, string delimiter)
        {
            using StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8);
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));

            foreach (T item in data)
            {
                writer.WriteLine(string.Join(delimiter, props.Cast<PropertyDescriptor>().Select(p => $"\"{p.GetValue(item)}\"")));
            }
        }

        /// <summary>
        /// Converts a <see cref="DataTable"/> into delimited lines. The first line is the header.
        /// </summary>
        public static List<string> ConvertDataTableToListString(DataTable dataTable, string delimiter)
        {
            List<string> lines = new List<string>();

            string[] columnNames = dataTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            lines.Add(string.Join(delimiter, columnNames.Select(name => $"{name}")));

            IEnumerable<string> valueLines = dataTable.AsEnumerable()
                .Select(row => string.Join(delimiter, row.ItemArray.Select(val => $"\"{val}\"")));

            lines.AddRange(valueLines);
            return lines;
        }

        /// <summary>Writes a delimited file from a <see cref="DataTable"/>, including a header row.</summary>
        public static void CreateCsvFromDataTable(DataTable dataTable, string csvPath, string delimiter)
        {
            StringBuilder csv = new StringBuilder();

            string[] columnNames = dataTable.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            csv.AppendLine(string.Join(delimiter, columnNames.Select(name => $"{name}")));

            object?[][] rowValues = dataTable.AsEnumerable().Select(row => row.ItemArray).ToArray();
            foreach (object?[] values in rowValues)
            {
                csv.AppendLine(string.Join(delimiter, values.Select(val => $"\"{val}\"")));
            }

            File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>Writes a delimited file from a <see cref="DataTable"/> (low heap), including a header row.</summary>
        public static void CreateCsvFromDataTableNoHeap(DataTable dataTable, string csvPath, char delimiter)
        {
            StringBuilder csv = new StringBuilder();

            int columnCount = dataTable.Columns.Count;
            for (int i = 0; i < columnCount; i++)
            {
                csv.Append(dataTable.Columns[i].ColumnName);
                if (i < columnCount - 1)
                {
                    csv.Append(delimiter);
                }
            }
            csv.AppendLine();

            int rowCount = dataTable.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                DataRow row = dataTable.Rows[i];
                for (int j = 0; j < columnCount; j++)
                {
                    csv.Append('"');
                    csv.Append(row[j]);
                    csv.Append('"');
                    if (j < columnCount - 1)
                    {
                        csv.Append(delimiter);
                    }
                }
                csv.AppendLine();
            }

            File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>Writes a delimited file from a <see cref="DataTable"/> with no header row.</summary>
        public static void CreateCsvFromDataTableNoHeaders(DataTable dataTable, string csvPath, string delimiter)
        {
            StringBuilder csv = new StringBuilder();

            object?[][] rowValues = dataTable.AsEnumerable().Select(row => row.ItemArray).ToArray();
            foreach (object?[] values in rowValues)
            {
                csv.AppendLine(string.Join(delimiter, values.Select(val => $"\"{val}\"")));
            }

            File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>Reads a delimited file from a path into a <see cref="DataTable"/>.</summary>
        public static DataTable ConvertCSVtoDataTable(string strFilePath, char delimiter)
        {
            using StreamReader streamReader = new StreamReader(strFilePath);
            return ReadCsv(streamReader, delimiter);
        }

        /// <summary>Reads delimited data from a stream into a <see cref="DataTable"/>.</summary>
        public static DataTable ConvertCSVtoDataTable(Stream csvStream, char delimiter)
        {
            using StreamReader streamReader = new StreamReader(csvStream);
            return ReadCsv(streamReader, delimiter);
        }

        private static DataTable ReadCsv(StreamReader streamReader, char delimiter)
        {
            DataTable dataTable = new DataTable();

            string? headerLine = streamReader.ReadLine();
            if (headerLine == null)
            {
                return dataTable;
            }

            string[] headers = headerLine.Split(delimiter);
            foreach (string header in headers)
            {
                string newHeader = header;
                if (newHeader.StartsWith("\"") && newHeader.EndsWith("\""))
                {
                    newHeader = newHeader.Substring(1, newHeader.Length - 2);
                }
                newHeader = newHeader.Replace(" ", string.Empty);
                dataTable.Columns.Add(newHeader);
            }

            while (!streamReader.EndOfStream)
            {
                string? line = streamReader.ReadLine();
                if (line == null)
                {
                    break;
                }

                string[] rows = Regex.Split(line, Regex.Escape(delimiter.ToString()) + "(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dataTable.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    string field = rows[i].Trim();
                    if (field.StartsWith("\"") && field.EndsWith("\""))
                    {
                        field = field.Substring(1, field.Length - 2);
                    }
                    dr[i] = field;
                }
                dataTable.Rows.Add(dr);
            }

            return dataTable;
        }

        /// <summary>Writes a delimited file from a <see cref="DbDataReader"/>, including a header row.</summary>
        public static void CreateCsvFromDataReader(DbDataReader reader, string csvPath, string delimiter) =>
            CreateCsvFromDataReader((IDataReader)reader, csvPath, delimiter);

        /// <summary>Writes a delimited file from an <see cref="IDataReader"/>, including a header row.</summary>
        public static void CreateCsvFromDataReader(IDataReader reader, string csvPath, string delimiter)
        {
            StringBuilder csv = new StringBuilder();
            int fieldCount = reader.FieldCount;

            for (int i = 0; i < fieldCount; i++)
            {
                csv.Append(reader.GetName(i));
                if (i < fieldCount - 1)
                {
                    csv.Append(delimiter);
                }
            }
            csv.AppendLine();

            while (reader.Read())
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    csv.Append('"');
                    csv.Append(reader[i]);
                    csv.Append('"');
                    if (i < fieldCount - 1)
                    {
                        csv.Append(delimiter);
                    }
                }
                csv.AppendLine();
            }

            File.WriteAllText(csvPath, csv.ToString());
        }

        /// <summary>Streams a delimited file from a <see cref="DbDataReader"/> (low heap), including a header row.</summary>
        public static void CreateCsvFromDataReaderNoHeap(DbDataReader reader, string csvPath, char delimiter) =>
            CreateCsvFromDataReaderNoHeap((IDataReader)reader, csvPath, delimiter);

        /// <summary>Streams a delimited file from an <see cref="IDataReader"/> (low heap), including a header row.</summary>
        public static void CreateCsvFromDataReaderNoHeap(IDataReader reader, string csvPath, char delimiter)
        {
            using StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8);
            int fieldCount = reader.FieldCount;

            for (int i = 0; i < fieldCount; i++)
            {
                writer.Write(reader.GetName(i));
                if (i < fieldCount - 1)
                {
                    writer.Write(delimiter);
                }
            }
            writer.WriteLine();

            while (reader.Read())
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    writer.Write('"');
                    writer.Write(reader[i]);
                    writer.Write('"');
                    if (i < fieldCount - 1)
                    {
                        writer.Write(delimiter);
                    }
                }
                writer.WriteLine();
            }
        }

        /// <summary>Streams a delimited file from a <see cref="DbDataReader"/> with no header row.</summary>
        public static void CreateCsvFromDataReaderNoHeaders(DbDataReader reader, string csvPath, string delimiter) =>
            CreateCsvFromDataReaderNoHeaders((IDataReader)reader, csvPath, delimiter);

        /// <summary>Streams a delimited file from an <see cref="IDataReader"/> with no header row.</summary>
        public static void CreateCsvFromDataReaderNoHeaders(IDataReader reader, string csvPath, string delimiter)
        {
            using StreamWriter writer = new StreamWriter(csvPath, false, Encoding.UTF8);
            int fieldCount = reader.FieldCount;

            while (reader.Read())
            {
                for (int i = 0; i < fieldCount; i++)
                {
                    writer.Write('"');
                    writer.Write(reader[i]);
                    writer.Write('"');
                    if (i < fieldCount - 1)
                    {
                        writer.Write(delimiter);
                    }
                }
                writer.WriteLine();
            }
        }

        /// <summary>Decodes a base64 string using ASCII.</summary>
        public static string DecodeString(string encodedString)
        {
            byte[] bytes = Convert.FromBase64String(encodedString);
            return Encoding.ASCII.GetString(bytes);
        }

        /// <summary>Encodes a string into base64 using ASCII.</summary>
        public static string EncodeString(string stringToEncode)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(stringToEncode);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Generates a random alphanumeric (base 62) string. When <paramref name="length"/> is less
        /// than or equal to zero a random length between 7 and 255 is used.
        /// </summary>
        public static string GenerateString(int length)
        {
            const string pool =
                "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                + "abcdefghijklmnopqrztuvwxyz"
                + "0123456789";

            Random random = new Random();
            if (length <= 0)
            {
                length = random.Next(7, 255);
            }

            IEnumerable<char> chars = Enumerable.Range(0, length)
                .Select(_ => pool[random.Next(0, pool.Length)]);

            return new string(chars.ToArray());
        }

        /// <summary>Maps a <see cref="DataRow"/> onto a new instance of <typeparamref name="T"/> by matching column names to writable properties.</summary>
        public static T GetObjectFromDataRow<T>(DataRow dataRow)
        {
            Type type = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                foreach (PropertyInfo prop in type.GetProperties())
                {
                    if (!prop.Name.Equals(column.ColumnName, StringComparison.Ordinal))
                    {
                        continue;
                    }

                    AssignProperty(obj, prop, dataRow[column.ColumnName]);
                }
            }
            return obj;
        }

        /// <summary>Maps the current record of an <see cref="IDataReader"/> onto a new instance of <typeparamref name="T"/>.</summary>
        public static T GetObjectFromDataReader<T>(IDataReader reader)
        {
            Type type = typeof(T);
            T obj = Activator.CreateInstance<T>();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                PropertyInfo? prop = type.GetProperty(columnName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null || !prop.CanWrite)
                {
                    continue;
                }

                AssignProperty(obj, prop, reader.GetValue(i));
            }
            return obj;
        }

        private static void AssignProperty<T>(T obj, PropertyInfo prop, object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                prop.SetValue(obj, null);
                return;
            }

            string? asString = value.ToString();

            if (prop.PropertyType == typeof(DateOnly))
            {
                prop.SetValue(obj, DateOnly.TryParse(asString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly parsedDateOnly) ? parsedDateOnly : default(DateOnly));
                return;
            }

            if (prop.PropertyType == typeof(DateTime))
            {
                prop.SetValue(obj, DateTime.TryParse(asString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime) ? parsedDateTime : default(DateTime));
                return;
            }

            try
            {
                prop.SetValue(obj, Convert.ChangeType(value, prop.PropertyType, CultureInfo.InvariantCulture));
            }
            catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or ArgumentException)
            {
                prop.SetValue(obj, null);
            }
        }

        /// <summary>Formats a <see cref="TimeSpan"/> as <c>0h:0m:0s</c>.</summary>
        public static string GetReadableTimeString(TimeSpan timeSpan) =>
            timeSpan.Hours + "h:" + timeSpan.Minutes + "m:" + timeSpan.Seconds + "s";

        /// <summary>True when <paramref name="compare"/> contains at least one of <paramref name="strings"/>.</summary>
        public static bool IfStringContainsOneOf(string compare, string[] strings) =>
            strings.Length != 0 && strings.Any(compare.Contains);

        /// <summary>True when <paramref name="compare"/> contains at least one of <paramref name="strings"/>.</summary>
        public static bool IfStringContainsOneOf(string compare, List<string> strings) =>
            strings.Count != 0 && strings.Any(compare.Contains);

        /// <summary>True when <paramref name="compare"/> contains at least one of the delimited substrings.</summary>
        public static bool IfStringContainsOneOf(string compare, string stringsDelimited, string delimiter)
        {
            if (string.IsNullOrEmpty(stringsDelimited))
            {
                return false;
            }
            return IfStringContainsOneOf(compare, stringsDelimited.Split(delimiter));
        }

        /// <summary>True when <paramref name="compare"/> contains at least one of the delimited substrings.</summary>
        public static bool IfStringContainsOneOf(string compare, string stringsDelimited, char delimiter)
        {
            if (string.IsNullOrEmpty(stringsDelimited))
            {
                return false;
            }
            return stringsDelimited.Split(delimiter).Any(compare.Contains);
        }

        /// <summary>Determines whether a string is a valid base64 string.</summary>
        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new byte[base64.Length];
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        /// <summary>Converts a two dimensional array into a jagged array.</summary>
        public static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
        {
            int rows = twoDimensionalArray.GetLength(0);
            int columns = twoDimensionalArray.GetLength(1);
            T[][] jaggedArray = new T[rows][];

            for (int i = 0; i < rows; i++)
            {
                jaggedArray[i] = new T[columns];
                for (int j = 0; j < columns; j++)
                {
                    jaggedArray[i][j] = twoDimensionalArray[i, j];
                }
            }

            return jaggedArray;
        }

        /// <summary>Splits an enumerable into chunks of at most <paramref name="chunkSize"/> items.</summary>
        public static IEnumerable<IEnumerable<T>> ToChunks<T>(this IEnumerable<T> enumerable, int chunkSize)
        {
            int itemsReturned = 0;
            List<T> list = enumerable.ToList();
            int count = list.Count;
            while (itemsReturned < count)
            {
                int currentChunkSize = Math.Min(chunkSize, count - itemsReturned);
                yield return list.GetRange(itemsReturned, currentChunkSize);
                itemsReturned += currentChunkSize;
            }
        }

        /// <summary>Recursively deletes a directory and its contents.</summary>
        public static void RecursiveDelete(DirectoryInfo baseDir)
        {
            if (!baseDir.Exists)
            {
                return;
            }

            Parallel.ForEach(baseDir.EnumerateDirectories(), dir => RecursiveDelete(dir));

            FileInfo[] files = baseDir.GetFiles();
            Parallel.ForEach(files, file =>
            {
                file.IsReadOnly = false;
                file.Delete();
            });

            baseDir.Delete();
        }

        /// <summary>Returns the substrings that <paramref name="stringToCompare"/> contains.</summary>
        public static IEnumerable<string> WhereContains(string stringToCompare, List<string>? subStrings, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(stringToCompare) || subStrings == null)
            {
                return Enumerable.Empty<string>();
            }

            return ignoreCase
                ? subStrings.Where(substring => stringToCompare.Contains(substring, StringComparison.CurrentCultureIgnoreCase))
                : subStrings.Where(stringToCompare.Contains);
        }

        /// <summary>Returns the substrings that <paramref name="stringToCompare"/> contains.</summary>
        public static IEnumerable<string> WhereContains(string stringToCompare, string[]? subStrings, bool ignoreCase)
        {
            if (string.IsNullOrEmpty(stringToCompare) || subStrings == null)
            {
                return Enumerable.Empty<string>();
            }

            return ignoreCase
                ? subStrings.Where(substring => stringToCompare.Contains(substring, StringComparison.CurrentCultureIgnoreCase))
                : subStrings.Where(stringToCompare.Contains);
        }
    }
}
