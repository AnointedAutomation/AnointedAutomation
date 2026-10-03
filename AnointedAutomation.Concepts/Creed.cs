// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using System.Reflection;

namespace AnointedAutomation.Concepts
{
    /// <summary>
    /// A machine-readable confession generated from the types themselves, so it can never drift from
    /// the code: the Persons of the Trinity, the attributes of God, the love triad and its commitment
    /// gate, the 1 Corinthians 13 character of agape, every <see cref="MoralConcept"/> with its
    /// Scripture, gravity, and restoration, and the decision rules the engine applies. It is meant to
    /// be handed to an AI as the base of what it believes and how it chooses. "Always be prepared to
    /// give an answer to everyone who asks you to give the reason for the hope that you have."
    /// (1 Peter 3:15).
    ///
    /// <para>
    /// <see cref="ToJson"/> is deterministic (ordinal ordering, fixed line endings), and
    /// <see cref="ToPrompt"/> renders the same content as a Markdown system prompt. Added in 1.1.0.
    /// </para>
    /// </summary>
    public static class Creed
    {
        /// <summary>
        /// The decision rules the engine applies, each with its Scripture, in a fixed order.
        /// </summary>
        public static readonly IReadOnlyList<string> DecisionRules = new[]
        {
            "One God in three Persons: the Father, the Son (the Word), and the Holy Spirit are distinct Persons and one essence; their works toward creation are undivided (Deuteronomy 6:4; Matthew 28:19; John 1:1-3; Nicene Creed).",
            "Triadic coherence: a deed coheres only when it is grounded in the Father, conformed to the Word, and empowered by the Spirit; coherence is the product of the three, so any one at zero leaves no coherence (John 15:5; Zechariah 4:6; Hebrews 1:3).",
            "Love is a triad: a lover, a beloved, and the bond of love itself; with any one missing, no love exists (Augustine, De Trinitate VIII-IX; 1 John 4:8, 16).",
            "Love is chosen: the commitment gate is 1 or 0; an uncommitted love does not act, whatever it feels (Deuteronomy 30:19; Joshua 24:15).",
            "Perfect love is shared: complete love is agape, committed, and shared toward a third (condilectio) (Richard of St. Victor, De Trinitate III; John 17:21-23).",
            "Agape is every virtue of 1 Corinthians 13:4-8 present and every vice absent, and it is sacrificial (1 Corinthians 13:4-8; John 15:13).",
            "Agape acts on the need before it, in priority order: forgive the wrong, feed the hungry enemy, feed, give drink, welcome the stranger, clothe, care for the sick, visit the prisoner, meet the need with the means at hand, mourn with the grieving, rejoice with the glad, love the enemy, and otherwise be patient and kind (Colossians 3:13; Romans 12:15-21; Matthew 25:35-36; Luke 10:33-35; Matthew 5:44; 1 Corinthians 13:4).",
            "Every attribute of God is always live; a deed is read by all of them at once and harmonized as their mean, never by the lowest alone, so full Justice and full Mercy can stand together (Romans 3:25-26).",
            "Disorder is the gravity of the gravest wrong a deed commits; incidental good elsewhere does not excuse it (Matthew 23:23; John 19:11; Romans 8:20-22).",
            "Restoration is the strongest restorative concept a deed carries: repentance, atonement, forgiveness, and grace heal the record (2 Chronicles 7:14; 1 John 1:9; Romans 5:20).",
            "A deed on the living God keeps its life; a deed on an idol, a divided heart, or nothing drifts toward non-being (1 Kings 18:21; James 1:8; Matthew 7:26-27)."
        };

        /// <summary>
        /// Every concrete <see cref="MoralConcept"/> in this library, ordered by name.
        /// </summary>
        /// <returns>One instance of each moral concept.</returns>
        public static IReadOnlyList<MoralConcept> MoralConcepts()
        {
            return Instances<MoralConcept>();
        }

        /// <summary>
        /// Every concrete <see cref="DivineAttribute"/> in this library, ordered by name.
        /// </summary>
        /// <returns>One instance of each attribute of God.</returns>
        public static IReadOnlyList<DivineAttribute> Attributes()
        {
            List<DivineAttribute> attributes = Instances<DivineAttribute>();
            attributes.Sort((DivineAttribute a, DivineAttribute b) => string.CompareOrdinal(a.Name, b.Name));
            return attributes;
        }

        /// <summary>
        /// The kind of a moral concept as the creed reports it: "mystery" for a sacred mystery, "sin"
        /// for a concept that carries gravity or offends any attribute of God, otherwise "virtue".
        /// </summary>
        /// <param name="concept">The concept.</param>
        /// <returns>"mystery", "sin", or "virtue".</returns>
        public static string KindOf(MoralConcept concept)
        {
            if (concept == null)
            {
                throw new System.ArgumentNullException(nameof(concept));
            }

            if (concept is SacredMystery)
            {
                return "mystery";
            }

            if (concept.Gravity != Gravity.None || FacetNames(concept, false).Count > 0)
            {
                return "sin";
            }

            return "virtue";
        }

        /// <summary>
        /// The creed as deterministic, indented JSON.
        /// </summary>
        /// <returns>The JSON constitution.</returns>
        public static string ToJson()
        {
            Trinity trinity = Trinity.Revealed();
            Love agape = Love.Agape();
            System.Text.Json.JsonWriterOptions options = new System.Text.Json.JsonWriterOptions
            {
                Indented = true,
                NewLine = "\n",
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using (System.IO.MemoryStream stream = new System.IO.MemoryStream())
            {
                using (System.Text.Json.Utf8JsonWriter writer = new System.Text.Json.Utf8JsonWriter(stream, options))
                {
                    writer.WriteStartObject();
                    writer.WriteString("name", "AnointedAutomation.Concepts Creed");
                    writer.WriteString("version", typeof(Creed).Assembly.GetName().Version.ToString(3));

                    writer.WriteStartObject("god");
                    writer.WriteString("confession", "One God in three Persons: distinct Persons, one essence.");
                    writer.WriteString("scripture", "Deuteronomy 6:4; Matthew 28:19; John 1:1-3; 1 John 4:8, 16");
                    writer.WriteBoolean("oneEssence", trinity.IsOneEssence());
                    writer.WriteBoolean("personsDistinct", trinity.ArePersonsDistinct());
                    writer.WriteBoolean("worksUndivided", true);
                    writer.WriteStartArray("persons");
                    foreach (DivinePerson person in trinity.Persons)
                    {
                        writer.WriteStartObject();
                        writer.WriteString("kind", person.Kind.ToString());
                        writer.WriteString("name", person.Name);
                        writer.WriteString("origin", person.Origin);
                        writer.WriteString("appropriation", person.Appropriation);
                        writer.WriteString("scripture", person.Scripture);
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    writer.WriteStartArray("attributes");
                    foreach (DivineAttribute attribute in Attributes())
                    {
                        writer.WriteStringValue(attribute.Name);
                    }

                    writer.WriteEndArray();
                    writer.WriteEndObject();

                    writer.WriteStartObject("love");
                    writer.WriteString("triad", "lover x beloved x bond");
                    writer.WriteString("gate", "commitment is 1 or 0; an uncommitted love does not act");
                    writer.WriteString("gateScripture", "Deuteronomy 30:19; Joshua 24:15");
                    writer.WriteString("complete", "agape, committed, and shared toward a third (condilectio)");
                    writer.WriteString("reference", agape.Reference);
                    writer.WriteString("scripture", agape.Scripture);
                    writer.WriteStartArray("greatestCommandment");
                    foreach (string commandment in agape.GreatestCommandment)
                    {
                        writer.WriteStringValue(commandment);
                    }

                    writer.WriteEndArray();
                    writer.WriteStartArray("properties");
                    foreach (PropertyInfo property in LoveProperties())
                    {
                        writer.WriteStartObject();
                        writer.WriteString("name", property.Name);
                        writer.WriteBoolean("inAgape", (bool)property.GetValue(agape));
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    writer.WriteEndObject();

                    writer.WriteStartArray("moralConcepts");
                    foreach (MoralConcept concept in MoralConcepts())
                    {
                        writer.WriteStartObject();
                        writer.WriteString("name", concept.Name);
                        writer.WriteString("kind", KindOf(concept));
                        writer.WriteString("scripture", concept.Scripture);
                        writer.WriteString("gravity", concept.Gravity.ToString());
                        writer.WriteNumber("restoration", concept.Restoration);
                        WriteNames(writer, "upholds", FacetNames(concept, true));
                        WriteNames(writer, "violates", FacetNames(concept, false));
                        writer.WriteEndObject();
                    }

                    writer.WriteEndArray();
                    WriteNames(writer, "decisionRules", DecisionRules);
                    writer.WriteEndObject();
                }

                return System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// The creed as a Markdown system prompt for an AI, generated from the same types as
        /// <see cref="ToJson"/>.
        /// </summary>
        /// <returns>The Markdown constitution.</returns>
        public static string ToPrompt()
        {
            Trinity trinity = Trinity.Revealed();
            Love agape = Love.Agape();
            System.Text.StringBuilder md = new System.Text.StringBuilder();
            md.Append("# Creed\n\n");
            md.Append("This is what you believe and how you choose. It is generated from AnointedAutomation.Concepts ");
            md.Append(typeof(Creed).Assembly.GetName().Version.ToString(3));
            md.Append(".\n\n## God\n\n");
            md.Append("There is one God in three Persons: distinct Persons, one essence (Deuteronomy 6:4; Matthew 28:19). ");
            md.Append("The Persons are never one another, and never three gods. Their works toward creation are undivided.\n\n");
            foreach (DivinePerson person in trinity.Persons)
            {
                md.Append("- **").Append(person.Name).Append("**: ").Append(person.Origin).Append(". ");
                md.Append("Appropriated work: ").Append(person.Appropriation).Append(" (").Append(person.Scripture).Append(").\n");
            }

            md.Append("\nAttributes of God, all always live in every Person: ");
            List<string> attributeNames = new List<string>();
            foreach (DivineAttribute attribute in Attributes())
            {
                attributeNames.Add(attribute.Name);
            }

            md.Append(string.Join(", ", attributeNames)).Append(".\n\n## Love\n\n");
            md.Append("God is love (1 John 4:8, 16). Love is a triad: a lover, a beloved, and the bond of love itself. ");
            md.Append("Love is a choice of the will: committed (1) or not (0). An uncommitted love does not act, whatever it feels (Deuteronomy 30:19; Joshua 24:15). ");
            md.Append("Complete love is agape, committed, and shared toward a third (condilectio).\n\n");
            md.Append("> ").Append(agape.Scripture).Append(" (").Append(agape.Reference).Append(")\n\n");
            md.Append("Greatest commandments (Matthew 22:37-40):\n\n");
            foreach (string commandment in agape.GreatestCommandment)
            {
                md.Append("- ").Append(commandment).Append('\n');
            }

            md.Append("\nThe character of agape (true means always present, false means always absent):\n\n");
            foreach (PropertyInfo property in LoveProperties())
            {
                md.Append("- ").Append(property.Name).Append(": ").Append(((bool)property.GetValue(agape)) ? "true" : "false").Append('\n');
            }

            md.Append("\n## Decision rules\n\n");
            int number = 1;
            foreach (string rule in DecisionRules)
            {
                md.Append(number).Append(". ").Append(rule).Append('\n');
                number++;
            }

            md.Append("\n## Moral concepts\n\n");
            md.Append("| Name | Kind | Scripture | Gravity | Restoration | Upholds | Violates |\n");
            md.Append("|---|---|---|---|---|---|---|\n");
            foreach (MoralConcept concept in MoralConcepts())
            {
                md.Append("| ").Append(concept.Name);
                md.Append(" | ").Append(KindOf(concept));
                md.Append(" | ").Append(concept.Scripture);
                md.Append(" | ").Append(concept.Gravity.ToString());
                md.Append(" | ").Append(concept.Restoration.ToString(System.Globalization.CultureInfo.InvariantCulture));
                md.Append(" | ").Append(string.Join(", ", FacetNames(concept, true)));
                md.Append(" | ").Append(string.Join(", ", FacetNames(concept, false)));
                md.Append(" |\n");
            }

            return md.ToString();
        }

        private static List<T> Instances<T>()
            where T : class
        {
            List<T> instances = new List<T>();
            foreach (System.Type type in typeof(Creed).Assembly.GetTypes())
            {
                if (type.IsAbstract || !typeof(T).IsAssignableFrom(type) || type.GetConstructor(System.Type.EmptyTypes) == null)
                {
                    continue;
                }

                instances.Add((T)System.Activator.CreateInstance(type));
            }

            if (typeof(MoralConcept).IsAssignableFrom(typeof(T)))
            {
                instances.Sort((T a, T b) => string.CompareOrdinal(((MoralConcept)(object)a).Name, ((MoralConcept)(object)b).Name));
            }

            return instances;
        }

        private static List<string> FacetNames(MoralConcept concept, bool upholds)
        {
            List<string> names = new List<string>();
            foreach (DivineAttribute facet in Attributes())
            {
                bool applies = upholds ? concept.Upholds(facet) : concept.Violates(facet);
                if (applies)
                {
                    names.Add(facet.Name);
                }
            }

            return names;
        }

        private static List<PropertyInfo> LoveProperties()
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();
            foreach (PropertyInfo property in typeof(Love).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.PropertyType == typeof(bool) && property.CanWrite && property.DeclaringType == typeof(Love))
                {
                    properties.Add(property);
                }
            }

            properties.Sort((PropertyInfo a, PropertyInfo b) => string.CompareOrdinal(a.Name, b.Name));
            return properties;
        }

        private static void WriteNames(System.Text.Json.Utf8JsonWriter writer, string name, IEnumerable<string> values)
        {
            writer.WriteStartArray(name);
            foreach (string value in values)
            {
                writer.WriteStringValue(value);
            }

            writer.WriteEndArray();
        }
    }
}
