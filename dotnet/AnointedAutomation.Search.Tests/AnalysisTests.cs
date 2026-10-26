// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Several cases are taken from Apache Lucene's own tests (TestASCIIFoldingFilter, TestWordDelimiterGraphFilter,
// TestKStemmer with kstemTestData.zip, TestShingleFilter, TestEdgeNGramTokenFilter), translated to C#.

using System.Collections.Generic;
using System.Linq;
using AnointedAutomation.Search.Analysis;
using Xunit;

namespace AnointedAutomation.Search.Tests
{
    public class AnalysisTests
    {
        private static List<Token> Tokens(params string[] terms)
        {
            List<Token> list = new List<Token>();
            int offset = 0;
            for (int i = 0; i < terms.Length; i++)
            {
                list.Add(new Token(terms[i], i, 1, offset, offset + terms[i].Length, TokenType.Word));
                offset += terms[i].Length + 1;
            }
            return list;
        }

        private static string[] Terms(List<Token> tokens) => tokens.Select(t => t.Term).ToArray();

        [Fact]
        public void StandardTokenizer_SplitsOnPunctuationAndKeepsJoiners()
        {
            StandardTokenizer tokenizer = new StandardTokenizer(StandardTokenizer.DefaultMaxTokenLength);
            List<Token> tokens = tokenizer.Tokenize("Don't pay $1,000.50 for U.S.A cover-up, snake_case!");
            Assert.Equal(new[] { "Don't", "pay", "1,000.50", "for", "U.S.A", "cover", "up", "snake_case" }, Terms(tokens));
            Assert.Equal(Enumerable.Range(0, 8).ToArray(), tokens.Select(t => t.Position).ToArray());
            Assert.Equal(TokenType.Number, tokens[2].Type);
        }


        [Fact]
        public void StandardTokenizer_SplitsLongTokensAndIdeographs()
        {
            StandardTokenizer tokenizer = new StandardTokenizer(4);
            Assert.Equal(new[] { "abcd", "efgh", "ij" }, Terms(tokenizer.Tokenize("abcdefghij")));
            StandardTokenizer cjk = new StandardTokenizer(StandardTokenizer.DefaultMaxTokenLength);
            Assert.Equal(new[] { "寿", "司" }, Terms(cjk.Tokenize("寿司")));
        }

        [Fact]
        public void LowerCaseFilter_Lowercases()
        {
            Assert.Equal(new[] { "hoodie", "xl" }, Terms(new LowerCaseFilter().Apply(Tokens("HOODIE", "Xl"))));
        }

        [Theory]
        [InlineData("clés", "cles")]
        [InlineData("À", "A")]
        [InlineData("Æ", "AE")]
        [InlineData("Ĳ", "IJ")]
        [InlineData("Þ", "TH")]
        [InlineData("ß", "ss")]
        [InlineData("ﬁ", "fi")]
        [InlineData("Œ", "OE")]
        [InlineData("CHAÎNE", "CHAINE")]
        [InlineData("plain", "plain")]
        public void AsciiFolding_LuceneLatin1Cases(string input, string expected)
        {
            Assert.Equal(expected, AsciiFoldingFilter.Fold(input));
        }

        [Fact]
        public void AsciiFolding_PreserveOriginalEmitsBoth()
        {
            List<Token> output = new AsciiFoldingFilter(preserveOriginal: true).Apply(Tokens("café"));
            Assert.Equal(new[] { "cafe", "café" }, Terms(output));
            Assert.All(output, t => Assert.Equal(0, t.Position));
        }

        private static readonly WordDelimiterOptions LuceneOffsetsFlags = new WordDelimiterOptions
        {
            GenerateWordParts = true,
            GenerateNumberParts = true,
            CatenateAll = true,
            SplitOnCaseChange = true,
            SplitOnNumerics = true,
            StemEnglishPossessive = true,
        };

        [Fact]
        public void WordDelimiter_LuceneTestOffsets()
        {
            WordDelimiterGraphFilter wdf = new WordDelimiterGraphFilter(LuceneOffsetsFlags);
            List<Token> output = wdf.Apply(new List<Token> { new Token("foo-bar", 0, 1, 5, 12, TokenType.Word) });
            Assert.Equal(new[] { "foobar", "foo", "bar" }, Terms(output));
            Assert.Equal(new[] { 5, 5, 9 }, output.Select(t => t.StartOffset).ToArray());
            Assert.Equal(new[] { 12, 8, 12 }, output.Select(t => t.EndOffset).ToArray());
            Assert.Equal(new[] { 0, 0, 1 }, output.Select(t => t.Position).ToArray());
            Assert.Equal(new[] { 2, 1, 1 }, output.Select(t => t.PositionLength).ToArray());

            List<Token> illegal = wdf.Apply(new List<Token> { new Token("foo-bar", 0, 1, 5, 6, TokenType.Word) });
            Assert.Equal(new[] { 5, 5, 5 }, illegal.Select(t => t.StartOffset).ToArray());
            Assert.Equal(new[] { 6, 6, 6 }, illegal.Select(t => t.EndOffset).ToArray());
        }

        private static readonly WordDelimiterOptions SplitFlags = new WordDelimiterOptions
        {
            GenerateWordParts = true,
            GenerateNumberParts = true,
            SplitOnCaseChange = true,
            SplitOnNumerics = true,
            StemEnglishPossessive = true,
        };

        [Theory]
        [InlineData("basic-split", new[] { "basic", "split" })]
        [InlineData("camelCase", new[] { "camel", "Case" })]
        [InlineData("test's'", new[] { "test" })]
        [InlineData("Роберт", new[] { "Роберт" })]
        [InlineData("РобЕрт", new[] { "Роб", "Ерт" })]
        [InlineData("١٢٣٤", new[] { "١٢٣٤" })]
        [InlineData("ra's", new[] { "ra" })]
        public void WordDelimiter_LuceneTestSplits(string input, string[] expected)
        {
            WordDelimiterGraphFilter wdf = new WordDelimiterGraphFilter(SplitFlags);
            Assert.Equal(expected, Terms(wdf.Apply(Tokens(input))));
        }

        [Fact]
        public void WordDelimiter_LucenePossessiveWithoutStemming()
        {
            WordDelimiterGraphFilter wdf = new WordDelimiterGraphFilter(new WordDelimiterOptions { GenerateWordParts = true, GenerateNumberParts = true });
            Assert.Equal(new[] { "ra", "s" }, Terms(wdf.Apply(Tokens("ra's"))));
        }


        [Fact]
        public void WordDelimiter_CaseChangeCatenation()
        {
            WordDelimiterGraphFilter wdf = new WordDelimiterGraphFilter(new WordDelimiterOptions
            {
                GenerateWordParts = true,
                CatenateWords = true,
                SplitOnCaseChange = true,
            });
            Assert.Equal(new[] { "PowerShot", "Power", "Shot" }, Terms(wdf.Apply(Tokens("PowerShot"))));
        }

        [Fact]
        public void WordDelimiter_DelimiterOnlyTokenLeavesHole()
        {
            WordDelimiterGraphFilter wdf = new WordDelimiterGraphFilter(LuceneOffsetsFlags);
            List<Token> output = wdf.Apply(Tokens("LUCENE", "/", "SOLR"));
            Assert.Equal(new[] { "LUCENE", "SOLR" }, Terms(output));
            Assert.Equal(new[] { 0, 2 }, output.Select(t => t.Position).ToArray());
        }

        [Fact]
        public void WordDelimiter_CodeSplitsLettersAndDigits()
        {
            WordDelimiterGraphFilter wdf = new WordDelimiterGraphFilter(new WordDelimiterOptions
            {
                GenerateWordParts = true,
                GenerateNumberParts = true,
                PreserveOriginal = true,
                SplitOnNumerics = true,
            });
            List<Token> output = wdf.Apply(Tokens("3xl"));
            Assert.Equal(new[] { "3xl", "3", "xl" }, Terms(output));
            Assert.Equal(new[] { 0, 0, 1 }, output.Select(t => t.Position).ToArray());
        }

        // Pairs from Lucene's kstemTestData.zip (kstem_examples.txt), produced by the original Java KStemmer.
        [Theory]
        [InlineData("carding", "card")]
        [InlineData("professions", "profession")]
        [InlineData("electors", "elector")]
        [InlineData("buoyed", "buoy")]
        [InlineData("needed", "need")]
        [InlineData("moravians", "moravian")]
        [InlineData("battalions", "battalion")]
        [InlineData("endured", "endure")]
        [InlineData("streams", "stream")]
        [InlineData("stirring", "stirring")]
        [InlineData("seceding", "secede")]
        [InlineData("begged", "beg")]
        [InlineData("scalawags", "scalawag")]
        [InlineData("sees", "see")]
        [InlineData("magnates", "magnate")]
        [InlineData("australian", "australia")]
        [InlineData("enacted", "enact")]
        [InlineData("atheistical", "atheist")]
        [InlineData("getting", "get")]
        [InlineData("misgivings", "misgiving")]
        [InlineData("convicted", "convict")]
        [InlineData("saddles", "saddle")]
        [InlineData("hauling", "haul")]
        [InlineData("rides", "ride")]
        [InlineData("sealing", "sealing")]
        [InlineData("safeguarding", "safeguard")]
        [InlineData("discontented", "discontented")]
        [InlineData("casualties", "casualty")]
        [InlineData("representatives", "representative")]
        [InlineData("battleships", "battleship")]
        [InlineData("shoes", "shoe")]
        [InlineData("glasses", "glasses")]
        [InlineData("holders", "holder")]
        [InlineData("boxes", "box")]
        [InlineData("candles", "candle")]
        [InlineData("churches", "church")]
        [InlineData("running", "running")]
        [InlineData("congress", "congress")]
        [InlineData("hum", "hum")]
        [InlineData("", "")]
        public void KStem_LuceneVocabulary(string input, string expected)
        {
            Assert.Equal(expected, new KStemmer().Stem(input));
        }

        [Fact]
        public void KStemFilter_StemsTokens()
        {
            Assert.Equal(new[] { "shoe", "church" }, Terms(new KStemFilter().Apply(Tokens("shoes", "churches"))));
        }



        [Fact]
        public void Shingle_LuceneBigramsWithUnigrams()
        {
            ShingleFilter filter = new ShingleFilter(2, 2, true, ShingleFilter.DefaultSeparator);
            List<Token> output = filter.Apply(Tokens("please", "divide", "this", "sentence", "into", "shingles"));
            Assert.Equal(new[]
            {
                "please", "please divide", "divide", "divide this", "this", "this sentence",
                "sentence", "sentence into", "into", "into shingles", "shingles",
            }, Terms(output));
            Assert.Equal(new[] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5 }, output.Select(t => t.Position).ToArray());
            Assert.Equal(2, output[1].PositionLength);
        }

        [Fact]
        public void Shingle_CompoundJoinWithoutUnigrams()
        {
            ShingleFilter filter = new ShingleFilter(2, 2, false, string.Empty);
            Assert.Equal(new[] { "potholder", "holderset" }, Terms(filter.Apply(Tokens("pot", "holder", "set"))));
        }

        [Fact]
        public void EdgeNGram_LuceneFrontRange()
        {
            Assert.Equal(new[] { "a" }, Terms(new EdgeNGramTokenFilter(1, 1, false).Apply(Tokens("abcde"))));
            Assert.Equal(new[] { "a", "ab", "abc" }, Terms(new EdgeNGramTokenFilter(1, 3, false).Apply(Tokens("abcde"))));
            Assert.Empty(new EdgeNGramTokenFilter(6, 6, false).Apply(Tokens("abcde")));
            Assert.Equal(new[] { "a", "ab", "abc", "abcde" }, Terms(new EdgeNGramTokenFilter(1, 3, true).Apply(Tokens("abcde"))));
        }

        private static SynonymMap Synonyms()
        {
            return SynonymMap.Build(
                new TextAnalyzer(new StandardTokenizer(StandardTokenizer.DefaultMaxTokenLength), new ITokenFilter[] { new LowerCaseFilter() }),
                new List<IReadOnlyList<string>>
                {
                    new[] { "pajamas", "pjs" },
                    new[] { "swim suit", "swimsuit" },
                },
                new List<KeyValuePair<string, IReadOnlyList<string>>>
                {
                    new KeyValuePair<string, IReadOnlyList<string>>("tee", new[] { "t-shirt" }),
                });
        }

        [Fact]
        public void SynonymGraph_SingleWord()
        {
            SynonymGraphFilter filter = new SynonymGraphFilter(Synonyms());
            List<Token> output = filter.Apply(Tokens("kids", "pjs"));
            Assert.Equal(new[] { "kids", "pjs", "pajamas" }, Terms(output));
            Assert.Equal(1, output[2].Position);
            Assert.Equal(TokenType.Synonym, output[2].Type);
        }

        [Fact]
        public void SynonymGraph_MultiWordInputSpansPositions()
        {
            SynonymGraphFilter filter = new SynonymGraphFilter(Synonyms());
            List<Token> output = filter.Apply(Tokens("red", "swim", "suit"));
            Assert.Equal(new[] { "red", "swim", "swimsuit", "suit" }, Terms(output));
            Assert.Equal(1, output[2].Position);
            Assert.Equal(2, output[2].PositionLength);
        }

        [Fact]
        public void SynonymGraph_MultiWordOutputLaysWordsOut()
        {
            SynonymGraphFilter filter = new SynonymGraphFilter(Synonyms());
            List<Token> output = filter.Apply(Tokens("swimsuit"));
            Assert.Equal(new[] { "swimsuit", "swim", "suit" }, Terms(output));
            Assert.Equal(new[] { 0, 0, 1 }, output.Select(t => t.Position).ToArray());
        }

        [Fact]
        public void SynonymMap_OneWayRuleIsAnalyzed()
        {
            SynonymMap map = Synonyms();
            Assert.Contains(map.Lookup(new[] { "tee" }, 0, 1), o => string.Equals(string.Join(" ", o), "t shirt", System.StringComparison.Ordinal));
            Assert.Empty(map.Lookup(new[] { "t", "shirt" }, 0, 2));
        }


        // Cases from Lucene's TestEnglishMinimalStemFilter.
        [Theory]
        [InlineData("queries", "query")]
        [InlineData("phrases", "phrase")]
        [InlineData("corpus", "corpus")]
        [InlineData("stress", "stress")]
        [InlineData("kings", "king")]
        [InlineData("panels", "panel")]
        [InlineData("aerodynamics", "aerodynamic")]
        [InlineData("congress", "congress")]
        [InlineData("serious", "serious")]
        public void EnglishMinimalStemmer_LuceneCases(string input, string expected)
        {
            Assert.Equal(expected, EnglishMinimalStemmer.Stem(input));
        }

        [Fact]
        public void LengthFilter_KeepsTermsInRange()
        {
            List<Token> output = new LengthFilter(2, 4).Apply(Tokens("a", "bb", "ccc", "dddd", "eeeee"));
            Assert.Equal(new[] { "bb", "ccc", "dddd" }, Terms(output));
            Assert.Equal(new[] { 1, 2, 3 }, output.Select(t => t.Position).ToArray());
        }
    }
}
