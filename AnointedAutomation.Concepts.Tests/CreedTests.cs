// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me on 2026-10-03 Jesus is King ✝️
// Stewarded by Alexander Fields

using System.Collections.Generic;
using Xunit;
using AnointedAutomation.Concepts;

namespace AnointedAutomation.Concepts.Tests
{
    public class CreedTests
    {
        private static List<System.Type> ConcreteTypes(System.Type baseType)
        {
            List<System.Type> types = new List<System.Type>();
            foreach (System.Type type in baseType.Assembly.GetTypes())
            {
                if (!type.IsAbstract && baseType.IsAssignableFrom(type))
                {
                    types.Add(type);
                }
            }

            return types;
        }

        [Fact]
        public void Json_ContainsEveryMoralConcept()
        {
            string json = Creed.ToJson();
            List<System.Type> types = ConcreteTypes(typeof(MoralConcept));

            Assert.True(types.Count > 100);
            Assert.Equal(types.Count, Creed.MoralConcepts().Count);
            foreach (System.Type type in types)
            {
                MoralConcept concept = (MoralConcept)System.Activator.CreateInstance(type);
                Assert.Contains("\"name\": \"" + concept.Name + "\"", json);
                Assert.Contains("\"scripture\": \"" + concept.Scripture + "\"", json);
            }
        }

        [Fact]
        public void JsonAndPrompt_ContainEveryAttribute()
        {
            string json = Creed.ToJson();
            string prompt = Creed.ToPrompt();
            List<System.Type> types = ConcreteTypes(typeof(DivineAttribute));

            Assert.Equal(types.Count, Creed.Attributes().Count);
            foreach (System.Type type in types)
            {
                DivineAttribute attribute = (DivineAttribute)System.Activator.CreateInstance(type);
                Assert.Contains("\"" + attribute.Name + "\"", json);
                Assert.Contains(attribute.Name, prompt);
            }
        }

        [Fact]
        public void Prompt_ContainsEveryMoralConceptRow()
        {
            string prompt = Creed.ToPrompt();

            foreach (MoralConcept concept in Creed.MoralConcepts())
            {
                Assert.Contains("| " + concept.Name + " | ", prompt);
            }
        }

        [Fact]
        public void Json_IsDeterministicAndValid()
        {
            string first = Creed.ToJson();
            string second = Creed.ToJson();

            Assert.Equal(first, second);
            using (System.Text.Json.JsonDocument document = System.Text.Json.JsonDocument.Parse(first))
            {
                System.Text.Json.JsonElement god = document.RootElement.GetProperty("god");
                Assert.Equal(3, god.GetProperty("persons").GetArrayLength());
                Assert.True(god.GetProperty("oneEssence").GetBoolean());
                Assert.True(god.GetProperty("personsDistinct").GetBoolean());
                Assert.Equal(Love.MaxCompleteness, document.RootElement.GetProperty("love").GetProperty("properties").GetArrayLength());
                Assert.Equal(Creed.DecisionRules.Count, document.RootElement.GetProperty("decisionRules").GetArrayLength());
            }
        }

        [Fact]
        public void Prompt_IsDeterministic_AndNamesTheTrinityAndGate()
        {
            string prompt = Creed.ToPrompt();

            Assert.Equal(prompt, Creed.ToPrompt());
            Assert.Contains("the Father", prompt);
            Assert.Contains("the Son, the Word", prompt);
            Assert.Contains("the Holy Spirit", prompt);
            Assert.Contains("Deuteronomy 30:19", prompt);
            Assert.Contains("condilectio", prompt);
        }

        [Fact]
        public void KindOf_ClassifiesConcepts()
        {
            Assert.Equal("virtue", Creed.KindOf(new Compassion()));
            Assert.Equal("sin", Creed.KindOf(new Idolatry()));
            Assert.Equal("mystery", Creed.KindOf(new Incarnation()));
            Assert.Throws<System.ArgumentNullException>(() => Creed.KindOf(null));
        }

        [Fact]
        public void Output_HasNoLongDashes()
        {
            string all = Creed.ToJson() + Creed.ToPrompt();

            Assert.DoesNotContain("\u2014", all);
            Assert.DoesNotContain("\u2013", all);
        }
    }
}
