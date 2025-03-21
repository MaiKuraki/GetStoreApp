﻿using GetStoreAppWidget.Services.Controls.Settings;
using GetStoreAppWidget.Services.Root;
using GetStoreAppWidget.WindowsAPI.PInvoke.Ole32;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

//开发完成后，这一部分内容加到 Package.AppxManifest 文件中
//<com:Extension Category="windows.comServer">
//  <com:ComServer>
//    <com:ExeServer Executable="GetStoreAppWidget.exe" DisplayName="GetStoreAppWidget">
//      <com:Class Id="F96AFBA5-38A4-DB97-DAC7-DE29871B26B5" DisplayName="GetStoreAppWidget" />
//    </com:ExeServer>
//  </com:ComServer>
//</com:Extension>

namespace GetStoreAppWidget
{
    /// <summary>
    /// 获取商店应用 小组件
    /// </summary>
    public class Program
    {
        private static readonly WidgetProviderFactory widgetProviderFactory = new();
        private static readonly AutoResetEvent autoResetEvent = new(false);

        public static StrategyBasedComWrappers StrategyBasedComWrappers { get; } = new();

        /// <summary>
        /// 应用程序的主入口点
        /// </summary>
        [MTAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();
            LanguageService.InitializeLanguage();
            ResourceService.InitializeResource(LanguageService.DefaultAppLanguage, LanguageService.AppLanguage);
            Ole32Library.CoRegisterClassObject(typeof(WidgetProvider).GUID, StrategyBasedComWrappers.GetOrCreateComInterfaceForObject(widgetProviderFactory, CreateComInterfaceFlags.None), CLSCTX.CLSCTX_LOCAL_SERVER, REGCLS.REGCLS_MULTIPLEUSE, out uint registrationHandle);
            autoResetEvent.WaitOne();
            autoResetEvent.Dispose();
            Ole32Library.CoRevokeClassObject(registrationHandle);
        }

        /// <summary>
        /// 关闭小组件
        /// </summary>
        public static void CloseWidget()
        {
            autoResetEvent.Set();
        }
    }
}
