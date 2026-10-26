// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using System;
using System.Collections.Generic;
using AnointedAutomation.Search.Scoring;
using Xunit;

namespace AnointedAutomation.Search.Tests
{
    public class ScoringTests
    {
        [Fact]
        public void Idf_MatchesTantivyFormula()
        {
            // ln(1 + (10 - 1 + 0.5) / (1 + 0.5)) = ln(7.3333...) = 1.99243...
            decimal idf = Bm25.Idf(1, 10);
            Assert.InRange(idf, 1.9924m, 1.9925m);
            Assert.True(Bm25.Idf(1, 10) > Bm25.Idf(5, 10));
            Assert.True(Bm25.Idf(10, 10) > 0m);
            Assert.Throws<ArgumentOutOfRangeException>(() => Bm25.Idf(11, 10));
        }

        [Fact]
        public void Score_AtAverageLengthAndTfOne()
        {
            // len == avg: norm = k1, tf factor = (k1 + 1) / (1 + k1) = 1, score = idf.
            decimal score = Bm25.Score(2m, 1, 10, 10m, Bm25.DefaultK1, Bm25.DefaultB);
            Assert.Equal(2m, score);
        }

        [Fact]
        public void Score_SaturatesWithTermFrequencyAndPenalizesLength()
        {
            decimal tf1 = Bm25.Score(1m, 1, 10, 10m, 1.2m, 0.75m);
            decimal tf2 = Bm25.Score(1m, 2, 10, 10m, 1.2m, 0.75m);
            decimal tf50 = Bm25.Score(1m, 50, 10, 10m, 1.2m, 0.75m);
            Assert.True(tf2 > tf1);
            Assert.True(tf50 < 1m + 1.2m);
            decimal shortField = Bm25.Score(1m, 1, 5, 10m, 1.2m, 0.75m);
            decimal longField = Bm25.Score(1m, 1, 20, 10m, 1.2m, 0.75m);
            Assert.True(shortField > longField);
            decimal noNorm = Bm25.Score(1m, 1, 20, 10m, 1.2m, 0m);
            Assert.Equal(Bm25.Score(1m, 1, 5, 10m, 1.2m, 0m), noNorm);
        }

        [Fact]
        public void TopK_KeepsBestInOrder()
        {
            TopK<int> top = new TopK<int>(3, (a, b) => b.CompareTo(a));
            foreach (int v in new[] { 5, 1, 9, 3, 7, 2, 8 })
            {
                top.Add(v);
            }
            Assert.Equal(new List<int> { 9, 8, 7 }, top.ToSortedList());
        }

        [Fact]
        public void Phrase_InOrderWithSlop()
        {
            Assert.True(PositionIntersection.PhraseMatches(new[] { new[] { 1 }, new[] { 2 } }, 0));
            Assert.False(PositionIntersection.PhraseMatches(new[] { new[] { 2 }, new[] { 1 } }, 0));
            Assert.False(PositionIntersection.PhraseMatches(new[] { new[] { 0 }, new[] { 2 } }, 0));
            Assert.True(PositionIntersection.PhraseMatches(new[] { new[] { 0 }, new[] { 2 } }, 1));
            // Backtracking: the greedy choice (1) fails, the later one (2) succeeds.
            Assert.True(PositionIntersection.PhraseMatches(new[] { new[] { 0 }, new[] { 1, 2 }, new[] { 4 } }, 1));
            Assert.True(PositionIntersection.IntersectionExists(new[] { 1, 4, 9 }, new[] { 2, 9 }));
            Assert.True(PositionIntersection.Follows(new[] { 3 }, new[] { 5 }, 2));
            Assert.False(PositionIntersection.Follows(new[] { 3 }, new[] { 6 }, 2));
        }
    }
}
