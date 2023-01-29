﻿using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Services.Root;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace GetStoreApp.Views.Pages
{
    /// <summary>
    /// 历史记录页面
    /// </summary>
    public sealed partial class HistoryPage : Page
    {
        public string Fillin { get; } = ResourceService.GetLocalized("History/Fillin");

        public string FillinToolTip { get; } = ResourceService.GetLocalized("History/FillinToolTip");

        public string Copy { get; } = ResourceService.GetLocalized("History/Copy");

        public string CopyToolTip { get; } = ResourceService.GetLocalized("History/CopyToolTip");

        public AppNaviagtionArgs HistoryNavigationArgs { get; set; } = AppNaviagtionArgs.None;

        public HistoryPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            ViewModel.OnNavigatedTo();
            if (args.Parameter is not null)
            {
                HistoryNavigationArgs = (AppNaviagtionArgs)Enum.Parse(typeof(AppNaviagtionArgs), Convert.ToString(args.Parameter));
            }
            else
            {
                HistoryNavigationArgs = AppNaviagtionArgs.None;
            }
        }

        /// <summary>
        /// 页面加载完成后如果有具体的要求，将页面滚动到指定位置
        /// </summary>
        public void HistoryPageLoaded(object sender, RoutedEventArgs args)
        {
            if (HistoryNavigationArgs == AppNaviagtionArgs.History)
            {
                HistoryScroll.ChangeView(null, 0, null);
            }
        }

        /// <summary>
        /// 本地化历史记录数量统计信息
        /// </summary>
        public string LocalizeHistoryCountInfo(int count)
        {
            if (count is 0)
            {
                return ResourceService.GetLocalized("History/HistoryEmpty");
            }
            else
            {
                return string.Format(ResourceService.GetLocalized("History/HistoryCountInfo"), count);
            }
        }
    }
}
