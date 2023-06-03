using Microsoft.UI.Xaml.Controls;

namespace MmdMapMaid.Helpers;
internal static class Factory
{
    public static Action<string> CreateLogWriter(this TextBox textBox) =>
        (log) => textBox.Text = $"""
            {DateTime.Now}
            {log}

            {textBox.Text}
            """;
}
