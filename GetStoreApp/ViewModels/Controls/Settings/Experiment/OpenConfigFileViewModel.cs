﻿using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Services.Controls.Download;
using System;
using Windows.Storage;
using Windows.System;

namespace GetStoreApp.ViewModels.Controls.Settings.Experiment
{
    /// <summary>
    /// 实验性功能：打开Aria2配置文件用户控件视图模型
    /// </summary>
    public sealed class OpenConfigFileViewModel
    {
        // 打开配置文件目录
        public IRelayCommand OpenConfigFileCommand => new RelayCommand(async () =>
        {
            if (Aria2Service.Aria2ConfPath is not null)
            {
                string filePath = Aria2Service.Aria2ConfPath.Replace(@"\\", @"\");

                // 定位文件，若定位失败，则仅启动资源管理器并打开桌面目录
                if (!string.IsNullOrEmpty(filePath))
                {
                    try
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(filePath);
                        StorageFolder folder = await file.GetParentAsync();
                        FolderLauncherOptions options = new FolderLauncherOptions();
                        options.ItemsToSelect.Add(file);
                        await Launcher.LaunchFolderAsync(folder, options);
                    }
                    catch (Exception)
                    {
                        await Launcher.LaunchFolderPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                    }
                }
                else
                {
                    await Launcher.LaunchFolderPathAsync(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                }
            }
        });
    }
}
