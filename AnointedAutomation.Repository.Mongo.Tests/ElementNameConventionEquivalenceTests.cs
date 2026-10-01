// Copyright © Anointed Automation, LLC., 2026. All Rights Reserved. Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️
// The Mongo conventions now delegate to NamingRules; these pin their observable output.

using AnointedAutomation.Repository.Mongo;
using MongoDB.Bson.Serialization;
using Xunit;

namespace ConventionProbe
{
    public class Sample
    {
        public string URLValue { get; set; }
        public int IDNumber { get; set; }
        public string LineItems { get; set; }
        public int? Address1 { get; set; }
    }

    public struct SampleStruct
    {
        public string Label { get; set; }
    }

    public class ElementNameConventionEquivalenceTests
    {
        private static string Hybrid(System.Type t, string member)
        {
            BsonClassMap cm = new BsonClassMap(t);
            cm.AutoMap();
            BsonMemberMap mm = cm.GetMemberMap(member);
            new HybridElementNameConvention().Apply(mm);
            return mm.ElementName;
        }

        private static string Snake(System.Type t, string member)
        {
            BsonClassMap cm = new BsonClassMap(t);
            cm.AutoMap();
            BsonMemberMap mm = cm.GetMemberMap(member);
            new SnakeCaseElementNameConvention().Apply(mm);
            return mm.ElementName;
        }

        [Fact]
        public void Hybrid_UsesFirstCharCamel_AndPascal()
        {
            Assert.Equal("URLValue", Hybrid(typeof(Sample), "URLValue"));
            Assert.Equal("iDNumber", Hybrid(typeof(Sample), "IDNumber"));
            Assert.Equal("address1", Hybrid(typeof(Sample), "Address1"));
            Assert.Equal("label", Hybrid(typeof(SampleStruct), "Label"));
        }

        [Fact]
        public void Snake_Converts()
        {
            Assert.Equal("url_value", Snake(typeof(Sample), "URLValue"));
            Assert.Equal("line_items", Snake(typeof(Sample), "LineItems"));
            Assert.Equal("id_number", Snake(typeof(Sample), "IDNumber"));
            Assert.Equal("line_items", SnakeCaseElementNameConvention.ToSnake("LineItems"));
        }

        [Fact]
        public void RenamedElements_AreLeftAlone()
        {
            BsonClassMap cm = new BsonClassMap(typeof(Sample));
            cm.AutoMap();
            BsonMemberMap mm = cm.GetMemberMap("LineItems");
            mm.SetElementName("li");
            new HybridElementNameConvention().Apply(mm);
            Assert.Equal("li", mm.ElementName);
            new SnakeCaseElementNameConvention().Apply(mm);
            Assert.Equal("li", mm.ElementName);
        }
    }
}
