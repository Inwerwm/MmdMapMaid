using CommunityToolkit.WinUI.UI;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MmdMapMaid.Controls;
using MmdMapMaid.ViewModels;
using Windows.Storage;
using Windows.UI;

namespace MmdMapMaid.Views;

public sealed partial class PmmPage : Page
{
    public PmmViewModel ViewModel
    {
        get;
    }

    public PmmPage()
    {
        ViewModel = App.GetService<PmmViewModel>();
        InitializeComponent();

        ViewModel.OnPathChanged += (_, _) =>
        {
            Search();
        };
    }

    private void SearchQueryChanged(object sender, TextChangedEventArgs e)
    {
        Search();
    }

    private void Search()
    {
        var theme = (Content as FrameworkElement)!.ActualTheme;

        foreach (BindableRichEditBox pathBox in PathsListView.FindDescendants().Where(element => element is BindableRichEditBox))
        {
            var range = pathBox.Document.GetRange(0, TextConstants.MaxUnitCount);
            range.CharacterFormat.BackgroundColor = ((SolidColorBrush)pathBox.Background).Color;

            var query = ViewModel.SearchQuery;
            if (string.IsNullOrEmpty(query)) { continue; }

            while (range.FindText(query, TextConstants.MaxUnitCount, FindOptions.None) > 0)
            {
                range.CharacterFormat.BackgroundColor = theme switch
                {
                    ElementTheme.Light => Color.FromArgb(0x80, 0xe0, 0xe0, 0xe0),
                    ElementTheme.Dark => Color.FromArgb(0x80, 0x50, 0x50, 0x50),
                    _ => Color.FromArgb(0x80, 0x80, 0x80, 0x80),
                };
            }
        }
    }

    private void ContentArea_DragOver(object _, DragEventArgs e)
    {
        e.AcceptedOperation = Windows.ApplicationModel.DataTransfer.DataPackageOperation.Copy;
    }

    private async void ContentArea_Drop(object sender, DragEventArgs e)
    {
        var droped = await e.DataView.GetStorageItemsAsync();
        var item = droped.FirstOrDefault(file => Path.GetExtension(file.Name) == ".pmm");
        if (item is not StorageFile pmm) { return; }

        ViewModel.ReadPmm(pmm);
    }
}
