﻿using GetStoreApp.Contracts.Command;
using GetStoreApp.Extensions.Command;
using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Extensions.Messaging;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Helpers.Window;
using GetStoreApp.Models.Controls.Settings.Appearance;
using GetStoreApp.Models.Window;
using GetStoreApp.Services.Controls.Download;
using GetStoreApp.Services.Controls.Settings.Advanced;
using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Services.Root;
using GetStoreApp.Services.Window;
using GetStoreApp.UI.Dialogs.Common;
using GetStoreApp.ViewModels.Base;
using GetStoreApp.Views.Pages;
using GetStoreApp.Views.Window;
using GetStoreApp.WindowsAPI.PInvoke.UxTheme;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace GetStoreApp.ViewModels.Window
{
    public sealed class MainWindowViewModel : ViewModelBase
    {
        private SolidColorBrush _appBackground;

        public SolidColorBrush AppBackground
        {
            get { return _appBackground; }

            set
            {
                _appBackground = value;
                OnPropertyChanged();
            }
        }

        private Thickness _appTitleBarMargin;

        public Thickness AppTitleBarMargin
        {
            get { return _appTitleBarMargin; }

            set
            {
                _appTitleBarMargin = value;
                OnPropertyChanged();
            }
        }

        private bool _isBackEnabled;

        public bool IsBackEnabled
        {
            get { return _isBackEnabled; }

            set
            {
                _isBackEnabled = value;
                OnPropertyChanged();
            }
        }

        private bool _isPaneToggleButtonVisible;

        public bool IsPaneToggleButtonVisible
        {
            get { return _isPaneToggleButtonVisible; }

            set
            {
                _isPaneToggleButtonVisible = value;
                OnPropertyChanged();
            }
        }

        private NavigationViewPaneDisplayMode _navigationDisplayMode;

        public NavigationViewPaneDisplayMode NavigationDispalyMode
        {
            get { return _navigationDisplayMode; }

            set
            {
                _navigationDisplayMode = value;
                OnPropertyChanged();
            }
        }

        private NavigationViewItem _selectedItem;

        public NavigationViewItem SelectedItem
        {
            get { return _selectedItem; }

            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private Dictionary<string, Type> PageDict { get; } = new Dictionary<string, Type>()
        {
            {"Home",typeof(HomePage) },
            {"History",typeof(HistoryPage) },
            {"Download",typeof(DownloadPage) },
            {"Web",typeof(WebPage) },
            {"About",typeof(AboutPage) },
            {"Settings",typeof(SettingsPage) }
        };

        /// <summary>
        /// 设置窗口处于非激活状态时的背景色
        /// </summary>
        public IRelayCommand ActivatedCommand => new RelayCommand<WindowActivatedEventArgs>((args) =>
        {
            BackdropHelper.SetBackdropState(AlwaysShowBackdropService.AlwaysShowBackdropValue, args);
        });

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        public IRelayCommand ClosedCommand => new RelayCommand<WindowEventArgs>(async (args) =>
        {
            args.Handled = true;

            if (AppExitService.AppExit.InternalName == AppExitService.AppExitList[0].InternalName)
            {
                WindowHelper.HideAppWindow();
            }
            else
            {
                // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
                if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
                {
                    // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                    if (!App.IsDialogOpening)
                    {
                        App.IsDialogOpening = true;

                        ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                        if (result == ContentDialogResult.Primary)
                        {
                            await CloseApp();
                        }
                        else if (result == ContentDialogResult.Secondary)
                        {
                            if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                            {
                                NavigationService.NavigateTo(typeof(DownloadPage));
                            }
                        }

                        App.IsDialogOpening = false;
                    }
                }
                else
                {
                    await CloseApp();
                }
            }
        });

        // 窗口大小发生改变时，设置页面导航视图样式
        public IRelayCommand SizeChangedCommand => new RelayCommand<WindowSizeChangedEventArgs>((args) =>
        {
            if (MainWindow.Current.Width >= 768)
            {
                IsPaneToggleButtonVisible = false;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.Left;
                AppTitleBarMargin = new Thickness(48, 0, 0, 0);
            }
            else
            {
                IsPaneToggleButtonVisible = true;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.LeftMinimal;
                AppTitleBarMargin = new Thickness(96, 0, 0, 0);
            }
        });

        /// <summary>
        /// 当菜单中的项收到交互（如单击或点击）时发生。
        /// </summary>
        public IRelayCommand NavigationItemCommand => new RelayCommand<object>((invokedItemTag) =>
        {
            if (invokedItemTag is not null)
            {
                NavigationModel navigationViewItem = NavigationService.NavigationItemList.Find(item => item.NavigationTag == Convert.ToString(invokedItemTag));
                if (SelectedItem != navigationViewItem.NavigationItem)
                {
                    NavigationService.NavigateTo(navigationViewItem.NavigationPage);
                }
            }
        });

        // 隐藏 / 显示窗口
        public IRelayCommand ShowOrHideWindowCommand => new RelayCommand(() =>
        {
            // 隐藏窗口
            if (MainWindow.Current.Visible)
            {
                WindowHelper.HideAppWindow();
            }
            // 显示窗口
            else
            {
                WindowHelper.ShowAppWindow();
            }
        });

        // 打开设置
        public IRelayCommand SettingsCommand => new RelayCommand(() =>
        {
            // 窗口置前端
            WindowHelper.ShowAppWindow();

            if (NavigationService.GetCurrentPageType() != typeof(SettingsPage))
            {
                NavigationService.NavigateTo(typeof(SettingsPage));
            }
        });

        // 退出应用
        public IRelayCommand ExitCommand => new RelayCommand(async () =>
        {
            // 下载队列存在任务时，弹出对话窗口确认是否要关闭窗口
            if (DownloadSchedulerService.DownloadingList.Count > 0 || DownloadSchedulerService.WaitingList.Count > 0)
            {
                // 窗口置前端
                WindowHelper.ShowAppWindow();

                // 关闭窗口提示对话框是否已经处于打开状态，如果是，不再弹出
                if (!App.IsDialogOpening)
                {
                    App.IsDialogOpening = true;

                    ContentDialogResult result = await new ClosingWindowDialog().ShowAsync();

                    if (result == ContentDialogResult.Primary)
                    {
                        await CloseApp();
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        if (NavigationService.GetCurrentPageType() != typeof(DownloadPage))
                        {
                            NavigationService.NavigateTo(typeof(DownloadPage));
                        }
                    }

                    App.IsDialogOpening = false;
                }
            }
            else
            {
                await CloseApp();
            }
        });

        // 当后退按钮收到交互（如单击或点击）时发生。
        public void OnNavigationViewBackRequested(object sender, NavigationViewBackRequestedEventArgs args)
        {
            NavigationService.NavigationFrom();
        }

        /// <summary>
        /// 导航控件加载完成后初始化内容，初始化导航视图控件属性和应用的背景色。
        /// </summary>
        public void OnNavigationViewLoaded(object sender, RoutedEventArgs args)
        {
            // 初始化导航视图控件属性和应用的背景色。
            if (MainWindow.Current.Width >= 768)
            {
                IsPaneToggleButtonVisible = false;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.Left;
                AppTitleBarMargin = new Thickness(48, 0, 0, 0);
            }
            else
            {
                IsPaneToggleButtonVisible = true;
                NavigationDispalyMode = NavigationViewPaneDisplayMode.LeftMinimal;
                AppTitleBarMargin = new Thickness(96, 0, 0, 0);
            }

            SetAppBackground();
            SetContextMenuTheme((FrameworkElement)MainWindow.Current.Content);

            ((FrameworkElement)MainWindow.Current.Content).ActualThemeChanged += OnActualThemeChanged;

            // 导航控件加载完成后初始化内容
            NavigationView navigationView = sender as NavigationView;

            if (navigationView is null)
            {
                return;
            }

            foreach (object item in navigationView.MenuItems)
            {
                NavigationViewItem navigationViewItem = item as NavigationViewItem;
                if (navigationViewItem is not null)
                {
                    string Tag = Convert.ToString(navigationViewItem.Tag);

                    NavigationService.NavigationItemList.Add(new NavigationModel()
                    {
                        NavigationTag = Tag,
                        NavigationItem = navigationViewItem,
                        NavigationPage = PageDict[Tag],
                    });
                }
            }

            foreach (object item in navigationView.FooterMenuItems)
            {
                NavigationViewItem navigationViewItem = item as NavigationViewItem;
                if (navigationViewItem is not null)
                {
                    string Tag = Convert.ToString(navigationViewItem.Tag);

                    NavigationService.NavigationItemList.Add(new NavigationModel()
                    {
                        NavigationTag = Tag,
                        NavigationItem = navigationViewItem,
                        NavigationPage = PageDict[Tag],
                    });
                }
            }

            SelectedItem = NavigationService.NavigationItemList[0].NavigationItem;
            NavigationService.NavigateTo(typeof(HomePage));
            IsBackEnabled = NavigationService.CanGoBack();

            // 应用背景色设置跟随系统发生变化时，当系统背景色设置发生变化时修改应用背景色
            Messenger.Default.Register<BackdropModel>(this, MessageToken.BackdropChanged, (backdropMessage) =>
            {
                SetAppBackground();
            });

            // 应用主题设置跟随系统发生变化时，当系统主题设置发生变化时修改修改应用背景色
            Messenger.Default.Register<bool>(this, MessageToken.SystemSettingsChanged, (systemSettingsChangedMessage) =>
            {
                SetAppBackground();
            });
        }

        // 页面被卸载时，注销所有事件
        public void OnNavigationViewUnLoaded(object sender, RoutedEventArgs args)
        {
            Messenger.Default.Unregister(this);
        }

        // 导航完成后发生
        public void OnFrameNavigated(object sender, NavigationEventArgs args)
        {
            Type CurrentPageType = NavigationService.GetCurrentPageType();
            SelectedItem = NavigationService.NavigationItemList.Find(item => item.NavigationPage == CurrentPageType).NavigationItem;
            IsBackEnabled = NavigationService.CanGoBack();
        }

        // 导航失败时发生
        public void OnFrameNavgationFailed(object sender, NavigationFailedEventArgs args)
        {
            throw new Exception("页面" + args.SourcePageType.FullName + "加载失败");
        }

        /// <summary>
        /// 设置主题发生变化时修改标题栏按钮的主题
        /// </summary>
        private void OnActualThemeChanged(FrameworkElement sender, object args)
        {
            SetAppBackground();
            SetContextMenuTheme(sender);
        }

        /// <summary>
        /// 设置应用背景色
        /// </summary>
        private void SetAppBackground()
        {
            if (BackdropService.AppBackdrop.InternalName == BackdropService.BackdropList[0].InternalName)
            {
                if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[0].InternalName)
                {
                    if (RegistryHelper.GetRegistryAppTheme() == ElementTheme.Light)
                    {
                        AppBackground = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                    }
                    else
                    {
                        AppBackground = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                    }
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[1].InternalName)
                {
                    AppBackground = new SolidColorBrush(Color.FromArgb(255, 240, 243, 249));
                }
                else if (ThemeService.AppTheme.InternalName == ThemeService.ThemeList[2].InternalName)
                {
                    AppBackground = new SolidColorBrush(Color.FromArgb(255, 20, 20, 20));
                }
            }
            else
            {
                AppBackground = new SolidColorBrush(Colors.Transparent);
            }
        }

        /// <summary>
        /// 设置Win32 传统右键菜单的主题值
        /// </summary>
        /// 注意：Windows 10 1809（17763）和Windows 10 1903（18362）及更高的版本系统最终呈现的效果不同
        /// Windows 10 1903（18362）有4个值，可以由用户单独指定传统右键菜单的主题值
        /// Windows 10 1809（17763）只有2个值，设定为True时右键菜单主题值会根据系统变化，不能由用户单独指定
        private void SetContextMenuTheme(FrameworkElement element)
        {
            switch (element.RequestedTheme)
            {
                case ElementTheme.Default:
                    {
                        if (InfoHelper.GetSystemVersion()["BuildNumber"] > 18362)
                        {
                            UxThemeLibrary.SetPreferredAppMode(PreferredAppMode.AllowDark);
                        }
                        else
                        {
                            UxThemeLibrary.AllowDarkModeForApp(true);
                        }
                        break;
                    }
                case ElementTheme.Light:
                    {
                        if (InfoHelper.GetSystemVersion()["BuildNumber"] > 18362)
                        {
                            UxThemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceLight);
                        }
                        else
                        {
                            UxThemeLibrary.AllowDarkModeForApp(false);
                        }
                        break;
                    }
                case ElementTheme.Dark:
                    {
                        if (InfoHelper.GetSystemVersion()["BuildNumber"] > 18362)
                        {
                            UxThemeLibrary.SetPreferredAppMode(PreferredAppMode.ForceDark);
                        }
                        else
                        {
                            UxThemeLibrary.AllowDarkModeForApp(true);
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// 关闭应用并释放所有资源
        /// </summary>
        private async Task CloseApp()
        {
            await DownloadSchedulerService.CloseDownloadSchedulerAsync();
            await Aria2Service.CloseAria2Async();
            Messenger.Default.Send(true, MessageToken.WindowClosed);
            AppNotificationService.Unregister();
            BackdropHelper.ReleaseBackdrop();
            Environment.Exit(Convert.ToInt32(AppExitCode.Successfully));
        }
    }
}
