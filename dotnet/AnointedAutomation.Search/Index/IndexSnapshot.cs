// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from tantivy src/core/searcher.rs (Searcher: an immutable snapshot of segment readers) and
// src/core/segment_reader.rs, with Apache Lucene lucene/core/src/java/org/apache/lucene/index/DirectoryReader.java
// (point in time reader). Translated from Rust and Java to C#: one in-memory segment holding the documents,
// the inverted index and the fast field columns.

using System;
using System.Collections.Generic;
using AnointedAutomation.Search.Analysis;

namespace AnointedAutomation.Search.Index
{
    /// <summary>A point in time, read only view of an indexed document list. Safe for concurrent searches.</summary>
    public sealed class IndexSnapshot<TDocument>
    {
        private readonly Dictionary<string, FastFieldColumn> _columns;

        internal IndexSnapshot(IReadOnlyList<TDocument> documents, InvertedIndex index, Dictionary<string, FastFieldColumn> columns, long generation)
        {
            Documents = documents;
            Index = index;
            _columns = columns;
            Generation = generation;
        }

        public IReadOnlyList<TDocument> Documents { get; }

        public InvertedIndex Index { get; }

        /// <summary>Increases by one on every rebuild.</summary>
        public long Generation { get; }

        public FastFieldColumn Column(string name)
        {
            FastFieldColumn column;
            if (name is null || !_columns.TryGetValue(name, out column))
            {
                throw new ArgumentException("Unknown fast field '" + name + "'.", nameof(name));
            }
            return column;
        }

        internal static IndexSnapshot<TDocument> Build(SearchSchema<TDocument> schema, IReadOnlyList<TextFieldDefinition<TDocument>> textFields, IReadOnlyList<FastFieldDefinition<TDocument>> fastFields, IReadOnlyList<TDocument> documents, long generation)
        {
            TDocument[] docs = new TDocument[documents.Count];
            for (int i = 0; i < documents.Count; i++)
            {
                if (documents[i] is null)
                {
                    throw new ArgumentException("Document " + i + " is null.", nameof(documents));
                }
                docs[i] = documents[i];
            }
            List<TextFieldOptions> options = new List<TextFieldOptions>(textFields.Count);
            List<string> names = new List<string>(textFields.Count);
            foreach (TextFieldDefinition<TDocument> f in textFields)
            {
                options.Add(f.Options);
                names.Add(f.Name);
            }
            List<List<string>[]> texts = new List<List<string>[]>(docs.Length);
            foreach (TDocument doc in docs)
            {
                List<string>[] perField = new List<string>[textFields.Count];
                for (int f = 0; f < textFields.Count; f++)
                {
                    List<string> values = new List<string>();
                    IEnumerable<string> raw = textFields[f].Values(doc);
                    if (raw is not null)
                    {
                        foreach (string v in raw)
                        {
                            if (!string.IsNullOrWhiteSpace(v))
                            {
                                values.Add(v);
                            }
                        }
                    }
                    perField[f] = values;
                }
                texts.Add(perField);
            }
            InvertedIndex index = new InvertedIndex(options, names, texts, schema.Analyzer);
            Dictionary<string, FastFieldColumn> columns = new Dictionary<string, FastFieldColumn>(StringComparer.Ordinal);
            foreach (FastFieldDefinition<TDocument> f in fastFields)
            {
                columns[f.Name] = FastFieldColumn.Build(f, docs);
            }
            return new IndexSnapshot<TDocument>(docs, index, columns, generation);
        }
    }
}
