﻿using GetStoreApp.Models.Controls.Store;
using GetStoreApp.Services.Root;
using GetStoreApp.WindowsAPI.Controls;
using GetStoreApp.WindowsAPI.PInvoke.Shell32;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GetStoreApp.UI.Controls.Store
{
    /// <summary>
    /// 微软商店页面：状态栏控件
    /// </summary>
    public sealed partial class StatusBarControl : InfoBar, INotifyPropertyChanged
    {
        private List<StatusBarStateModel> StatusBarStateList => ResourceService.StatusBarStateList;

        private InfoBarSeverity _infoSeverity = InfoBarSeverity.Informational;

        public InfoBarSeverity InfoBarSeverity
        {
            get { return _infoSeverity; }

            set
            {
                _infoSeverity = value;
                OnPropertyChanged();
            }
        }

        private string _stateInfoText;

        public string StateInfoText
        {
            get { return _stateInfoText; }

            set
            {
                _stateInfoText = value;
                OnPropertyChanged();
            }
        }

        private bool _statePrRingActValue = false;

        public bool StatePrRingActValue
        {
            get { return _statePrRingActValue; }

            set
            {
                _statePrRingActValue = value;
                OnPropertyChanged();
            }
        }

        private bool _statePrRingVisValue = false;

        public bool StatePrRingVisValue
        {
            get { return _statePrRingVisValue; }

            set
            {
                _statePrRingVisValue = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StatusBarControl()
        {
            InitializeComponent();
            StateInfoText = ResourceService.GetLocalized("Store/StatusInfoWelcome");
            PropertyChanged += OnStatusBarPropertyChanged;
        }

        /// <summary>
        /// 设置控件的状态
        /// </summary>
        public void SetControlState(int state)
        {
            InfoBarSeverity = StatusBarStateList[state].InfoBarSeverity;
            StateInfoText = StatusBarStateList[state].StateInfoText;
            StatePrRingVisValue = StatusBarStateList[state].StatePrRingVisValue;
            StatePrRingActValue = StatusBarStateList[state].StatePrRingActValue;
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 圆环动画状态修改时修改任务栏的动画显示
        /// </summary>
        private void OnStatusBarPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(StatePrRingActValue))
            {
                if (StatePrRingActValue)
                {
                    TaskbarStateManager.SetProgressState(TBPFLAG.TBPF_INDETERMINATE, Program.ApplicationRoot.MainWindow.Handle);
                }
                else
                {
                    TaskbarStateManager.SetProgressState(TBPFLAG.TBPF_NOPROGRESS, Program.ApplicationRoot.MainWindow.Handle);
                }
            }
        }
    }
}
