// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/schema/schema.rs (SchemaBuilder: fixed fields declared up front, text vs fast
// fields) and src/schema/numeric_options.rs. Translated from Rust to C#: fields read from the caller's own
// document type through getters instead of a dynamic Document.

using System;
using System.Collections.Generic;
using AnointedAutomation.Search.Analysis;

namespace AnointedAutomation.Search.Index
{
    /// <summary>The kind of a fast (columnar) field. Zero is reserved.</summary>
    public enum FastFieldKind
    {
        Unknown = 0,
        Integer = 1,
        Decimal = 2,
        Keyword = 3,
    }

    /// <summary>A text field: its name, getter (one or many values) and options.</summary>
    public sealed class TextFieldDefinition<TDocument>
    {
        internal TextFieldDefinition(string name, Func<TDocument, IEnumerable<string>> values, TextFieldOptions options)
        {
            Name = name;
            Values = values;
            Options = options;
        }

        public string Name { get; }
        public Func<TDocument, IEnumerable<string>> Values { get; }
        public TextFieldOptions Options { get; }
    }

    /// <summary>A fast field: one value per document read by filters and sorts.</summary>
    public sealed class FastFieldDefinition<TDocument>
    {
        internal FastFieldDefinition(string name, FastFieldKind kind, Func<TDocument, long?> integer, Func<TDocument, decimal?> decimalValue, Func<TDocument, IEnumerable<string>> keywords)
        {
            Name = name;
            Kind = kind;
            Integer = integer;
            Decimal = decimalValue;
            Keywords = keywords;
        }

        public string Name { get; }
        public FastFieldKind Kind { get; }
        internal Func<TDocument, long?> Integer { get; }
        internal Func<TDocument, decimal?> Decimal { get; }
        internal Func<TDocument, IEnumerable<string>> Keywords { get; }
    }

    /// <summary>The fixed field layout of an index over <typeparamref name="TDocument"/>.</summary>
    public sealed class SearchSchema<TDocument>
    {
        /// <summary>Gap between the values of a multi valued text field, so a phrase never spans two values (Lucene positionIncrementGap).</summary>
        public const int MultiValuePositionGap = 100;

        private readonly List<TextFieldDefinition<TDocument>> _text = new List<TextFieldDefinition<TDocument>>();
        private readonly List<FastFieldDefinition<TDocument>> _fast = new List<FastFieldDefinition<TDocument>>();
        private readonly HashSet<string> _names = new HashSet<string>(StringComparer.Ordinal);

        /// <param name="analyzer">Analyzer for indexed text and query text (stemming, if any, is a filter in it).</param>
        public SearchSchema(TextAnalyzer analyzer)
        {
            if (analyzer is null)
            {
                throw new ArgumentNullException(nameof(analyzer));
            }
            Analyzer = analyzer;
        }

        public TextAnalyzer Analyzer { get; }


        public IReadOnlyList<TextFieldDefinition<TDocument>> TextFields => _text;

        public IReadOnlyList<FastFieldDefinition<TDocument>> FastFields => _fast;

        public SearchSchema<TDocument> AddText(string name, Func<TDocument, string> value, TextFieldOptions options)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            return AddTextList(name, doc => new[] { value(doc) }, options);
        }

        public SearchSchema<TDocument> AddTextList(string name, Func<TDocument, IEnumerable<string>> values, TextFieldOptions options)
        {
            CheckName(name);
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.Validate(name);
            _text.Add(new TextFieldDefinition<TDocument>(name, values, options));
            return this;
        }

        public SearchSchema<TDocument> AddInteger(string name, Func<TDocument, long?> value)
        {
            CheckName(name);
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _fast.Add(new FastFieldDefinition<TDocument>(name, FastFieldKind.Integer, value, null, null));
            return this;
        }

        public SearchSchema<TDocument> AddDecimal(string name, Func<TDocument, decimal?> value)
        {
            CheckName(name);
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            _fast.Add(new FastFieldDefinition<TDocument>(name, FastFieldKind.Decimal, null, value, null));
            return this;
        }

        /// <summary>Exact match keywords (store ids, lanes, tags) compared ordinally, unanalyzed.</summary>
        public SearchSchema<TDocument> AddKeyword(string name, Func<TDocument, IEnumerable<string>> values)
        {
            CheckName(name);
            if (values is null)
            {
                throw new ArgumentNullException(nameof(values));
            }
            _fast.Add(new FastFieldDefinition<TDocument>(name, FastFieldKind.Keyword, null, null, values));
            return this;
        }

        internal int TextFieldIndex(string name)
        {
            for (int i = 0; i < _text.Count; i++)
            {
                if (string.Equals(_text[i].Name, name, StringComparison.Ordinal))
                {
                    return i;
                }
            }
            throw new ArgumentException("Unknown text field '" + name + "'.", nameof(name));
        }

        private void CheckName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Field name is required.", nameof(name));
            }
            if (!_names.Add(name))
            {
                throw new ArgumentException("Field '" + name + "' is already defined.", nameof(name));
            }
        }
    }
}
