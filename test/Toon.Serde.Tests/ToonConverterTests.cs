using Newtonsoft.Json.Linq;
using Xunit;

namespace Toon.Serde.Tests
{
    public class ToonConverterTests
    {
        [Fact]
        public void SimpleObject_IsDeterministicAndEscapes()
        {
            var json = JObject.Parse(@"{""b"":2,""a"":1,""text"":""hello world""}");
            var toon = ToonConverter.ToToon(json, new ToonOptions { Indent = 2, SafeStrings = true, StableKeys = true });

            Assert.Contains("a: 1", toon);
            Assert.Contains("b: 2", toon);
            Assert.Contains("\"hello world\"", toon);
        }

        //[Fact]
        //public void EmptyObject_Compact()
        //{
        //    var json = JObject.Parse("{}");
        //    var toon = ToonConverter.ToToon(json, new ToonOptions { Indent = 0 });
        //    Assert.Equal("{}", toon.Trim());
        //}

        [Fact]
        public void Canonical_UserTags_Example()
        {
            var json = JObject.Parse(@"{""user"":{""id"":123,""name"":""Akira""},""tags"":[""agent"",""llm""]}");
            var toon = ToonConverter.ToToon(json, new ToonOptions { Indent = 2, SafeStrings = true, StableKeys = false });

            // ✅ DO NOT escape backslashes here — just use regular C# escapes
            var expected = "user:\n  id: 123\n  name: Akira\ntags[2]:\n  agent\n  llm";

            // ✅ This line is perfectly valid C#
            Assert.Equal(expected, toon.Replace("\r\n", "\n").TrimEnd());
        }

    }
}
