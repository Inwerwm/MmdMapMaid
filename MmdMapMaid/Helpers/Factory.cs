using System.Text.Json;
using Microsoft.UI.Xaml.Controls;
using MmdMapMaid.Models;

namespace MmdMapMaid.Helpers;
internal static class Factory
{
    public static JsonSerializerOptions CreateJsonSerializerOptions()
    {
        var options = new JsonSerializerOptions()
        {
            WriteIndented = true
        };
        options.Converters.Add(new NonStringKeyDictionaryConverter<PathInformation, string[]>());

        return options;
    }

    public static Action<string> CreateLogWriter(this TextBox textBox) =>
        (log) => textBox.Text = $"""
            {DateTime.Now}
            {log}

            {textBox.Text}
            """;
}
