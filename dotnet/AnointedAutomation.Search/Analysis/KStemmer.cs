// Copyright © Anointed Automation Corporation., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// Ported from Apache Lucene lucene/analysis/common/src/java/org/apache/lucene/analysis/en/KStemmer.java
// (Apache-2.0; KStem algorithm Copyright 2003 Center for Intelligent Information Retrieval, University of
// Massachusetts, Amherst, BSD-style license reproduced in NOTICE.md). Translated from Java to C#: the
// CharArrayMap dictionary became a Dictionary with a span alternate lookup and OpenStringBuilder became
// KStemBuffer. The algorithm, its tables and its comments are unchanged.

using System;
using System.Collections.Generic;

namespace AnointedAutomation.Search.Analysis
{
/// <summary>The Krovetz (KStem) English stemmer: dictionary-checked inflectional and derivational
/// stemming that only produces real words. Input must already be lowercase ASCII.</summary>
public sealed class KStemmer
{
  private const int MaxWordLen = 50;

  private static readonly string[] exceptionWords = {
    "aide",
    "bathe",
    "caste",
    "cute",
    "dame",
    "dime",
    "doge",
    "done",
    "dune",
    "envelope",
    "gage",
    "grille",
    "grippe",
    "lobe",
    "mane",
    "mare",
    "nape",
    "node",
    "pane",
    "pate",
    "plane",
    "pope",
    "programme",
    "quite",
    "ripe",
    "rote",
    "rune",
    "sage",
    "severe",
    "shoppe",
    "sine",
    "slime",
    "snipe",
    "steppe",
    "suite",
    "swinge",
    "tare",
    "tine",
    "tope",
    "tripe",
    "twine"
  };

  private static readonly string[][] directConflations = {
    new[] { "aging", "age" },
    new[] { "going", "go" },
    new[] { "goes", "go" },
    new[] { "lying", "lie" },
    new[] { "using", "use" },
    new[] { "owing", "owe" },
    new[] { "suing", "sue" },
    new[] { "dying", "die" },
    new[] { "tying", "tie" },
    new[] { "vying", "vie" },
    new[] { "aged", "age" },
    new[] { "used", "use" },
    new[] { "vied", "vie" },
    new[] { "cued", "cue" },
    new[] { "died", "die" },
    new[] { "eyed", "eye" },
    new[] { "hued", "hue" },
    new[] { "iced", "ice" },
    new[] { "lied", "lie" },
    new[] { "owed", "owe" },
    new[] { "sued", "sue" },
    new[] { "toed", "toe" },
    new[] { "tied", "tie" },
    new[] { "does", "do" },
    new[] { "doing", "do" },
    new[] { "aeronautical", "aeronautics" },
    new[] { "mathematical", "mathematics" },
    new[] { "political", "politics" },
    new[] { "metaphysical", "metaphysics" },
    new[] { "cylindrical", "cylinder" },
    new[] { "nazism", "nazi" },
    new[] { "ambiguity", "ambiguous" },
    new[] { "barbarity", "barbarous" },
    new[] { "credulity", "credulous" },
    new[] { "generosity", "generous" },
    new[] { "spontaneity", "spontaneous" },
    new[] { "unanimity", "unanimous" },
    new[] { "voracity", "voracious" },
    new[] { "fled", "flee" },
    new[] { "miscarriage", "miscarry" }
  };

  private static readonly string[][] countryNationality = {
    new[] { "afghan", "afghanistan" },
    new[] { "african", "africa" },
    new[] { "albanian", "albania" },
    new[] { "algerian", "algeria" },
    new[] { "american", "america" },
    new[] { "andorran", "andorra" },
    new[] { "angolan", "angola" },
    new[] { "arabian", "arabia" },
    new[] { "argentine", "argentina" },
    new[] { "armenian", "armenia" },
    new[] { "asian", "asia" },
    new[] { "australian", "australia" },
    new[] { "austrian", "austria" },
    new[] { "azerbaijani", "azerbaijan" },
    new[] { "azeri", "azerbaijan" },
    new[] { "bangladeshi", "bangladesh" },
    new[] { "belgian", "belgium" },
    new[] { "bermudan", "bermuda" },
    new[] { "bolivian", "bolivia" },
    new[] { "bosnian", "bosnia" },
    new[] { "botswanan", "botswana" },
    new[] { "brazilian", "brazil" },
    new[] { "british", "britain" },
    new[] { "bulgarian", "bulgaria" },
    new[] { "burmese", "burma" },
    new[] { "californian", "california" },
    new[] { "cambodian", "cambodia" },
    new[] { "canadian", "canada" },
    new[] { "chadian", "chad" },
    new[] { "chilean", "chile" },
    new[] { "chinese", "china" },
    new[] { "colombian", "colombia" },
    new[] { "croat", "croatia" },
    new[] { "croatian", "croatia" },
    new[] { "cuban", "cuba" },
    new[] { "cypriot", "cyprus" },
    new[] { "czechoslovakian", "czechoslovakia" },
    new[] { "danish", "denmark" },
    new[] { "egyptian", "egypt" },
    new[] { "equadorian", "equador" },
    new[] { "eritrean", "eritrea" },
    new[] { "estonian", "estonia" },
    new[] { "ethiopian", "ethiopia" },
    new[] { "european", "europe" },
    new[] { "fijian", "fiji" },
    new[] { "filipino", "philippines" },
    new[] { "finnish", "finland" },
    new[] { "french", "france" },
    new[] { "gambian", "gambia" },
    new[] { "georgian", "georgia" },
    new[] { "german", "germany" },
    new[] { "ghanian", "ghana" },
    new[] { "greek", "greece" },
    new[] { "grenadan", "grenada" },
    new[] { "guamian", "guam" },
    new[] { "guatemalan", "guatemala" },
    new[] { "guinean", "guinea" },
    new[] { "guyanan", "guyana" },
    new[] { "haitian", "haiti" },
    new[] { "hawaiian", "hawaii" },
    new[] { "holland", "dutch" },
    new[] { "honduran", "honduras" },
    new[] { "hungarian", "hungary" },
    new[] { "icelandic", "iceland" },
    new[] { "indonesian", "indonesia" },
    new[] { "iranian", "iran" },
    new[] { "iraqi", "iraq" },
    new[] { "iraqui", "iraq" },
    new[] { "irish", "ireland" },
    new[] { "israeli", "israel" },
    new[] { "italian", "italy" },
    new[] { "jamaican", "jamaica" },
    new[] { "japanese", "japan" },
    new[] { "jordanian", "jordan" },
    new[] { "kampuchean", "cambodia" },
    new[] { "kenyan", "kenya" },
    new[] { "korean", "korea" },
    new[] { "kuwaiti", "kuwait" },
    new[] { "lankan", "lanka" },
    new[] { "laotian", "laos" },
    new[] { "latvian", "latvia" },
    new[] { "lebanese", "lebanon" },
    new[] { "liberian", "liberia" },
    new[] { "libyan", "libya" },
    new[] { "lithuanian", "lithuania" },
    new[] { "macedonian", "macedonia" },
    new[] { "madagascan", "madagascar" },
    new[] { "malaysian", "malaysia" },
    new[] { "maltese", "malta" },
    new[] { "mauritanian", "mauritania" },
    new[] { "mexican", "mexico" },
    new[] { "micronesian", "micronesia" },
    new[] { "moldovan", "moldova" },
    new[] { "monacan", "monaco" },
    new[] { "mongolian", "mongolia" },
    new[] { "montenegran", "montenegro" },
    new[] { "moroccan", "morocco" },
    new[] { "myanmar", "burma" },
    new[] { "namibian", "namibia" },
    new[] { "nepalese", "nepal" },
    // new[] { "netherlands", "dutch" },
    new[] { "nicaraguan", "nicaragua" },
    new[] { "nigerian", "nigeria" },
    new[] { "norwegian", "norway" },
    new[] { "omani", "oman" },
    new[] { "pakistani", "pakistan" },
    new[] { "panamanian", "panama" },
    new[] { "papuan", "papua" },
    new[] { "paraguayan", "paraguay" },
    new[] { "peruvian", "peru" },
    new[] { "portuguese", "portugal" },
    new[] { "romanian", "romania" },
    new[] { "rumania", "romania" },
    new[] { "rumanian", "romania" },
    new[] { "russian", "russia" },
    new[] { "rwandan", "rwanda" },
    new[] { "samoan", "samoa" },
    new[] { "scottish", "scotland" },
    new[] { "serb", "serbia" },
    new[] { "serbian", "serbia" },
    new[] { "siam", "thailand" },
    new[] { "siamese", "thailand" },
    new[] { "slovakia", "slovak" },
    new[] { "slovakian", "slovak" },
    new[] { "slovenian", "slovenia" },
    new[] { "somali", "somalia" },
    new[] { "somalian", "somalia" },
    new[] { "spanish", "spain" },
    new[] { "swedish", "sweden" },
    new[] { "swiss", "switzerland" },
    new[] { "syrian", "syria" },
    new[] { "taiwanese", "taiwan" },
    new[] { "tanzanian", "tanzania" },
    new[] { "texan", "texas" },
    new[] { "thai", "thailand" },
    new[] { "tunisian", "tunisia" },
    new[] { "turkish", "turkey" },
    new[] { "ugandan", "uganda" },
    new[] { "ukrainian", "ukraine" },
    new[] { "uruguayan", "uruguay" },
    new[] { "uzbek", "uzbekistan" },
    new[] { "venezuelan", "venezuela" },
    new[] { "vietnamese", "viet" },
    new[] { "virginian", "virginia" },
    new[] { "yemeni", "yemen" },
    new[] { "yugoslav", "yugoslavia" },
    new[] { "yugoslavian", "yugoslavia" },
    new[] { "zambian", "zambia" },
    new[] { "zealander", "zealand" },
    new[] { "zimbabwean", "zimbabwe" }
  };

  private static readonly string[] supplementDict = {
    "aids",
    "applicator",
    "capacitor",
    "digitize",
    "electromagnet",
    "ellipsoid",
    "exosphere",
    "extensible",
    "ferromagnet",
    "graphics",
    "hydromagnet",
    "polygraph",
    "toroid",
    "superconduct",
    "backscatter",
    "connectionism"
  };

  private static readonly string[] properNouns = {
    "abrams",
    "achilles",
    "acropolis",
    "adams",
    "agnes",
    "aires",
    "alexander",
    "alexis",
    "alfred",
    "algiers",
    "alps",
    "amadeus",
    "ames",
    "amos",
    "andes",
    "angeles",
    "annapolis",
    "antilles",
    "aquarius",
    "archimedes",
    "arkansas",
    "asher",
    "ashly",
    "athens",
    "atkins",
    "atlantis",
    "avis",
    "bahamas",
    "bangor",
    "barbados",
    "barger",
    "bering",
    "brahms",
    "brandeis",
    "brussels",
    "bruxelles",
    "cairns",
    "camoros",
    "camus",
    "carlos",
    "celts",
    "chalker",
    "charles",
    "cheops",
    "ching",
    "christmas",
    "cocos",
    "collins",
    "columbus",
    "confucius",
    "conners",
    "connolly",
    "copernicus",
    "cramer",
    "cyclops",
    "cygnus",
    "cyprus",
    "dallas",
    "damascus",
    "daniels",
    "davies",
    "davis",
    "decker",
    "denning",
    "dennis",
    "descartes",
    "dickens",
    "doris",
    "douglas",
    "downs",
    "dreyfus",
    "dukakis",
    "dulles",
    "dumfries",
    "ecclesiastes",
    "edwards",
    "emily",
    "erasmus",
    "euphrates",
    "evans",
    "everglades",
    "fairbanks",
    "federales",
    "fisher",
    "fitzsimmons",
    "fleming",
    "forbes",
    "fowler",
    "france",
    "francis",
    "goering",
    "goodling",
    "goths",
    "grenadines",
    "guiness",
    "hades",
    "harding",
    "harris",
    "hastings",
    "hawkes",
    "hawking",
    "hayes",
    "heights",
    "hercules",
    "himalayas",
    "hippocrates",
    "hobbs",
    "holmes",
    "honduras",
    "hopkins",
    "hughes",
    "humphreys",
    "illinois",
    "indianapolis",
    "inverness",
    "iris",
    "iroquois",
    "irving",
    "isaacs",
    "italy",
    "james",
    "jarvis",
    "jeffreys",
    "jesus",
    "jones",
    "josephus",
    "judas",
    "julius",
    "kansas",
    "keynes",
    "kipling",
    "kiwanis",
    "lansing",
    "laos",
    "leeds",
    "levis",
    "leviticus",
    "lewis",
    "louis",
    "maccabees",
    "madras",
    "maimonides",
    "maldive",
    "massachusetts",
    "matthews",
    "mauritius",
    "memphis",
    "mercedes",
    "midas",
    "mingus",
    "minneapolis",
    "mohammed",
    "moines",
    "morris",
    "moses",
    "myers",
    "myknos",
    "nablus",
    "nanjing",
    "nantes",
    "naples",
    "neal",
    "netherlands",
    "nevis",
    "nostradamus",
    "oedipus",
    "olympus",
    "orleans",
    "orly",
    "papas",
    "paris",
    "parker",
    "pauling",
    "peking",
    "pershing",
    "peter",
    "peters",
    "philippines",
    "phineas",
    "pisces",
    "pryor",
    "pythagoras",
    "queens",
    "rabelais",
    "ramses",
    "reynolds",
    "rhesus",
    "rhodes",
    "richards",
    "robins",
    "rodgers",
    "rogers",
    "rubens",
    "sagittarius",
    "seychelles",
    "socrates",
    "texas",
    "thames",
    "thomas",
    "tiberias",
    "tunis",
    "venus",
    "vilnius",
    "wales",
    "warner",
    "wilkins",
    "williams",
    "wyoming",
    "xmas",
    "yonkers",
    "zeus",
    "frances",
    "aarhus",
    "adonis",
    "andrews",
    "angus",
    "antares",
    "aquinas",
    "arcturus",
    "ares",
    "artemis",
    "augustus",
    "ayers",
    "barnabas",
    "barnes",
    "becker",
    "bejing",
    "biggs",
    "billings",
    "boeing",
    "boris",
    "borroughs",
    "briggs",
    "buenos",
    "calais",
    "caracas",
    "cassius",
    "cerberus",
    "ceres",
    "cervantes",
    "chantilly",
    "chartres",
    "chester",
    "connally",
    "conner",
    "coors",
    "cummings",
    "curtis",
    "daedalus",
    "dionysus",
    "dobbs",
    "dolores",
    "edmonds"
  };

  private sealed class DictEntry {
    public readonly bool exception;
    public readonly string root;

    public DictEntry(string root, bool isException) {
      this.root = root;
      this.exception = isException;
    }
  }

  private static readonly Dictionary<string, DictEntry> dict_ht = initializeDictHash();

  private static readonly Dictionary<string, DictEntry>.AlternateLookup<ReadOnlySpan<char>> dict_lookup =
      dict_ht.GetAlternateLookup<ReadOnlySpan<char>>();

  private static DictEntry DictGet(ReadOnlySpan<char> key) {
    DictEntry found;
    return dict_lookup.TryGetValue(key, out found) ? found : null;
  }

  /***
   * caching off private int maxCacheSize; private CharArrayMap<string> cache =
   * null; private static final string SAME = "SAME"; // use if stemmed form is
   * the same
   ***/

  private readonly KStemBuffer word = new KStemBuffer();

  private int j; /* index of final letter in stem (within word) */
  private int k; /*
                  * INDEX of final letter in word. You must add 1 to k to get
                  * the current length of word. When you want the length of
                  * word, use the method wordLength, which returns (k+1).
                  */

  /*
   * private void initializeStemHash() { if (maxCacheSize > 0) cache = new
   * CharArrayMap<string>(maxCacheSize,false); }
   ***/

  private char penultChar() {
    return word.CharAt(k - 1);
  }

  private bool isVowel(int index) {
    return !isCons(index);
  }

  private bool isCons(int index) {
    char ch;

    ch = word.CharAt(index);

    if ((ch == 'a') || (ch == 'e') || (ch == 'i') || (ch == 'o') || (ch == 'u')) return false;
    if ((ch != 'y') || (index == 0)) return true;
    else return (!isCons(index - 1));
  }

  private static Dictionary<string, DictEntry> initializeDictHash() {
    DictEntry defaultEntry;
    DictEntry entry;

    Dictionary<string, DictEntry> d = new Dictionary<string, DictEntry>(StringComparer.Ordinal);
    foreach (string exceptionWord in exceptionWords) {
      if (!d.ContainsKey(exceptionWord)) {
        entry = new DictEntry(exceptionWord, true);
        d[exceptionWord] = entry;
      } else {
        throw new InvalidOperationException(
            "Warning: Entry [" + exceptionWord + "] already in dictionary 1");
      }
    }

    foreach (string[] directConflation in directConflations) {
      if (!d.ContainsKey(directConflation[0])) {
        entry = new DictEntry(directConflation[1], false);
        d[directConflation[0]] = entry;
      } else {
        throw new InvalidOperationException(
            "Warning: Entry [" + directConflation[0] + "] already in dictionary 2");
      }
    }

    foreach (string[] strings in countryNationality) {
      if (!d.ContainsKey(strings[0])) {
        entry = new DictEntry(strings[1], false);
        d[strings[0]] = entry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + strings[0] + "] already in dictionary 3");
      }
    }

    defaultEntry = new DictEntry(null, false);

    string[] array;
    array = KStemData1.Data;

    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData2.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData3.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData4.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData5.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData6.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData7.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    array = KStemData8.Data;
    foreach (string s in array) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 4");
      }
    }

    foreach (string s in supplementDict) {
      if (!d.ContainsKey(s)) {
        d[s] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + s + "] already in dictionary 5");
      }
    }

    foreach (string properNoun in properNouns) {
      if (!d.ContainsKey(properNoun)) {
        d[properNoun] = defaultEntry;
      } else {
        throw new InvalidOperationException("Warning: Entry [" + properNoun + "] already in dictionary 6");
      }
    }

    return d;
  }

  private bool isAlpha(char ch) {
    return ch >= 'a' && ch <= 'z'; // terms must be lowercased already
  }

  /* length of stem within word */
  private int stemLength() {
    return j + 1;
  }

  private bool endsIn(char[] s) {
    if (s.Length > k) return false;

    int r = word.Length - s.Length; /* length of word before this suffix */
    j = k;
    for (int r1 = r, i = 0; i < s.Length; i++, r1++) {
      if (s[i] != word.CharAt(r1)) return false;
    }
    j = r - 1; /* index of the character BEFORE the posfix */
    return true;
  }

  private bool endsIn(char a, char b) {
    if (2 > k) return false;
    // check left to right since the endings have often already matched
    if (word.CharAt(k - 1) == a && word.CharAt(k) == b) {
      j = k - 2;
      return true;
    }
    return false;
  }

  private bool endsIn(char a, char b, char c) {
    if (3 > k) return false;
    if (word.CharAt(k - 2) == a && word.CharAt(k - 1) == b && word.CharAt(k) == c) {
      j = k - 3;
      return true;
    }
    return false;
  }

  private bool endsIn(char a, char b, char c, char d) {
    if (4 > k) return false;
    if (word.CharAt(k - 3) == a
        && word.CharAt(k - 2) == b
        && word.CharAt(k - 1) == c
        && word.CharAt(k) == d) {
      j = k - 4;
      return true;
    }
    return false;
  }

  private DictEntry wordInDict() {
    /*
     * if (matchedEntry is not null) { if (dict_ht.get(word.getArray(), 0,
     * word.Length) != matchedEntry) {
     * System.out.println("Uh oh... cached entry doesn't match"); } return
     * matchedEntry; }
     */
    if (matchedEntry is not null) return matchedEntry;
    DictEntry e = DictGet(word.AsSpan());
    if (e is not null && !e.exception) {
      matchedEntry = e; // only cache if it's not an exception.
    }
    // lookups.add(word.ToString());
    return e;
  }

  /* Convert plurals to singular form, and '-ies' to 'y' */
  private void plural() {
    if (word.CharAt(k) == 's') {
      if (endsIn('i', 'e', 's')) {
        word.SetLength(j + 3);
        k--;
        if (lookup()) /* ensure calories -> calorie */ return;
        k++;
        word.UnsafeWrite('s');
        setSuffix("y");
        lookup();
      } else if (endsIn('e', 's')) {
        /* try just removing the "s" */
        word.SetLength(j + 2);
        k--;

        /*
         * note: don't check for exceptions here. So, `aides' -> `aide', but
         * `aided' -> `aid'. The exception for double s is used to prevent
         * crosses -> crosse. This is actually correct if crosses is a plural
         * noun (a type of racket used in lacrosse), but the verb is much more
         * common
         */

        /*
         * YCS: this was the one place where lookup was not followed by return.
         * So restructure it. if ((j>0)&&(lookup(word.ToString())) &&
         * !((word.CharAt(j) == 's') && (word.CharAt(j-1) == 's'))) return;
         */
        bool tryE = j > 0 && !((word.CharAt(j) == 's') && (word.CharAt(j - 1) == 's'));
        if (tryE && lookup()) return;

        /* try removing the "es" */

        word.SetLength(j + 1);
        k--;
        if (lookup()) return;

        /* the default is to retain the "e" */
        word.UnsafeWrite('e');
        k++;

        if (!tryE) lookup(); // if we didn't try the "e" ending before
        return;
      } else {
        if (word.Length > 3 && penultChar() != 's' && !endsIn('o', 'u', 's')) {
          /* unless the word ends in "ous" or a double "s", remove the final "s" */

          word.SetLength(k);
          k--;
          lookup();
        }
      }
    }
  }

  private void setSuffix(string s) {
    setSuff(s, s.Length);
  }

  /* replace old suffix with s */
  private void setSuff(string s, int len) {
    word.SetLength(j + 1);
    for (int l = 0; l < len; l++) {
      word.UnsafeWrite(s[l]);
    }
    k = j + len;
  }

  /* Returns true if the word is found in the dictionary */
  // almost all uses of lookup() return immediately and are
  // followed by another lookup in the dict. Store the match
  // to avoid this double lookup.
  private DictEntry matchedEntry = null;

  private bool lookup() {
    /*
     * debugging code string thisLookup = word.ToString(); bool added =
     * lookups.add(thisLookup); if (!added) {
     * System.out.println("######extra lookup:" + thisLookup); // occaasional
     * extra lookups aren't necessarily errors... could happen by diff
     * manipulations // throw new InvalidOperationException("######extra lookup:" +
     * thisLookup); } else { // System.out.println("new lookup:" + thisLookup);
     * }
     */

    matchedEntry = DictGet(word.AsSpan());
    return matchedEntry is not null;
  }

  // Set<string> lookups = new HashSet<>();

  /* convert past tense (-ed) to present, and `-ied' to `y' */
  private void pastTense() {
    /*
     * Handle words less than 5 letters with a direct mapping This prevents
     * (fled -> fl).
     */
    if (word.Length <= 4) return;

    if (endsIn('i', 'e', 'd')) {
      word.SetLength(j + 3);
      k--;
      if (lookup()) /* we almost always want to convert -ied to -y, but */
        return; /* this isn't true for short words (died->die) */
      k++; /* I don't know any long words that this applies to, */
      word.UnsafeWrite('d'); /* but just in case... */
      setSuffix("y");
      lookup();
      return;
    }

    /* the vowelInStem() is necessary so we don't stem acronyms */
    if (endsIn('e', 'd') && vowelInStem()) {
      /* see if the root ends in `e' */
      word.SetLength(j + 2);
      k = j + 1;

      DictEntry entry = wordInDict();
      if (entry is not null) if (!entry.exception) /*
                                                * if it's in the dictionary and
                                                * not an exception
                                                */ return;

      /* try removing the "ed" */
      word.SetLength(j + 1);
      k = j;
      if (lookup()) return;

      /*
       * try removing a doubled consonant. if the root isn't found in the
       * dictionary, the default is to leave it doubled. This will correctly
       * capture `backfilled' -> `backfill' instead of `backfill' ->
       * `backfille', and seems correct most of the time
       */

      if (doubleC(k)) {
        word.SetLength(k);
        k--;
        if (lookup()) return;
        word.UnsafeWrite(word.CharAt(k));
        k++;
        lookup();
        return;
      }

      /* if we have a `un-' prefix, then leave the word alone */
      /* (this will sometimes screw up with `under-', but we */
      /* will take care of that later) */

      if ((word.CharAt(0) == 'u') && (word.CharAt(1) == 'n')) {
        word.UnsafeWrite('e');
        word.UnsafeWrite('d');
        k = k + 2;
        // nolookup()
        return;
      }

      /*
       * it wasn't found by just removing the `d' or the `ed', so prefer to end
       * with an `e' (e.g., `microcoded' -> `microcode').
       */

      word.SetLength(j + 1);
      word.UnsafeWrite('e');
      k = j + 1;
      // nolookup() - we already tried the "e" ending
      return;
    }
  }

  /* return TRUE if word ends with a double consonant */
  private bool doubleC(int i) {
    if (i < 1) return false;

    if (word.CharAt(i) != word.CharAt(i - 1)) return false;
    return (isCons(i));
  }

  private bool vowelInStem() {
    for (int i = 0; i < stemLength(); i++) {
      if (isVowel(i)) return true;
    }
    return false;
  }

  /* handle `-ing' endings */
  private void aspect() {
    /*
     * handle short words (aging -> age) via a direct mapping. This prevents
     * (thing -> the) in the version of this routine that ignores inflectional
     * variants that are mentioned in the dictionary (when the root is also
     * present)
     */

    if (word.Length <= 5) return;

    /* the vowelinstem() is necessary so we don't stem acronyms */
    if (endsIn('i', 'n', 'g') && vowelInStem()) {

      /* try adding an `e' to the stem and check against the dictionary */
      word.SetCharAt(j + 1, 'e');
      word.SetLength(j + 2);
      k = j + 1;

      DictEntry entry = wordInDict();
      if (entry is not null) {
        if (!entry.exception) /* if it's in the dictionary and not an exception */ return;
      }

      /* adding on the `e' didn't work, so remove it */
      word.SetLength(k);
      k--; /* note that `ing' has also been removed */

      if (lookup()) return;

      /* if I can remove a doubled consonant and get a word, then do so */
      if (doubleC(k)) {
        k--;
        word.SetLength(k + 1);
        if (lookup()) return;
        word.UnsafeWrite(word.CharAt(k)); /* restore the doubled consonant */

        /* the default is to leave the consonant doubled */
        /* (e.g.,`fingerspelling' -> `fingerspell'). Unfortunately */
        /* `bookselling' -> `booksell' and `mislabelling' -> `mislabell'). */
        /* Without making the algorithm significantly more complicated, this */
        /* is the best I can do */
        k++;
        lookup();
        return;
      }

      /*
       * the word wasn't in the dictionary after removing the stem, and then
       * checking with and without a final `e'. The default is to add an `e'
       * unless the word ends in two consonants, so `microcoding' ->
       * `microcode'. The two consonants restriction wouldn't normally be
       * necessary, but is needed because we don't try to deal with prefixes and
       * compounds, and most of the time it is correct (e.g., footstamping ->
       * footstamp, not footstampe; however, decoupled -> decoupl). We can
       * prevent almost all of the incorrect stems if we try to do some prefix
       * analysis first
       */

      if ((j > 0) && isCons(j) && isCons(j - 1)) {
        k = j;
        word.SetLength(k + 1);
        // nolookup() because we already did according to the comment
        return;
      }

      word.SetLength(j + 1);
      word.UnsafeWrite('e');
      k = j + 1;
      // nolookup(); we already tried an 'e' ending
      return;
    }
  }

  /*
   * this routine deals with -ity endings. It accepts -ability, -ibility, and
   * -ality, even without checking the dictionary because they are so
   * productive. The first two are mapped to -ble, and the -ity is remove for
   * the latter
   */
  private void ityEndings() {
    int old_k = k;

    if (endsIn('i', 't', 'y')) {
      word.SetLength(j + 1); /* try just removing -ity */
      k = j;
      if (lookup()) return;
      word.UnsafeWrite('e'); /* try removing -ity and adding -e */
      k = j + 1;
      if (lookup()) return;
      word.SetCharAt(j + 1, 'i');
      word.Append("ty");
      k = old_k;
      /*
       * the -ability and -ibility endings are highly productive, so just accept
       * them
       */
      if ((j > 0) && (word.CharAt(j - 1) == 'i') && (word.CharAt(j) == 'l')) {
        word.SetLength(j - 1);
        word.Append("le"); /* convert to -ble */
        k = j;
        lookup();
        return;
      }

      /* ditto for -ivity */
      if ((j > 0) && (word.CharAt(j - 1) == 'i') && (word.CharAt(j) == 'v')) {
        word.SetLength(j + 1);
        word.UnsafeWrite('e'); /* convert to -ive */
        k = j + 1;
        lookup();
        return;
      }
      /* ditto for -ality */
      if ((j > 0) && (word.CharAt(j - 1) == 'a') && (word.CharAt(j) == 'l')) {
        word.SetLength(j + 1);
        k = j;
        lookup();
        return;
      }

      /*
       * if the root isn't in the dictionary, and the variant *is* there, then
       * use the variant. This allows `immunity'->`immune', but prevents
       * `capacity'->`capac'. If neither the variant nor the root form are in
       * the dictionary, then remove the ending as a default
       */

      if (lookup()) return;

      /* the default is to remove -ity altogether */
      word.SetLength(j + 1);
      k = j;
      // nolookup(), we already did it.
      return;
    }
  }

  /* handle -ence and -ance */
  private void nceEndings() {
    int old_k = k;
    char word_char;

    if (endsIn('n', 'c', 'e')) {
      word_char = word.CharAt(j);
      if (!((word_char == 'e') || (word_char == 'a'))) return;
      word.SetLength(j);
      word.UnsafeWrite('e'); /* try converting -e/ance to -e (adherance/adhere) */
      k = j;
      if (lookup()) return;
      word.SetLength(j); /*
                          * try removing -e/ance altogether
                          * (disappearance/disappear)
                          */
      k = j - 1;
      if (lookup()) return;
      word.UnsafeWrite(word_char); /* restore the original ending */
      word.Append("nce");
      k = old_k;
      // nolookup() because we restored the original ending
    }
    return;
  }

  /* handle -ness */
  private void nessEndings() {
    if (endsIn('n', 'e', 's', 's')) {
      /*
       * this is a very productive endings, so
       * just accept it
       */
      word.SetLength(j + 1);
      k = j;
      if (word.CharAt(j) == 'i') word.SetCharAt(j, 'y');
      lookup();
    }
    return;
  }

  /* handle -ism */
  private void ismEndings() {
    if (endsIn('i', 's', 'm')) {
      /*
       * this is a very productive ending, so just
       * accept it
       */
      word.SetLength(j + 1);
      k = j;
      lookup();
    }
    return;
  }

  /* this routine deals with -ment endings. */
  private void mentEndings() {
    int old_k = k;

    if (endsIn('m', 'e', 'n', 't')) {
      word.SetLength(j + 1);
      k = j;
      if (lookup()) return;
      word.Append("ment");
      k = old_k;
      // nolookup
    }
    return;
  }

  /* this routine deals with -ize endings. */
  private void izeEndings() {
    int old_k = k;

    if (endsIn('i', 'z', 'e')) {
      word.SetLength(j + 1); /* try removing -ize entirely */
      k = j;
      if (lookup()) return;
      word.UnsafeWrite('i');

      if (doubleC(j)) {
        /* allow for a doubled consonant */
        word.SetLength(j);
        k = j - 1;
        if (lookup()) return;
        word.UnsafeWrite(word.CharAt(j - 1));
      }

      word.SetLength(j + 1);
      word.UnsafeWrite('e'); /* try removing -ize and adding -e */
      k = j + 1;
      if (lookup()) return;
      word.SetLength(j + 1);
      word.Append("ize");
      k = old_k;
      // nolookup()
    }
    return;
  }

  /* handle -ency and -ancy */
  private void ncyEndings() {
    if (endsIn('n', 'c', 'y')) {
      if (!((word.CharAt(j) == 'e') || (word.CharAt(j) == 'a'))) return;
      word.SetCharAt(j + 2, 't'); /* try converting -ncy to -nt */
      word.SetLength(j + 3);
      k = j + 2;

      if (lookup()) return;

      word.SetCharAt(j + 2, 'c'); /* the default is to convert it to -nce */
      word.UnsafeWrite('e');
      k = j + 3;
      lookup();
    }
    return;
  }

  /* handle -able and -ible */
  private void bleEndings() {
    int old_k = k;
    char word_char;

    if (endsIn('b', 'l', 'e')) {
      if (!((word.CharAt(j) == 'a') || (word.CharAt(j) == 'i'))) return;
      word_char = word.CharAt(j);
      word.SetLength(j); /* try just removing the ending */
      k = j - 1;
      if (lookup()) return;
      if (doubleC(k)) {
        /* allow for a doubled consonant */
        word.SetLength(k);
        k--;
        if (lookup()) return;
        k++;
        word.UnsafeWrite(word.CharAt(k - 1));
      }
      word.SetLength(j);
      word.UnsafeWrite('e'); /* try removing -a/ible and adding -e */
      k = j;
      if (lookup()) return;
      word.SetLength(j);
      word.Append("ate"); /* try removing -able and adding -ate */
      /* (e.g., compensable/compensate) */
      k = j + 2;
      if (lookup()) return;
      word.SetLength(j);
      word.UnsafeWrite(word_char); /* restore the original values */
      word.Append("ble");
      k = old_k;
      // nolookup()
    }
    return;
  }

  /*
   * handle -ic endings. This is fairly straightforward, but this is also the
   * only place we try *expanding* an ending, -ic -> -ical. This is to handle
   * cases like `canonic' -> `canonical'
   */
  private void icEndings() {
    if (endsIn('i', 'c')) {
      word.SetLength(j + 3);
      word.Append("al"); /* try converting -ic to -ical */
      k = j + 4;
      if (lookup()) return;

      word.SetCharAt(j + 1, 'y'); /* try converting -ic to -y */
      word.SetLength(j + 2);
      k = j + 1;
      if (lookup()) return;

      word.SetCharAt(j + 1, 'e'); /* try converting -ic to -e */
      if (lookup()) return;

      word.SetLength(j + 1); /* try removing -ic altogether */
      k = j;
      if (lookup()) return;
      word.Append("ic"); /* restore the original ending */
      k = j + 2;
      // nolookup()
    }
    return;
  }

  private static readonly char[] ization = "ization".ToCharArray();
  private static readonly char[] ition = "ition".ToCharArray();
  private static readonly char[] ation = "ation".ToCharArray();
  private static readonly char[] ication = "ication".ToCharArray();

  /* handle some derivational endings */
  /*
   * this routine deals with -ion, -ition, -ation, -ization, and -ication. The
   * -ization ending is always converted to -ize
   */
  private void ionEndings() {
    int old_k = k;
    if (!endsIn('i', 'o', 'n')) {
      return;
    }

    if (endsIn(ization)) {
      /*
       * the -ize ending is very productive, so simply
       * accept it as the root
       */
      word.SetLength(j + 3);
      word.UnsafeWrite('e');
      k = j + 3;
      lookup();
      return;
    }

    if (endsIn(ition)) {
      word.SetLength(j + 1);
      word.UnsafeWrite('e');
      k = j + 1;
      if (lookup()) {
        // remove -ition and add `e', and check against the
        return; /* (e.g., definition->define, opposition->oppose) */
      }

      /* restore original values */
      word.SetLength(j + 1);
      word.Append("ition");
      k = old_k;
      // nolookup()
    } else if (endsIn(ation)) {
      word.SetLength(j + 3);
      word.UnsafeWrite('e');
      k = j + 3;
      if (lookup()) {
        /* remove -ion and add `e', and check against the dictionary */
        return; /* (elimination -> eliminate) */
      }

      word.SetLength(j + 1);
      word.UnsafeWrite('e');
      // remove -ation and add `e', and check against the dictionary
      k = j + 1;
      if (lookup()) return;

      word.SetLength(j + 1);
      // just remove -ation (resignation->resign) and check dictionary
      k = j;
      if (lookup()) return;

      /* restore original values */
      word.SetLength(j + 1);
      word.Append("ation");
      k = old_k;
      // nolookup()

    }

    /*
     * test -ication after -ation is attempted (e.g., `complication->complicate'
     * rather than `complication->comply')
     */

    if (endsIn(ication)) {
      word.SetLength(j + 1);
      word.UnsafeWrite('y');
      k = j + 1;
      if (lookup()) {
        // remove -ication and add `y', and check against the dictionary
        return; /* (e.g., amplification -> amplify) */
      }

      /* restore original values */
      word.SetLength(j + 1);
      word.Append("ication");
      k = old_k;
      // nolookup()
    }

    // if (endsIn(ion)) {
    if (true) { // we checked for this earlier... just need to set "j"
      j = k - 3; // YCS

      word.SetLength(j + 1);
      word.UnsafeWrite('e');
      k = j + 1;
      if (lookup()) /* remove -ion and add `e', and check against the dictionary */ return;

      word.SetLength(j + 1);
      k = j;
      if (lookup()) /* remove -ion, and if it's found, treat that as the root */ return;

      /* restore original values */
      word.SetLength(j + 1);
      word.Append("ion");
      k = old_k;
      // nolookup()
    }

    // nolookup(); all of the other paths restored original values
    return;
  }

  /*
   * this routine deals with -er, -or, -ier, and -eer. The -izer ending is
   * always converted to -ize
   */
  private void erAndOrEndings() {
    int old_k = k;

    if (word.CharAt(k) != 'r') return; // YCS

    char word_char; /* so we can remember if it was -er or -or */

    if (endsIn('i', 'z', 'e', 'r')) {
      /*
       * -ize is very productive, so accept it
       * as the root
       */
      word.SetLength(j + 4);
      k = j + 3;
      lookup();
      return;
    }

    if (endsIn('e', 'r') || endsIn('o', 'r')) {
      word_char = word.CharAt(j + 1);
      if (doubleC(j)) {
        word.SetLength(j);
        k = j - 1;
        if (lookup()) return;
        word.UnsafeWrite(word.CharAt(j - 1)); /* restore the doubled consonant */
      }

      if (word.CharAt(j) == 'i') {
        /* do we have a -ier ending? */
        word.SetCharAt(j, 'y');
        word.SetLength(j + 1);
        k = j;
        if (lookup()) /* yes, so check against the dictionary */ return;
        word.SetCharAt(j, 'i'); /* restore the endings */
        word.UnsafeWrite('e');
      }

      if (word.CharAt(j) == 'e') {
        /* handle -eer */
        word.SetLength(j);
        k = j - 1;
        if (lookup()) return;
        word.UnsafeWrite('e');
      }

      word.SetLength(j + 2); /* remove the -r ending */
      k = j + 1;
      if (lookup()) return;
      word.SetLength(j + 1); /* try removing -er/-or */
      k = j;
      if (lookup()) return;
      word.UnsafeWrite('e'); /* try removing -or and adding -e */
      k = j + 1;
      if (lookup()) return;
      word.SetLength(j + 1);
      word.UnsafeWrite(word_char);
      word.UnsafeWrite('r'); /* restore the word to the way it was */
      k = old_k;
      // nolookup()
    }
  }

  /*
   * this routine deals with -ly endings. The -ally ending is always converted
   * to -al Sometimes this will temporarily leave us with a non-word (e.g.,
   * heuristically maps to heuristical), but then the -al is removed in the next
   * step.
   */
  private void lyEndings() {
    int old_k = k;

    if (endsIn('l', 'y')) {

      word.SetCharAt(j + 2, 'e'); /* try converting -ly to -le */

      if (lookup()) return;
      word.SetCharAt(j + 2, 'y');

      word.SetLength(j + 1); /* try just removing the -ly */
      k = j;

      if (lookup()) return;

      if ((j > 0) && (word.CharAt(j - 1) == 'a') && (word.CharAt(j) == 'l')) /*
                                                                              * always
                                                                              * convert
                                                                              * -
                                                                              * ally
                                                                              * to
                                                                              * -
                                                                              * al
                                                                              */ return;
      word.Append("ly");
      k = old_k;

      if ((j > 0) && (word.CharAt(j - 1) == 'a') && (word.CharAt(j) == 'b')) {
        /*
         * always
         * convert
         * -
         * ably
         * to
         * -
         * able
         */
        word.SetCharAt(j + 2, 'e');
        k = j + 2;
        return;
      }

      if (word.CharAt(j) == 'i') {
        /* e.g., militarily -> military */
        word.SetLength(j);
        word.UnsafeWrite('y');
        k = j;
        if (lookup()) return;
        word.SetLength(j);
        word.Append("ily");
        k = old_k;
      }

      word.SetLength(j + 1); /* the default is to remove -ly */

      k = j;
      // nolookup()... we already tried removing the "ly" variant
    }
    return;
  }

  /*
   * this routine deals with -al endings. Some of the endings from the previous
   * routine are finished up here.
   */
  private void alEndings() {
    int old_k = k;

    if (word.Length < 4) return;
    if (endsIn('a', 'l')) {
      word.SetLength(j + 1);
      k = j;
      if (lookup()) /* try just removing the -al */ return;

      if (doubleC(j)) {
        /* allow for a doubled consonant */
        word.SetLength(j);
        k = j - 1;
        if (lookup()) return;
        word.UnsafeWrite(word.CharAt(j - 1));
      }

      word.SetLength(j + 1);
      word.UnsafeWrite('e'); /* try removing the -al and adding -e */
      k = j + 1;
      if (lookup()) return;

      word.SetLength(j + 1);
      word.Append("um"); /* try converting -al to -um */
      /* (e.g., optimal - > optimum ) */
      k = j + 2;
      if (lookup()) return;

      word.SetLength(j + 1);
      word.Append("al"); /* restore the ending to the way it was */
      k = old_k;

      if ((j > 0) && (word.CharAt(j - 1) == 'i') && (word.CharAt(j) == 'c')) {
        word.SetLength(j - 1); /* try removing -ical */
        k = j - 2;
        if (lookup()) return;

        word.SetLength(j - 1);
        word.UnsafeWrite('y'); /* try turning -ical to -y (e.g., bibliographical) */
        k = j - 1;
        if (lookup()) return;

        word.SetLength(j - 1);
        word.Append("ic"); /* the default is to convert -ical to -ic */
        k = j;
        // nolookup() ... converting ical to ic means removing "al" which we
        // already tried
        // ERROR
        lookup();
        return;
      }

      if (word.CharAt(j) == 'i') {
        /* sometimes -ial endings should be removed */
        word.SetLength(j); /* (sometimes it gets turned into -y, but we */
        k = j - 1; /* aren't dealing with that case for now) */
        if (lookup()) return;
        word.Append("ial");
        k = old_k;
        lookup();
      }
    }
    return;
  }

  /*
   * this routine deals with -ive endings. It normalizes some of the -ative
   * endings directly, and also maps some -ive endings to -ion.
   */
  private void iveEndings() {
    int old_k = k;

    if (endsIn('i', 'v', 'e')) {
      word.SetLength(j + 1); /* try removing -ive entirely */
      k = j;
      if (lookup()) return;

      word.UnsafeWrite('e'); /* try removing -ive and adding -e */
      k = j + 1;
      if (lookup()) return;
      word.SetLength(j + 1);
      word.Append("ive");
      if ((j > 0) && (word.CharAt(j - 1) == 'a') && (word.CharAt(j) == 't')) {
        word.SetCharAt(j - 1, 'e'); /* try removing -ative and adding -e */
        word.SetLength(j); /* (e.g., determinative -> determine) */
        k = j - 1;
        if (lookup()) return;
        word.SetLength(j - 1); /* try just removing -ative */
        if (lookup()) return;

        word.Append("ative");
        k = old_k;
      }

      /* try mapping -ive to -ion (e.g., injunctive/injunction) */
      word.SetCharAt(j + 2, 'o');
      word.SetCharAt(j + 3, 'n');
      if (lookup()) return;

      word.SetCharAt(j + 2, 'v'); /* restore the original values */
      word.SetCharAt(j + 3, 'e');
      k = old_k;
      // nolookup()
    }
    return;
  }

  public KStemmer() {}

  /// <summary>True when <paramref name="word"/> is an entry of the KStem dictionary (Lucene's dict_ht lookup).</summary>
  public static bool InDictionary(string word) {
    return !string.IsNullOrEmpty(word) && DictGet(word.AsSpan()) is not null;
  }

  /// <summary>The KStem stem of <paramref name="term"/> (the term itself when unchanged).</summary>
  public string Stem(string term) {
    bool changed = Stem(term.ToCharArray(), term.Length);
    if (!changed) return term;
    return asString();
  }

  /** Returns the result of the stem (assuming the word was changed) as a string. */
  internal string asString() {
    string s = getString();
    if (s is not null) return s;
    return word.ToString();
  }


  private string getString() {
    return result;
  }



  private string result;

  private bool matched() {
    /*
     * if (!lookups.contains(word.ToString())) { throw new
     * RuntimeException("didn't look up "+word.ToString()+" prev="+prevLookup);
     * }
     */
    // lookup();
    return matchedEntry is not null;
  }

  /** Stems the text in the token. Returns true if changed. */
  internal bool Stem(char[] term, int len) {

    result = null;

    k = len - 1;
    if ((k <= 1) || (k >= MaxWordLen - 1)) {
      return false; // don't stem
    }

    // first check the stemmer dictionaries, and avoid using the
    // cache if it's in there.
    DictEntry entry = DictGet(new ReadOnlySpan<char>(term, 0, len));
    if (entry is not null) {
      if (entry.root is not null) {
        result = entry.root;
        return true;
      }
      return false;
    }

    /*
     * caching off is normally faster if (cache is null) initializeStemHash();
     *
     * // now check the cache, before we copy chars to "word" if (cache is not null)
     * { string val = cache.get(term, 0, len); if (val is not null) { if (val !=
     * SAME) { result = val; return true; } return false; } }
     */

    word.Reset();
    // allocate enough space so that an expansion is never needed
    word.Reserve(len + 10);
    for (int i = 0; i < len; i++) {
      char ch = term[i];
      if (!isAlpha(ch)) return false; // don't stem
      // don't lowercase... it's a requirement that lowercase filter be
      // used before this stemmer.
      word.UnsafeWrite(ch);
    }

    matchedEntry = null;
    /*
     * lookups.clear(); lookups.add(word.ToString());
     */

    /*
     * This while loop will never be executed more than one time; it is here
     * only to allow the break statement to be used to escape as soon as a word
     * is recognized
     */
    while (true) {
      // YCS: extra lookup()s were inserted so we don't need to
      // do an extra wordInDict() here.
      plural();
      if (matched()) break;
      pastTense();
      if (matched()) break;
      aspect();
      if (matched()) break;
      ityEndings();
      if (matched()) break;
      nessEndings();
      if (matched()) break;
      ionEndings();
      if (matched()) break;
      erAndOrEndings();
      if (matched()) break;
      lyEndings();
      if (matched()) break;
      alEndings();
      if (matched()) break;
      entry = wordInDict();
      iveEndings();
      if (matched()) break;
      izeEndings();
      if (matched()) break;
      mentEndings();
      if (matched()) break;
      bleEndings();
      if (matched()) break;
      ismEndings();
      if (matched()) break;
      icEndings();
      if (matched()) break;
      ncyEndings();
      if (matched()) break;
      nceEndings();
      matched();
      break;
    }

    /*
     * try for a direct mapping (allows for cases like `Italian'->`Italy' and
     * `Italians'->`Italy')
     */
    entry = matchedEntry;
    if (entry is not null) {
      result = entry.root; // may be null, which means that "word" is the stem
    }

    /*
     * caching off is normally faster if (cache is not null && cache.size() <
     * maxCacheSize) { char[] key = new char[len]; System.arraycopy(term, 0,
     * key, 0, len); if (result is not null) { cache.put(key, result); } else {
     * cache.put(key, word.ToString()); } }
     */

    /*
     * if (entry is null) { if (!word.ToString().equals(new string(term,0,len)))
     * { System.out.println("CASE:" + word.ToString() + "," + new
     * string(term,0,len));
     *
     * } }
     */

    // no entry matched means result is "word"
    return true;
  }
}
}
