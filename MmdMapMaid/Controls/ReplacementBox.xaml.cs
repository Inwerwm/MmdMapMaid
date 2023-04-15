// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CommunityToolkit.WinUI.UI;
using MmdMapMaid.Helpers;
using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Controls;

public sealed partial class ReplacementBox : UserControl
{
    public static readonly DependencyProperty TextListViewProperty =
        DependencyProperty.Register(
            "TextListView",
            typeof(ListView),
            typeof(ReplacementBox),
            new PropertyMetadata(default(ListView), TextListViewPropertyChanged)
        );

    public ListView TextListView
    {
        get => (ListView)GetValue(TextListViewProperty);
        set => SetValue(TextListViewProperty, value);
    }

    private static void TextListViewPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        var box = (ReplacementBox)dependencyObject;
        box.UpdateRichEditBoxes();
        box.RegisterUpdatingRichEditBoxes();
    }

    public ReplacementBoxViewModel ViewModel
    {
        get;
        set;
    }

    public ReplacementBox()
    {
        ViewModel = App.GetService<ReplacementBoxViewModel>();
        this.InitializeComponent();

        if (TextListView != null)
        {
            RegisterUpdatingRichEditBoxes();
        }
    }

    private void RegisterUpdatingRichEditBoxes() => TextListView.LayoutUpdated += (_, _) => UpdateRichEditBoxes();
    private void UpdateRichEditBoxes() => ViewModel.RichEditBoxes = TextListView.FindDescendants().OfType<BindableRichEditBox>().ToArray();

    private void Search() => SearchHelpers.HighlightSearch(this, ViewModel.RichEditBoxes, ViewModel.SearchQuery, UseRegex.IsOn);
    private void SearchQueryChanged(object sender, TextChangedEventArgs e) => Search();
    private void UseRegex_Toggled(object sender, RoutedEventArgs e) => Search();
}
