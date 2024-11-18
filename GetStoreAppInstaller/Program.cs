﻿using GetStoreAppInstaller.WindowsAPI.ComTypes;
using GetStoreAppInstaller.WindowsAPI.PInvoke.Comctl32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.User32;
using GetStoreAppInstaller.WindowsAPI.PInvoke.WindowsUI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Graphics;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using WinRT;

// 抑制 CA1806 警告
#pragma warning disable CA1806

namespace GetStoreAppInstaller
{
    /// <summary>
    /// 获取商店应用 应用安装器
    /// </summary>
    public class Program
    {
        private static SUBCLASSPROC mainWindowSubClassProc;

        public static AppWindow MainAppWindow { get; private set; }

        private static AppWindow CoreAppWindow { get; set; }

        [STAThread]
        public static void Main()
        {
            ComWrappersSupport.InitializeComWrappers();

            // 应用主窗口
            MainAppWindow = AppWindow.Create();
            MainAppWindow.Title = "获取商店应用 应用安装器";
            MainAppWindow.TitleBar.ExtendsContentIntoTitleBar = true;
            MainAppWindow.Changed += OnAppWindowChanged;
            MainAppWindow.Closing += OnAppWindowClosing;

            // 创建 CoreWindow
            WindowsUILibrary.PrivateCreateCoreWindow(WINDOW_TYPE.IMMERSIVE_HOSTED, "XamlContentCoreWindow", 0, 0, 0, 0, 0, Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), typeof(ICoreWindow).GUID, out IntPtr coreWindowPtr);
            CoreWindow coreWindow = CoreWindow.FromAbi(coreWindowPtr);
            coreWindow.As<ICoreWindowInterop>().GetWindowHandle(out IntPtr coreWindowHandle);
            CoreAppWindow = AppWindow.GetFromWindowId(Win32Interop.GetWindowIdFromWindow(coreWindowHandle));
            CoreAppWindow.Move(new PointInt32());
            CoreAppWindow.Resize(MainAppWindow.Size);
            User32Library.SetParent(coreWindowHandle, Win32Interop.GetWindowFromWindowId(MainAppWindow.Id));
            SynchronizationContext.SetSynchronizationContext(new DispatcherQueueSynchronizationContext(coreWindow.DispatcherQueue));
            new XamlIslandsApp();

            mainWindowSubClassProc = new SUBCLASSPROC(MainWindowSubClassProc);
            Comctl32Library.SetWindowSubclass(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), mainWindowSubClassProc, 0, IntPtr.Zero);

            CoreApplication.As<ICoreApplicationPrivate2>().CreateNonImmersiveView(out IntPtr coreApplicationViewPtr);
            CoreApplicationView coreApplicationView = CoreApplicationView.FromAbi(coreApplicationViewPtr);

            int style = GetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE);
            style |= (int)WindowStyle.WS_CHILD;
            style &= ~unchecked((int)WindowStyle.WS_POPUP);
            SetWindowLongAuto(coreWindowHandle, WindowLongIndexFlags.GWL_STYLE, style);

            FrameworkView frameworkView = new();
            frameworkView.Initialize(coreApplicationView);
            frameworkView.SetWindow(coreWindow);

            SetAppIcon(MainAppWindow);
            MainAppWindow.Show();
            Application.Current.Resources = new XamlControlsResources();
            Window.Current.Content = new MainPage();
            frameworkView.Run();
        }

        /// <summary>
        /// 窗口位置变化发生的事件
        /// </summary>
        private static void OnAppWindowChanged(AppWindow sender, AppWindowChangedEventArgs args)
        {
            if (args.DidSizeChange)
            {
                CoreAppWindow.Resize(sender.Size);
            }
            else if (args.DidPositionChange)
            {
                User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), WindowMessage.WM_MOVE, UIntPtr.Zero, IntPtr.Zero);
            }
        }

        /// <summary>
        /// 关闭窗口之后关闭其他服务
        /// </summary>
        private static void OnAppWindowClosing(AppWindow sender, AppWindowClosingEventArgs args)
        {
            MainAppWindow.Changed -= OnAppWindowChanged;
            Comctl32Library.RemoveWindowSubclass(Win32Interop.GetWindowFromWindowId(MainAppWindow.Id), mainWindowSubClassProc, 0);
            Window.Current.CoreWindow.Close();
        }

        /// <summary>
        /// 应用主窗口消息处理
        /// </summary>
        private static IntPtr MainWindowSubClassProc(IntPtr hWnd, WindowMessage Msg, UIntPtr wParam, IntPtr lParam, uint uIdSubclass, IntPtr dwRefData)
        {
            switch (Msg)
            {
                // 窗口激活状态发生更改时的消息
                case WindowMessage.WM_ACTIVATE:
                    {
                        User32Library.SendMessage(Win32Interop.GetWindowFromWindowId(CoreAppWindow.Id), Msg, wParam, lParam);
                        break;
                    }
            }

            return Comctl32Library.DefSubclassProc(hWnd, Msg, wParam, lParam);
        }

        /// <summary>
        /// 设置应用窗口图标
        /// </summary>
        private static void SetAppIcon(AppWindow appWindow)
        {
            // 选中文件中的图标总数
            int iconTotalCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 0, 0, null, null, 0, 0);

            // 用于接收获取到的图标指针
            IntPtr[] hIcons = new IntPtr[iconTotalCount];

            // 对应的图标id
            int[] ids = new int[iconTotalCount];

            // 成功获取到的图标个数
            int successCount = User32Library.PrivateExtractIcons(Environment.ProcessPath, 0, 256, 256, hIcons, ids, iconTotalCount, 0);

            // GetStoreApp.exe 应用程序只有一个图标
            if (successCount >= 1 && hIcons[0] != IntPtr.Zero)
            {
                appWindow.SetIcon(Win32Interop.GetIconIdFromIcon(hIcons[0]));
            }
        }

        /// <summary>
        /// 获取窗口属性
        /// </summary>
        private static int GetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.GetWindowLongPtr(hWnd, nIndex);
            }
            else
            {
                return User32Library.GetWindowLong(hWnd, nIndex);
            }
        }

        /// <summary>
        /// 更改窗口属性
        /// </summary>
        private static IntPtr SetWindowLongAuto(IntPtr hWnd, WindowLongIndexFlags nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size is 8)
            {
                return User32Library.SetWindowLongPtr(hWnd, nIndex, dwNewLong);
            }
            else
            {
                return User32Library.SetWindowLong(hWnd, nIndex, dwNewLong);
            }
        }
    }
}
