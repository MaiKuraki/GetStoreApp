﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Models.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Window;
using GetStoreApp.Views.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinRT;

namespace GetStoreApp.UI.Controls.Settings.Advanced
{
    /// <summary>
    /// 设置页面：应用安装方式设置控件
    /// </summary>
    public sealed partial class InstallModeControl : Grid, INotifyPropertyChanged
    {
        private InstallModeModel _installMode = InstallModeService.InstallMode;

        public InstallModeModel InstallMode
        {
            get { return _installMode; }

            set
            {
                _installMode = value;
                OnPropertyChanged();
            }
        }

        public List<InstallModeModel> InstallModeList => InstallModeService.InstallModeList;

        public event PropertyChangedEventHandler PropertyChanged;

        public InstallModeControl()
        {
            InitializeComponent();
        }

        public bool IsItemChecked(string selectedInternalName, string internalName)
        {
            return selectedInternalName == internalName;
        }

        /// <summary>
        /// 应用安装方式说明
        /// </summary>
        public void OnInstallModeTipClicked(object sender, RoutedEventArgs args)
        {
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
        }

        /// <summary>
        /// 应用安装方式设置
        /// </summary>
        public async void OnInstallModeSelectClicked(object sender, RoutedEventArgs args)
        {
            ToggleMenuFlyoutItem item = sender.As<ToggleMenuFlyoutItem>();
            if (item.Tag is not null)
            {
                InstallMode = InstallModeList[Convert.ToInt32(item.Tag)];
                await InstallModeService.SetInstallModeAsync(InstallMode);
            }
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
