﻿using AtmosphereTool.Contracts.Services;
using AtmosphereTool.ViewModels;
using AtmosphereTool.Views;
using AtmosphereTool.FeaturePages;

using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Controls;

namespace AtmosphereTool.Services;

public class PageService : IPageService
{
    private readonly Dictionary<string, Type> _pages = new();

    public PageService()
    {
        Configure<MainViewModel, MainPage>();
        Configure<WindowsSettingsViewModel, WindowsSettingsPage>();
        Configure<SettingsViewModel, SettingsPage>();
        Configure<AtmosphereSettingsViewModel, AtmosphereSettingsPage>();
        // You have to add namespace if you want to register without viewmodel
        Configure("AtmosphereTool.FeaturePages.GeneralConfig", typeof(GeneralConfig));
    }

    public Type GetPageType(string key)
    {
        Type? pageType;
        lock (_pages)
        {
            if (!_pages.TryGetValue(key, out pageType))
            {
                throw new ArgumentException($"Page not found: {key}. Did you forget to call PageService.Configure?");
            }
        }

        return pageType;
    }

    private void Configure(string key, Type pageType)
    {
        lock (_pages)
        {
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }
            if (_pages.ContainsValue(pageType))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == pageType).Key}");
            }
            _pages.Add(key, pageType);
        }
    }

    private void Configure<VM, V>()
        where VM : ObservableObject
        where V : Page
    {
        lock (_pages)
        {
            var key = typeof(VM).FullName!;
            if (_pages.ContainsKey(key))
            {
                throw new ArgumentException($"The key {key} is already configured in PageService");
            }

            var type = typeof(V);
            if (_pages.ContainsValue(type))
            {
                throw new ArgumentException($"This type is already configured with key {_pages.First(p => p.Value == type).Key}");
            }

            _pages.Add(key, type);
        }
    }
}
