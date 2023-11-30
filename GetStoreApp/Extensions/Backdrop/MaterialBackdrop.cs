﻿using GetStoreApp.Services.Root;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;

namespace GetStoreApp.Extensions.Backdrop
{
    /// <summary>
    /// 自定义扩展的背景色
    /// </summary>
    public sealed class MaterialBackdrop : SystemBackdrop
    {
        private bool isMicaBackdrop;
        private readonly MicaKind micaBackdropKind;
        private readonly DesktopAcrylicKind desktopAcrylicBackdropKind;
        private ISystemBackdropControllerWithTargets systemBackdropController;

        public SystemBackdropConfiguration BackdropConfiguration { get; private set; }

        public MaterialBackdrop(MicaKind micaKind)
        {
            isMicaBackdrop = true;
            micaBackdropKind = micaKind;
        }

        public MaterialBackdrop(DesktopAcrylicKind desktopAcrylicKind)
        {
            isMicaBackdrop = false;
            desktopAcrylicBackdropKind = desktopAcrylicKind;
        }

        /// <summary>
        /// 当此对象附加到有效容器时调用
        /// </summary>
        protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
        {
            base.OnTargetConnected(connectedTarget, xamlRoot);

            if (systemBackdropController is not null)
            {
                throw new ApplicationException(ResourceService.GetLocalized("Resources/SystemBackdropControllerInitializeFailed"));
            }

            if (isMicaBackdrop)
            {
                systemBackdropController = new MicaController() { Kind = micaBackdropKind };
                systemBackdropController.AddSystemBackdropTarget(connectedTarget);
                BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
                systemBackdropController.SetSystemBackdropConfiguration(BackdropConfiguration);
            }
            else
            {
                systemBackdropController = new DesktopAcrylicController() { Kind = desktopAcrylicBackdropKind };
                systemBackdropController.AddSystemBackdropTarget(connectedTarget);
                BackdropConfiguration = GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot);
                systemBackdropController.SetSystemBackdropConfiguration(BackdropConfiguration);
            }
        }

        /// <summary>
        /// 从其容器中清除此对象时调用
        /// </summary>
        protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
        {
            base.OnTargetDisconnected(disconnectedTarget);

            if (systemBackdropController is not null)
            {
                systemBackdropController.RemoveSystemBackdropTarget(disconnectedTarget);
                systemBackdropController = null;
            }
        }
    }
}
