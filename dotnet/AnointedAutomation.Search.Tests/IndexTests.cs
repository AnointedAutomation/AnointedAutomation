// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The in-memory index: atomic snapshot swap, BooleanQuery clauses (term, phrase, fuzzy; must, should, must not),
// per field BM25, fast field filters and sorts.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnointedAutomation.Search.Analysis;
using AnointedAutomation.Search.Index;
using AnointedAutomation.Search.Query;
using AnointedAutomation.Search.Scoring;
using Xunit;

namespace AnointedAutomation.Search.Tests
{
    public class IndexTests
    {
        public sealed class Book
        {
            public string Title { get; init; }
            public string Body { get; init; }
            public decimal? Price { get; init; }
            public string Shelf { get; init; }
        }

        private static TextAnalyzer Analyzer() =>
            new TextAnalyzer(new StandardTokenizer(StandardTokenizer.DefaultMaxTokenLength),
                new ITokenFilter[] { new LowerCaseFilter(), new EnglishMinimalStemFilter() });

        private static TextFieldOptions Field(decimal boost) =>
            new TextFieldOptions { Boost = boost, Positions = true, K1 = Bm25.DefaultK1, B = Bm25.DefaultB };

        private static SearchIndex<Book> Index(params Book[] books)
        {
            SearchSchema<Book> schema = new SearchSchema<Book>(Analyzer());
            schema.AddText("title", b => b.Title, Field(2m));
            schema.AddText("body", b => b.Body, Field(1m));
            schema.AddDecimal("price", b => b.Price);
            schema.AddKeyword("shelf", b => b.Shelf is null ? null : new[] { b.Shelf });
            SearchIndex<Book> index = new SearchIndex<Book>(schema);
            index.Rebuild(books);
            return index;
        }

        private static readonly Book[] Books =
        {
            new Book { Title = "The Quick Brown Fox", Body = "A fox jumps over the lazy dog.", Price = 10m, Shelf = "animals" },
            new Book { Title = "Lazy Dogs", Body = "Dogs sleeping all day.", Price = 5m, Shelf = "animals" },
            new Book { Title = "Brown Bread", Body = "Baking with brown flour.", Price = 7m, Shelf = "food" },
            new Book { Title = "Quick Recipes", Body = "Fast food for busy days.", Price = null, Shelf = "food" },
        };

        private static string[] Titles(SearchResult<Book> r) => r.Hits.Select(h => h.Document.Title).ToArray();

        private static SearchResult<Book> Find(SearchIndex<Book> index, params QueryClause[] clauses)
        {
            SearchRequest<Book> request = new SearchRequest<Book>(10, 0);
            request.Clauses.AddRange(clauses);
            return index.Search(request);
        }

        [Fact]
        public void Search_BeforeRebuild_Throws()
        {
            SearchSchema<Book> schema = new SearchSchema<Book>(Analyzer());
            schema.AddText("title", b => b.Title, Field(1m));
            Assert.Throws<InvalidOperationException>(() => new SearchIndex<Book>(schema).Search(new SearchRequest<Book>(1, 0)));
        }

        [Fact]
        public void Term_EveryWordMustMatch_TitleBoostWins()
        {
            SearchIndex<Book> index = Index(Books);
            Assert.Equal(new[] { "Lazy Dogs", "The Quick Brown Fox" }, Titles(Find(index, new TermClause("lazy dog", Occur.Must, null))));
            Assert.Equal(new[] { "Brown Bread", "The Quick Brown Fox" }, Titles(Find(index, new TermClause("brown", Occur.Must, null))));
        }

        [Fact]
        public void Phrase_RequiresOrderWithinSlop()
        {
            SearchIndex<Book> index = Index(Books);
            Assert.Equal(new[] { "The Quick Brown Fox" }, Titles(Find(index, new PhraseClause("quick brown", 0, Occur.Must, null))));
            Assert.Empty(Find(index, new PhraseClause("brown quick", 0, Occur.Must, null)).Hits);
            Assert.Equal(new[] { "The Quick Brown Fox" }, Titles(Find(index, new PhraseClause("quick fox", 1, Occur.Must, null))));
        }

        [Fact]
        public void Fuzzy_MatchesWithinEditsAndPrefix()
        {
            SearchIndex<Book> index = Index(Books);
            Assert.Equal(new[] { "Brown Bread", "The Quick Brown Fox" }, Titles(Find(index, new FuzzyClause("brwn", 1, true, 1, Occur.Must, null))));
            Assert.Empty(Find(index, new FuzzyClause("crown", 1, true, 1, Occur.Must, null)).Hits);
        }

        [Fact]
        public void Boolean_ShouldMustNotAndFieldRestriction()
        {
            SearchIndex<Book> index = Index(Books);
            Assert.Equal(new[] { "Brown Bread", "Quick Recipes", "The Quick Brown Fox" },
                Titles(Find(index, new TermClause("bread", Occur.Should, null), new TermClause("quick", Occur.Should, null))).OrderBy(t => t).ToArray());
            Assert.Equal(new[] { "Quick Recipes" },
                Titles(Find(index, new TermClause("quick", Occur.Must, null), new TermClause("fox", Occur.MustNot, null))));
            Assert.Equal(new[] { "The Quick Brown Fox" }, Titles(Find(index, new TermClause("fox", Occur.Must, new[] { "body" }))));
            Assert.Empty(Find(index, new TermClause("baking", Occur.Must, new[] { "title" })).Hits);
        }

        [Fact]
        public void FiltersAndSort_PriceAndShelf()
        {
            SearchIndex<Book> index = Index(Books);
            SearchRequest<Book> request = new SearchRequest<Book>(10, 0);
            request.Filters.Add(FastFieldFilter.AnyKeyword("shelf", new[] { "animals", "food" }));
            request.Filters.Add(FastFieldFilter.Range("price", null, 8m));
            request.Sort.Add(new SortField("price", false));
            Assert.Equal(new[] { "Lazy Dogs", "Brown Bread" }, Titles(index.Search(request)));

            SearchRequest<Book> all = new SearchRequest<Book>(2, 1);
            all.Sort.Add(new SortField("price", true));
            SearchResult<Book> page = index.Search(all);
            Assert.Equal(4, page.TotalHits);
            Assert.Equal(new[] { "Brown Bread", "Lazy Dogs" }, Titles(page));
        }

        [Fact]
        public async Task Rebuild_WhileSearching_NeverFails()
        {
            SearchIndex<Book> index = Index(Books);
            Task searches = Task.Run(() =>
            {
                for (int i = 0; i < 300; i++)
                {
                    Assert.NotEmpty(Find(index, new TermClause("brown", Occur.Must, null)).Hits);
                }
            });
            for (int i = 0; i < 20; i++)
            {
                index.Rebuild(Books);
            }
            await searches;
            Assert.Equal(21, index.Snapshot.Generation);
        }

        [Fact]
        public void Schema_RejectsDuplicateFields()
        {
            SearchSchema<Book> schema = new SearchSchema<Book>(Analyzer());
            schema.AddText("title", b => b.Title, Field(1m));
            Assert.Throws<ArgumentException>(() => schema.AddDecimal("title", b => b.Price));
            Assert.Throws<ArgumentOutOfRangeException>(() => schema.AddText("x", b => b.Body, new TextFieldOptions { Boost = 0m, Positions = false, K1 = 1.2m, B = 0.75m }));
        }
    }
}
