// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

using AnointedAutomation.Shopify.Converters;
using Newtonsoft.Json;
using Xunit;

namespace AnointedAutomation.Shopify.Tests
{
    public class FalseToNullConverterTests
    {
        private sealed class Holder
        {
            [JsonConverter(typeof(FalseToNullConverter))]
            public bool? Flag { get; set; }
        }

        [Theory]
        [InlineData("{\"Flag\":null}", false)]
        [InlineData("{\"Flag\":\"\"}", false)]
        [InlineData("{\"Flag\":true}", true)]
        [InlineData("{\"Flag\":\"false\"}", false)]
        public void ReadJson_MapsNullAndEmptyToFalse(string json, bool expected)
        {
            Holder holder = JsonConvert.DeserializeObject<Holder>(json);

            Assert.Equal(expected, holder.Flag);
        }

        [Fact]
        public void ReadJson_ThrowsOnUnparseableValue()
        {
            Assert.Throws<JsonReaderException>(() => JsonConvert.DeserializeObject<Holder>("{\"Flag\":\"maybe\"}"));
        }

        [Theory]
        [InlineData(false, "{\"Flag\":null}")]
        [InlineData(true, "{\"Flag\":true}")]
        public void WriteJson_WritesFalseAsNull(bool value, string expected)
        {
            Assert.Equal(expected, JsonConvert.SerializeObject(new Holder { Flag = value }));
        }
    }
}
