﻿using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.UI.Dialogs.Settings;
using System;

namespace GetStoreApp.ViewModels.Pages
{
    /// <summary>
    /// 设置页面数据模型
    /// </summary>
    public sealed class SettingsViewModel
    {
        // 打开重启应用确认的窗口对话框
        public IRelayCommand RestartCommand = new RelayCommand(async () =>
        {
            if (!Program.ApplicationRoot.IsDialogOpening)
            {
                Program.ApplicationRoot.IsDialogOpening = true;
                await new RestartAppsDialog().ShowAsync();
                Program.ApplicationRoot.IsDialogOpening = false;
            }
        });
    }
}
