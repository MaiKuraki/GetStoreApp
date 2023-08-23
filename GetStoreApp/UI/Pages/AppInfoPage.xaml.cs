using GetStoreApp.Extensions.DataType.Enums;
using GetStoreApp.Helpers.Root;
using GetStoreApp.Models.Controls.UWPApp;
using GetStoreApp.Services.Root;
using GetStoreApp.UI.Notifications;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Store.Preview;
using Windows.UI.Shell;
using Windows.UI.StartScreen;

namespace GetStoreApp.UI.Pages
{
    /// <summary>
    /// 应用信息页面
    /// </summary>
    public sealed partial class AppInfoPage : Page, INotifyPropertyChanged
    {
        private Dictionary<string, object> AppInfoDict { get; set; }

        private string _displayName = string.Empty;

        public string DisplayName
        {
            get { return _displayName; }

            set
            {
                _displayName = value;
                OnPropertyChanged();
            }
        }

        private string _familyName = string.Empty;

        public string FamilyName
        {
            get { return _familyName; }

            set
            {
                _familyName = value;
                OnPropertyChanged();
            }
        }

        private string _fullName = string.Empty;

        public string FullName
        {
            get { return _fullName; }

            set
            {
                _fullName = value;
                OnPropertyChanged();
            }
        }

        private string _description = string.Empty;

        public string Description
        {
            get { return _description; }

            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }

        private string _publisherName = string.Empty;

        public string PublisherName
        {
            get { return _publisherName; }

            set
            {
                _publisherName = value;
                OnPropertyChanged();
            }
        }

        private string _publisherId = string.Empty;

        public string PublisherId
        {
            get { return _publisherId; }

            set
            {
                _publisherId = value;
                OnPropertyChanged();
            }
        }

        private string _version;

        public string Version
        {
            get { return _version; }

            set
            {
                _version = value;
                OnPropertyChanged();
            }
        }

        private string _installedDate;

        public string InstalledDate
        {
            get { return _installedDate; }

            set
            {
                _installedDate = value;
                OnPropertyChanged();
            }
        }

        private string _architecture;

        public string Architecture
        {
            get { return _architecture; }

            set
            {
                _architecture = value;
                OnPropertyChanged();
            }
        }

        private string _signatureKind;

        private string SignatureKind
        {
            get { return _signatureKind; }

            set
            {
                _signatureKind = value;
                OnPropertyChanged();
            }
        }

        private string _resourceId;

        public string ResourceId
        {
            get { return _resourceId; }

            set
            {
                _resourceId = value;
                OnPropertyChanged();
            }
        }

        private string _isBundle;

        public string IsBundle
        {
            get { return _isBundle; }

            set
            {
                _isBundle = value;
                OnPropertyChanged();
            }
        }

        private string _isDevelopmentMode;

        public string IsDevelopmentMode
        {
            get { return _isDevelopmentMode; }

            set
            {
                _isDevelopmentMode = value;
                OnPropertyChanged();
            }
        }

        private string _isFramework;

        public string IsFramework
        {
            get { return _isFramework; }

            set
            {
                _isFramework = value;
                OnPropertyChanged();
            }
        }

        private string _isOptional;

        public string IsOptional
        {
            get { return _isOptional; }

            set
            {
                _isOptional = value;
                OnPropertyChanged();
            }
        }

        private string _isResourcePackage;

        public string IsResourcePackage
        {
            get { return _isResourcePackage; }

            set
            {
                _isResourcePackage = value;
                OnPropertyChanged();
            }
        }

        private string _isStub;

        public string IsStub
        {
            get { return _isStub; }

            set
            {
                _isStub = value;
                OnPropertyChanged();
            }
        }

        private string _vertifyIsOK;

        public string VertifyIsOK
        {
            get { return _vertifyIsOK; }

            set
            {
                _vertifyIsOK = value;
                OnPropertyChanged();
            }
        }

        private int _appListEntryCount;

        public int AppListEntryCount
        {
            get { return _appListEntryCount; }

            set
            {
                _appListEntryCount = value;
                OnPropertyChanged();
            }
        }

        // 启动对应入口的应用
        public XamlUICommand LaunchCommand { get; } = new XamlUICommand();

        // 复制应用入口的应用程序用户模型 ID
        public XamlUICommand CopyAUMIDCommand { get; } = new XamlUICommand();

        // 固定应用到桌面
        public XamlUICommand PinToDesktopCommand { get; } = new XamlUICommand();

        // 固定应用入口到开始“屏幕”
        public XamlUICommand PinToStartScreenCommand { get; } = new XamlUICommand();

        // 固定应用入口到任务栏
        public XamlUICommand PinToTaskbarCommand { get; } = new XamlUICommand();

        public ObservableCollection<AppListEntryModel> AppListEntryList { get; } = new ObservableCollection<AppListEntryModel>();

        public event PropertyChangedEventHandler PropertyChanged;

        public AppInfoPage()
        {
            InitializeComponent();

            LaunchCommand.ExecuteRequested += (sender, args) =>
            {
                AppListEntryModel appListEntryItem = args.Parameter as AppListEntryModel;
                Task.Run(async () =>
                {
                    try
                    {
                        await appListEntryItem.AppListEntry.LaunchAsync();
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, string.Format("Open app {0} failed", appListEntryItem.DisplayName), e);
                    }
                });
            };

            CopyAUMIDCommand.ExecuteRequested += (sender, args) =>
            {
                string aumid = args.Parameter as string;

                if (aumid is not null)
                {
                    CopyPasteHelper.CopyToClipBoard(aumid);
                    new DataCopyNotification(this, DataCopyType.AppUserModelId).Show();
                }
            };

            PinToDesktopCommand.ExecuteRequested += (sender, args) =>
            {
                Task.Run(() =>
                {
                    bool IsPinnedSuccessfully = false;

                    try
                    {
                        if (StoreConfiguration.IsPinToDesktopSupported())
                        {
                            StoreConfiguration.PinToDesktop(FamilyName);
                            IsPinnedSuccessfully = true;
                        }
                    }
                    catch (Exception e)
                    {
                        LogService.WriteLog(LogType.ERROR, "Create desktop shortcut failed.", e);
                    }
                    finally
                    {
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            new QuickOperationNotification(this, QuickOperationType.DesktopShortcut, IsPinnedSuccessfully).Show();
                        });
                    }
                });
            };

            PinToStartScreenCommand.ExecuteRequested += (sender, args) =>
            {
                AppListEntryModel appListEntryItem = args.Parameter as AppListEntryModel;

                if (appListEntryItem is not null)
                {
                    Task.Run(async () =>
                    {
                        bool IsPinnedSuccessfully = false;

                        try
                        {
                            StartScreenManager startScreenManager = StartScreenManager.GetDefault();

                            IsPinnedSuccessfully = await startScreenManager.RequestAddAppListEntryAsync(appListEntryItem.AppListEntry);
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.ERROR, "Pin app to startscreen failed.", e);
                        }
                        finally
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                new QuickOperationNotification(this, QuickOperationType.StartScreen, IsPinnedSuccessfully).Show();
                            });
                        }
                    });
                }
            };

            PinToTaskbarCommand.ExecuteRequested += (sender, args) =>
            {
                AppListEntryModel appListEntryItem = args.Parameter as AppListEntryModel;

                if (appListEntryItem is not null)
                {
                    Task.Run(async () =>
                    {
                        bool IsPinnedSuccessfully = false;

                        try
                        {
                            string featureId = "com.microsoft.windows.taskbar.pin";
                            string token = FeatureAccessHelper.GenerateTokenFromFeatureId(featureId);
                            string attestation = FeatureAccessHelper.GenerateAttestation(featureId);
                            LimitedAccessFeatureRequestResult accessResult = LimitedAccessFeatures.TryUnlockFeature(featureId, token, attestation);

                            if (accessResult.Status is LimitedAccessFeatureStatus.Available)
                            {
                                IsPinnedSuccessfully = await TaskbarManager.GetDefault().RequestPinAppListEntryAsync(appListEntryItem.AppListEntry);
                            }
                        }
                        catch (Exception e)
                        {
                            LogService.WriteLog(LogType.ERROR, "Pin app to taskbar failed.", e);
                        }
                        finally
                        {
                            DispatcherQueue.TryEnqueue(() =>
                            {
                                new QuickOperationNotification(this, QuickOperationType.Taskbar, IsPinnedSuccessfully).Show();
                            });
                        }
                    });
                }
            };
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            base.OnNavigatedTo(args);
            if (args.Parameter is not null)
            {
                AppInfoDict = args.Parameter as Dictionary<string, object>;
            }

            InitializeAppInfo();
        }

        /// <summary>
        /// 复制应用信息
        /// </summary>
        public void OnCopyClicked(object sender, RoutedEventArgs args)
        {
            Task.Run(() =>
            {
                StringBuilder copyBuilder = new StringBuilder();
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/DisplayName"), DisplayName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/FamilyName"), FamilyName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/FullName"), FullName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/Description"), Description));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/PublisherName"), PublisherName));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/PublisherId"), PublisherId));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/Version"), Version));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/InstalledDate"), InstalledDate));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/Architecture"), Architecture));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/SignatureKind"), SignatureKind));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/ResourceId"), ResourceId));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsBundle"), IsBundle));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsDevelopmentMode"), IsDevelopmentMode));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsFramework"), IsFramework));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsOptional"), IsOptional));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsResourcePackage"), IsResourcePackage));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/IsStub"), IsStub));
                copyBuilder.AppendLine(string.Format("{0}:\t{1}", ResourceService.GetLocalized("UWPApp/VertifyIsOK"), VertifyIsOK));

                DispatcherQueue.TryEnqueue(() =>
                {
                    CopyPasteHelper.CopyToClipBoard(copyBuilder.ToString());
                    new DataCopyNotification(this, DataCopyType.PackageInformation).Show();
                });
            });
        }

        /// <summary>
        /// 属性值发生变化时通知更改
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 初始化应用信息
        /// </summary>
        private void InitializeAppInfo()
        {
            DisplayName = AppInfoDict[nameof(DisplayName)].ToString();
            FamilyName = AppInfoDict[nameof(FamilyName)].ToString();
            FullName = AppInfoDict[nameof(FullName)].ToString();
            Description = AppInfoDict[nameof(Description)].ToString();
            PublisherName = AppInfoDict[nameof(PublisherName)].ToString();
            PublisherId = AppInfoDict[nameof(PublisherId)].ToString();
            Version = AppInfoDict[nameof(Version)].ToString();
            InstalledDate = AppInfoDict[nameof(InstalledDate)].ToString();
            Architecture = AppInfoDict[nameof(Architecture)].ToString();
            SignatureKind = AppInfoDict[nameof(SignatureKind)].ToString();
            ResourceId = AppInfoDict[nameof(ResourceId)].ToString();
            IsBundle = AppInfoDict[nameof(IsBundle)].ToString();
            IsDevelopmentMode = AppInfoDict[nameof(IsDevelopmentMode)].ToString();
            IsFramework = AppInfoDict[nameof(IsFramework)].ToString();
            IsOptional = AppInfoDict[nameof(IsOptional)].ToString();
            IsResourcePackage = AppInfoDict[nameof(IsResourcePackage)].ToString();
            IsStub = AppInfoDict[nameof(IsStub)].ToString();
            VertifyIsOK = AppInfoDict[nameof(VertifyIsOK)].ToString();
            AppListEntryCount = Convert.ToInt32(AppInfoDict[nameof(AppListEntryCount)]);

            AppListEntryList.Clear();
            foreach (AppListEntryModel appListEntry in AppInfoDict[nameof(AppListEntryList)] as List<AppListEntryModel>)
            {
                AppListEntryList.Add(appListEntry);
            }
        }
    }
}
