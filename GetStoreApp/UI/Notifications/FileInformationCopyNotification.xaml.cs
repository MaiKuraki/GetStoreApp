﻿using GetStoreApp.Services.Controls.Settings.Appearance;
using GetStoreApp.Views.Window;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;

namespace GetStoreApp.UI.Notifications
{
    public sealed partial class FileInformationCopyNotification : UserControl
    {
        public ElementTheme NotificationTheme => (ElementTheme)Enum.Parse(typeof(ElementTheme), ThemeService.AppTheme.InternalName);

        private Popup Popup { get; set; } = new Popup();

        private int Duration = 2000;

        public FileInformationCopyNotification(bool copyState, double? duration = null)
        {
            InitializeComponent();
            ViewModel.Initialize(copyState);

            SetPopUpPlacement();

            Popup.Child = this;
            Popup.XamlRoot = MainWindow.GetMainWindowXamlRoot();

            if (duration.HasValue)
            {
                Duration = Convert.ToInt32(duration * 1000);
            }
        }

        /// <summary>
        /// 控件加载完成后显示动画，动态设置控件位置
        /// </summary>
        private void NotificationLoaded(object sender, RoutedEventArgs args)
        {
            PopupIn.Begin();
            MainWindow.Current.SizeChanged += NotificationPlaceChanged;
        }

        /// <summary>
        /// 控件卸载时移除相应的事件
        /// </summary>
        private void NotificationUnLoaded(object sender, RoutedEventArgs args)
        {
            MainWindow.Current.SizeChanged -= NotificationPlaceChanged;
        }

        /// <summary>
        /// 窗口大小调整时修改应用内通知的相对位置
        /// </summary>
        private void NotificationPlaceChanged(object sender, WindowSizeChangedEventArgs args)
        {
            SetPopUpPlacement();
        }

        /// <summary>
        /// 应用内通知加载动画演示完成时发生
        /// </summary>
        private async void PopupInCompleted(object sender, object e)
        {
            await Task.Delay(Duration);
            PopupOut.Begin();
        }

        /// <summary>
        /// 应用内通知加载动画卸载完成时发生，关闭控件
        /// </summary>
        public void PopupOutCompleted(object sender, object e)
        {
            Popup.IsOpen = false;
        }

        /// <summary>
        /// 设置PopUp的显示位置
        /// </summary>
        private void SetPopUpPlacement()
        {
            Width = MainWindow.Current.Bounds.Width;
            Height = MainWindow.Current.Bounds.Height;

            Popup.VerticalOffset = 75;
        }

        /// <summary>
        /// 显示弹窗
        /// </summary>
        public void Show()
        {
            Popup.IsOpen = true;
        }
    }
}
