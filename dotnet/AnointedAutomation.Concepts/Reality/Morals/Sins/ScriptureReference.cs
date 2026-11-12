// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-11-12 Jesus is King ✝️
// Stewarded by Alexander Fields

using System;

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// One Bible reference: a book, chapter, and a verse or inclusive verse range. Validated on
    /// construction (known book, chapter within the book, positive verse, ascending range).
    /// </summary>
    public readonly struct ScriptureReference : IEquatable<ScriptureReference>
    {
        // Chapter count per book, indexed by BibleBook value (index 0 is Unknown).
        private static readonly int[] ChapterCounts =
        {
            0, 50, 40, 27, 36, 34, 24, 21, 4, 31, 24, 22, 25, 29, 36, 10, 13, 10, 42, 150, 31, 12, 8, 66, 52, 5, 48, 12, 14, 3, 9, 1, 4, 7,
            3, 3, 3, 2, 14, 4, 28, 16, 24, 21, 28, 16, 16, 13, 6, 6, 4, 4, 5, 3, 6, 4, 3, 1, 13, 5, 5, 3, 5, 1, 1, 1, 22,
        };

        // Display name per book ("1 Corinthians"), indexed by BibleBook value, matching MoralConcept.Scripture.
        private static readonly string[] DisplayNames =
        {
            "Unknown", "Genesis", "Exodus", "Leviticus", "Numbers", "Deuteronomy", "Joshua", "Judges", "Ruth", "1 Samuel", "2 Samuel", "1 Kings", "2 Kings", "1 Chronicles", "2 Chronicles", "Ezra", "Nehemiah", "Esther", "Job", "Psalms", "Proverbs", "Ecclesiastes", "Song of Solomon", "Isaiah", "Jeremiah", "Lamentations", "Ezekiel", "Daniel", "Hosea", "Joel", "Amos", "Obadiah", "Jonah", "Micah", "Nahum", "Habakkuk", "Zephaniah", "Haggai", "Zechariah", "Malachi", "Matthew", "Mark", "Luke", "John", "Acts", "Romans", "1 Corinthians", "2 Corinthians", "Galatians", "Ephesians", "Philippians", "Colossians", "1 Thessalonians", "2 Thessalonians", "1 Timothy", "2 Timothy", "Titus", "Philemon", "Hebrews", "James", "1 Peter", "2 Peter", "1 John", "2 John", "3 John", "Jude", "Revelation",
        };

        /// <summary>
        /// Creates a reference.
        /// </summary>
        /// <param name="book">The book; never <see cref="BibleBook.Unknown"/>.</param>
        /// <param name="chapter">The chapter, 1 to the book's chapter count.</param>
        /// <param name="verse">The first verse, 1 or more.</param>
        /// <param name="endVerse">The last verse of a range, or 0 for a single verse.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when any part is out of range.</exception>
        public ScriptureReference(BibleBook book, int chapter, int verse, int endVerse)
        {
            if (book == BibleBook.Unknown || (uint)book >= (uint)ChapterCounts.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(book), book, "A reference needs a known book.");
            }

            if (chapter < 1 || chapter > ChapterCounts[(int)book])
            {
                throw new ArgumentOutOfRangeException(nameof(chapter), chapter, book + " has " + ChapterCounts[(int)book] + " chapters.");
            }

            if (verse < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(verse), verse, "Verses start at 1.");
            }

            if (endVerse != 0 && endVerse <= verse)
            {
                throw new ArgumentOutOfRangeException(nameof(endVerse), endVerse, "A range must end after it starts.");
            }

            Book = book;
            Chapter = chapter;
            Verse = verse;
            EndVerse = endVerse;
        }

        /// <summary>The book.</summary>
        public BibleBook Book { get; }

        /// <summary>The chapter.</summary>
        public int Chapter { get; }

        /// <summary>The first verse.</summary>
        public int Verse { get; }

        /// <summary>The last verse of a range, or 0 for a single verse.</summary>
        public int EndVerse { get; }

        /// <summary>Whether this reference spans a verse range.</summary>
        public bool IsRange => EndVerse != 0;

        /// <summary>
        /// Returns the number of chapters in a book.
        /// </summary>
        /// <param name="book">A known book.</param>
        /// <returns>The chapter count.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown for <see cref="BibleBook.Unknown"/> or an undefined value.</exception>
        public static int ChapterCount(BibleBook book)
        {
            if (book == BibleBook.Unknown || (uint)book >= (uint)ChapterCounts.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(book), book, "Unknown book.");
            }

            return ChapterCounts[(int)book];
        }

        /// <summary>
        /// Formats as "1 Corinthians 6:18" or "Exodus 21:22-25", the engine's Scripture style.
        /// </summary>
        /// <returns>The formatted reference.</returns>
        public override string ToString()
        {
            string head = DisplayNames[(int)Book] + " " + Chapter + ":" + Verse;
            return IsRange ? head + "-" + EndVerse : head;
        }

        /// <summary>
        /// Value equality.
        /// </summary>
        /// <param name="other">The other reference.</param>
        /// <returns>True when every part matches.</returns>
        public bool Equals(ScriptureReference other) =>
            Book == other.Book && Chapter == other.Chapter && Verse == other.Verse && EndVerse == other.EndVerse;

        /// <summary>
        /// Value equality.
        /// </summary>
        /// <param name="obj">The other object.</param>
        /// <returns>True when it is an equal reference.</returns>
        public override bool Equals(object obj) => obj != null && obj.GetType() == typeof(ScriptureReference) && Equals((ScriptureReference)obj);

        /// <summary>
        /// Hash code over every part.
        /// </summary>
        /// <returns>The hash code.</returns>
        public override int GetHashCode() => HashCode.Combine(Book, Chapter, Verse, EndVerse);
    }
}
