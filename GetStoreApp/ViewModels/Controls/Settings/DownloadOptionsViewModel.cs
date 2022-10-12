﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GetStoreApp.Contracts.Services.Settings;
using GetStoreApp.Contracts.Services.Shell;
using GetStoreApp.Extensions.Enum;
using GetStoreApp.Helpers;
using GetStoreApp.Models.Settings;
using GetStoreApp.UI.Dialogs;
using GetStoreApp.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace GetStoreApp.ViewModels.Controls.Settings
{
    public class DownloadOptionsViewModel : ObservableRecipient
    {
        private IDownloadOptionsService DownloadOptionsService { get; } = IOCHelper.GetService<IDownloadOptionsService>();

        private INavigationService NavigationService { get; } = IOCHelper.GetService<INavigationService>();

        public List<int> DownloadItemList => DownloadOptionsService.DownloadItemList;

        public List<DownloadModeModel> DownloadModeList => DownloadOptionsService.DownloadModeList;

        private StorageFolder _downloadFolder;

        public StorageFolder DownloadFolder
        {
            get { return _downloadFolder; }

            set { SetProperty(ref _downloadFolder, value); }
        }

        private int _downloadItem;

        public int DownloadItem
        {
            get { return _downloadItem; }

            set { SetProperty(ref _downloadItem, value); }
        }

        private DownloadModeModel _downloadMode;

        public DownloadModeModel DownloadMode
        {
            get { return _downloadMode; }

            set { SetProperty(ref _downloadMode, value); }
        }

        // 下载管理说明
        public IRelayCommand DownloadInstructionCommand => new RelayCommand(() =>
        {
            App.NavigationArgs = AppNaviagtionArgs.SettingsHelp;
            NavigationService.NavigateTo(typeof(AboutViewModel).FullName, null, new DrillInNavigationTransitionInfo(), false);
        });

        // 打开文件存放目录
        public IRelayCommand OpenFolderCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.OpenFolderAsync(DownloadFolder);
        });

        // 使用默认目录
        public IRelayCommand UseDefaultFolderCommand => new RelayCommand(async () =>
        {
            DownloadFolder = DownloadOptionsService.DefaultFolder;
            await DownloadOptionsService.SetFolderAsync(DownloadOptionsService.DefaultFolder);
        });

        // 修改下载目录
        public IRelayCommand ChangeFolderCommand => new RelayCommand(async () =>
        {
            bool ChangeSuccessfully = false;

            while (!ChangeSuccessfully)
            {
                // 选择文件夹
                FolderPicker folderPicker = new FolderPicker();

                IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
                WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hwnd);

                folderPicker.SuggestedStartLocation = PickerLocationId.Downloads;

                StorageFolder Folder = await folderPicker.PickSingleFolderAsync();

                if (Folder is not null)
                {
                    bool CheckResult = FolderHelper.CanWriteToFolder(Folder, FileSystemRights.Write);

                    if (CheckResult)
                    {
                        DownloadFolder = Folder;
                        await DownloadOptionsService.SetFolderAsync(DownloadFolder);

                        ChangeSuccessfully = true;
                    }
                    else
                    {
                        ContentDialogResult result = await new FolderAccessFailedDialog().ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            ChangeSuccessfully = false;
                        }
                        else if (result == ContentDialogResult.Secondary)
                        {
                            DownloadFolder = DownloadOptionsService.DefaultFolder;
                            await DownloadOptionsService.SetFolderAsync(DownloadFolder);

                            ChangeSuccessfully = true;
                        }
                        else
                        {
                            ChangeSuccessfully = true;
                        }
                    }
                }
                else
                {
                    ChangeSuccessfully = true;
                }
            }
        });

        // 修改同时下载文件数
        public IRelayCommand DownloadItemCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.SetItemAsync(DownloadItem);
        });

        // 修改下载文件的方式
        public IRelayCommand DownloadModeCommand => new RelayCommand(async () =>
        {
            await DownloadOptionsService.SetModeAsync(DownloadMode);
        });

        public DownloadOptionsViewModel()
        {
            DownloadFolder = DownloadOptionsService.DownloadFolder;

            DownloadItem = DownloadOptionsService.DownloadItem;

            DownloadMode = DownloadOptionsService.DownloadMode;
        }
    }
}
