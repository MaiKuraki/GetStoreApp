﻿using GetStoreApp.Services.Controls.Settings.Appearance;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace GetStoreApp.UI.Dialogs.Settings
{
    /// <summary>
    /// 实验性功能配置对话框视图
    /// </summary>
    public sealed partial class ExperimentalConfigDialog : ContentDialog
    {
        public ElementTheme DialogTheme { get; } = (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        public ExperimentalConfigDialog()
        {
            XamlRoot = Program.ApplicationRoot.MainWindow.GetMainWindowXamlRoot();
            InitializeComponent();
        }
    }
}
