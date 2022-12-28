﻿using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.About
{
    public sealed partial class CloudflareValidationDialog : ContentDialog
    {
        public ElementTheme DialogTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public CloudflareValidationDialog()
        {
            XamlRoot = MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
        }
    }
}
