using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace MmdMapMaid.Controls;

public class BindableRichEditBox : RichEditBox
{
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register(
        "Text", typeof(string), typeof(BindableRichEditBox),
        new PropertyMetadata(default(string), TextPropertyChanged));

    private bool _lockChangeExecution;

    public BindableRichEditBox()
    {
        SelectionFlyout = null;
        TextChanged += BindableRichEditBox_TextChanged;
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private void BindableRichEditBox_TextChanged(object sender, RoutedEventArgs e)
    {
        if (!_lockChangeExecution)
        {
            _lockChangeExecution = true;
            Document.GetText(TextGetOptions.None, out var text);
            Text = text.Replace("\r", "");
            _lockChangeExecution = false;
        }
    }

    private static void TextPropertyChanged(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var rtb = dependencyObject as BindableRichEditBox;
        if (rtb == null) return;
        if (!rtb._lockChangeExecution)
        {
            rtb._lockChangeExecution = true;
            rtb.Document.SetText(TextSetOptions.None, rtb.Text);
            rtb._lockChangeExecution = false;
        }
    }
}