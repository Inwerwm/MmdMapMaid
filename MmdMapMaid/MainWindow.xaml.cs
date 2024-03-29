﻿using MmdMapMaid.Helpers;

namespace MmdMapMaid;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/maid.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
    }
}
