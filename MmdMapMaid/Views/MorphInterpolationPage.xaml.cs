﻿using Microsoft.UI.Xaml.Controls;

using MmdMapMaid.ViewModels;

namespace MmdMapMaid.Views;

public sealed partial class MorphInterpolationPage : Page
{
    public MorphInterpolationViewModel ViewModel
    {
        get;
    }

    public MorphInterpolationPage()
    {
        ViewModel = App.GetService<MorphInterpolationViewModel>();
        InitializeComponent();
    }
}
