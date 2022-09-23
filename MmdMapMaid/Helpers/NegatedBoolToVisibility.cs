using CommunityToolkit.WinUI.UI.Converters;
using Microsoft.UI.Xaml.Data;

namespace MmdMapMaid.Helpers;

internal class NegatedBoolToVisibility : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language) =>
        new BoolToVisibilityConverter().Convert(value is bool b && !b, targetType, parameter, language);
    public object ConvertBack(object value, Type targetType, object parameter, string language) =>
        new BoolToVisibilityConverter().Convert(value is bool b && !b, targetType, parameter, language);
}
