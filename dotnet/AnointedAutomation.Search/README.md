# AnointedAutomation.Search

Jesus is King ✝️

In-process full-text search for .NET with no dependencies: **Apache Lucene (Java) and tantivy (Rust)
translated to C#**. Every source file names the Lucene and/or tantivy file(s) it was ported from. Nothing
else is in here: no product-specific ranking rules, just the search building blocks. See
[NOTICE.md](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/dotnet/AnointedAutomation.Search/NOTICE.md)
for the attribution and the Lucene, KStem and tantivy license texts.

## Installation

```bash
dotnet add package AnointedAutomation.Search
```

- Target framework: `net10.0`
- Dependencies: none

## What is inside

| Piece | Ported from |
|---|---|
| `StandardTokenizer` (UAX#29 word breaks) | Lucene StandardTokenizer, tantivy simple_tokenizer |
| `LowerCaseFilter`, `AsciiFoldingFilter`, `LengthFilter`, `DuplicateTermFilter` | Lucene, tantivy |
| `WordDelimiterGraphFilter` + `WordDelimiterOptions` | Lucene WordDelimiterGraphFilter |
| `KStemFilter` / `KStemmer` (full Krovetz stemmer with its dictionary) | Lucene KStemmer |
| `EnglishMinimalStemmer` / `EnglishMinimalStemFilter` (Harman S-stemmer) | Lucene EnglishMinimalStemmer |
| `ShingleFilter`, `SynonymGraphFilter` + `SynonymMap`, `EdgeNGramTokenFilter` | Lucene |
| `TextAnalyzer` (a tokenizer plus a filter chain) | Lucene Analyzer, tantivy TextAnalyzer |
| `InvertedIndex` (per field postings: doc, term frequency, positions; field lengths; term dictionary) | tantivy postings and fieldnorm, Lucene index |
| `FastFieldColumn`, `FastFieldFilter` (range, keyword), `SortField` | tantivy columnar and range query |
| `SearchIndex<T>`: immutable snapshot swapped atomically on `Rebuild` | tantivy Searcher/IndexReader, Lucene SearcherManager |
| `TermClause`, `PhraseClause` (slop), `FuzzyClause` with `Occur.Must` / `Should` / `MustNot` | Lucene BooleanQuery, TermQuery, PhraseQuery, FuzzyQuery |
| `Bm25` (k1 1.2, b 0.75, per field), `TopK<T>`, `PositionIntersection` | tantivy bm25.rs, Lucene BM25Similarity |
| `LevenshteinAutomatonBuilder`, `FuzzyTermEnumerator` (up to 2 edits, transpositions, prefix length) | tantivy levenshtein_automata, Lucene LevenshteinAutomata / FuzzyTermsEnum |

## Quick start

```csharp
using AnointedAutomation.Search;
using AnointedAutomation.Search.Analysis;
using AnointedAutomation.Search.Index;
using AnointedAutomation.Search.Query;
using AnointedAutomation.Search.Scoring;

public sealed class Book
{
    public string Title { get; init; }
    public string Body { get; init; }
    public decimal? Price { get; init; }
}

TextAnalyzer analyzer = new TextAnalyzer(
    new StandardTokenizer(StandardTokenizer.DefaultMaxTokenLength),
    new ITokenFilter[] { new LowerCaseFilter(), new AsciiFoldingFilter(preserveOriginal: false), new KStemFilter() });

SearchSchema<Book> schema = new SearchSchema<Book>(analyzer);
schema.AddText("title", b => b.Title, new TextFieldOptions { Boost = 2m, Positions = true, K1 = Bm25.DefaultK1, B = Bm25.DefaultB });
schema.AddText("body", b => b.Body, new TextFieldOptions { Boost = 1m, Positions = true, K1 = Bm25.DefaultK1, B = Bm25.DefaultB });
schema.AddDecimal("price", b => b.Price);

SearchIndex<Book> index = new SearchIndex<Book>(schema);
index.Rebuild(books); // call again whenever the documents change; searches never block

SearchRequest<Book> request = new SearchRequest<Book>(limit: 20, offset: 0);
request.Clauses.Add(new TermClause("quick brown", Occur.Must, null));
request.Clauses.Add(new FuzzyClause("foxx", maxEdits: 1, transpositions: true, prefixLength: 1, Occur.Should, null));
request.Filters.Add(FastFieldFilter.Range("price", null, 50m));
SearchResult<Book> result = index.Search(request);
```

## How a query is scored

- `TermClause`: every analyzed word must match; each word scores BM25 in its best field times the field
  boost (Lucene DisjunctionMaxQuery, tie breaker 0); the clause score is the sum over its words.
- `PhraseClause`: the words in order within the slop, in one positions field.
- `FuzzyClause`: every dictionary term within the edit budget, boosted `1 - edits / shorter length`
  (Lucene FuzzyTermsEnum); a document keeps its best expansion.
- `Occur.Must` clauses are required and add up, `Should` clauses add where they match (at least one is
  required when there is no `Must`), `MustNot` excludes. Filters exclude without scoring.
- Results order by score, or by `Sort` fields then score; ties fall to document order.

## Rules this code follows

- `decimal` for all scoring math; `double` appears only where .NET has no decimal logarithm.
- No fallback defaults: options are `required`, a missing snapshot throws, invalid input throws.
- Enums reserve 0 for `Unknown`.

## License

Apache License 2.0 for the portions translated from Apache Lucene, MIT for the portions translated from
tantivy; see NOTICE.md.
