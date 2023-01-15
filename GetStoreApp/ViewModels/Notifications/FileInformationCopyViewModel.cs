﻿using GetStoreApp.ViewModels.Base;

namespace GetStoreApp.ViewModels.Notifications
{
    /// <summary>
    /// 文件信息复制成功后应用内通知视图模型
    /// </summary>
    public sealed class FileInformationCopyViewModel : ViewModelBase
    {
        private bool _copyState = false;

        public bool CopyState
        {
            get { return _copyState; }

            set
            {
                _copyState = value;
                OnPropertyChanged();
            }
        }

        public void Initialize(bool copyState)
        {
            CopyState = copyState;
        }
    }
}
