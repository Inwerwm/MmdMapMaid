using System.Text.Json;
using MmdMapMaid.Helpers;

namespace Test;
public class JsonTest
{
    record Rec(int I, string S);

    [Test]
    public void NonStringKeyDictionaryConverterTest()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new NonStringKeyDictionaryConverter<Rec, string[]>());
        var testDict = new Dictionary<Rec, string[]> { { new(1, "a"), new[] { "one", "one" } }, { new(2, "b"), new[] { "two", "two" } }, { new(3, "c"), new[] { "three", "three" } } };
        var serializedDict = JsonSerializer.Serialize(testDict, options);
        Console.WriteLine(serializedDict);
    }
}
