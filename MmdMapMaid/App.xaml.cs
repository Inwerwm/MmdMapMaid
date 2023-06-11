using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;

using MmdMapMaid.Activation;
using MmdMapMaid.Contracts.Services;
using MmdMapMaid.Core.Contracts.Services;
using MmdMapMaid.Core.Services;
using MmdMapMaid.FeatureState;
using MmdMapMaid.Models;
using MmdMapMaid.Services;
using MmdMapMaid.ViewModels;
using MmdMapMaid.Views;

namespace MmdMapMaid;

// To learn more about WinUI 3, see https://docs.microsoft.com/windows/apps/winui/winui3/.
public partial class App : Application
{
    // The .NET Generic Host provides dependency injection, configuration, logging, and other services.
    // https://docs.microsoft.com/dotnet/core/extensions/generic-host
    // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
    // https://docs.microsoft.com/dotnet/core/extensions/configuration
    // https://docs.microsoft.com/dotnet/core/extensions/logging
    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Default Activation Handler
            services.AddTransient<ActivationHandler<LaunchActivatedEventArgs>, DefaultActivationHandler>();

            // Other Activation Handlers

            // Services
            services.AddSingleton<ILocalSettingsService, LocalSettingsService>();
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();
            services.AddTransient<INavigationViewService, NavigationViewService>();

            services.AddSingleton<IActivationService, ActivationService>();
            services.AddSingleton<IPageService, PageService>();
            services.AddSingleton<INavigationService, NavigationService>();

            // Core Services
            services.AddSingleton<IFileService, FileService>();

            // Views and ViewModels
            services.AddSingleton<VmdRangeEditViewModel>();
            services.AddTransient<VmdRangeEditPage>();
            services.AddSingleton<MorphInterpolationViewModel>();
            services.AddTransient<MorphInterpolationPage>();
            services.AddTransient<MotionLoopViewModel>();
            services.AddTransient<MotionLoopPage>();
            services.AddTransient<ReplaceVmdViewModel>();
            services.AddTransient<ReplaceVmdPage>();
            services.AddTransient<ExtractVmdViewModel>();
            services.AddTransient<ExtractVmdPage>();
            services.AddTransient<ExtractEmdViewModel>();
            services.AddTransient<ExtractEmdPage>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsPage>();
            services.AddTransient<EmmViewModel>();
            services.AddTransient<EmmPage>();
            services.AddTransient<PmmViewModel>();
            services.AddTransient<PmmPage>();
            services.AddTransient<ShellPage>();
            services.AddTransient<ShellViewModel>();

            services.AddTransient<ReplacementBoxViewModel>();

            // Configuration
            services.Configure<LocalSettingsOptions>(context.Configuration.GetSection(nameof(LocalSettingsOptions)));

            // States
            services.AddSingleton<PmmReplacerState>();
            services.AddSingleton<VmdReplacerState>();
            services.AddSingleton<VmdExtractorState>();
            services.AddSingleton<EmmOrderMapperState>();
            services.AddSingleton<EmdExtractorState>();
            services.AddSingleton<VmdMotionLoopState>();
        }).
        Build();

        UnhandledException += App_UnhandledException;
    }

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        // TODO: Log and handle exceptions as appropriate.
        // https://docs.microsoft.com/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.application.unhandledexception.
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        base.OnLaunched(args);

        await App.GetService<IActivationService>().ActivateAsync(args);
    }
}
