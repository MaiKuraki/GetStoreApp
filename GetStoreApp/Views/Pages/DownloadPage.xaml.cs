﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Common;
using GetStoreApp.Services.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinRT;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 下载页面
    /// </summary>
    public sealed partial class DownloadPage : Page, INotifyPropertyChanged
    {
        private bool _useInsVisValue;

        public bool UseInsVisValue
        {
            get { return _useInsVisValue; }

            set
            {
                _useInsVisValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public DownloadPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            UseInsVisValue = UseInstructionService.UseInsVisValue;
            Downloading.StartDownloadingTimer();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            base.OnNavigatingFrom(args);
            Downloading.StopDownloadingTimer(true);
            DownloadSchedulerService.DownloadingList.CollectionChanged -= Unfinished.OnDownloadingListItemsChanged;
            DownloadSchedulerService.DownloadingList.CollectionChanged -= Completed.OnDownloadingListItemsChanged;
        }

        /// <summary>
        /// DownloadPivot选中项发生变化时，关闭离开页面的事件，开启要导航到的页面的事件，并更新新页面的数据
        /// </summary>
        public async void OnSelectionChanged(object sender, SelectionChangedEventArgs args)
        {
            if (args.RemovedItems.Count > 0)
            {
                Pivot pivot = sender.As<Pivot>();
                if (pivot is not null)
                {
                    if (pivot.SelectedIndex == 0)
                    {
                        Downloading.StartDownloadingTimer();
                    }
                    else if (pivot.SelectedIndex == 1)
                    {
                        Downloading.StopDownloadingTimer(false);
                        await Unfinished.GetUnfinishedDataListAsync();
                    }
                    else if (pivot.SelectedIndex == 2)
                    {
                        Downloading.StopDownloadingTimer(false);
                        await Completed.GetCompletedDataListAsync();
                    }
                }
            }
        }

        /// <summary>
        /// 打开应用“下载设置”
        /// </summary>
        public void OnDownloadSettingsClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            NavigationService.NavigateTo(typeof(SettingsPage), AppNaviagtionArgs.DownloadOptions);
        }

        /// <summary>
        /// 了解更多下载管理说明
        /// </summary>
        public void OnLearnMoreClicked(object sender, RoutedEventArgs args)
        {
            DownloadFlyout.Hide();
            NavigationService.NavigateTo(typeof(AboutPage), AppNaviagtionArgs.SettingsHelp);
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
