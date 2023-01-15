﻿namespace GetStoreApp.Models.Dialogs.CommonDialogs.About
{
    /// <summary>
    /// 应用启动参数数据模型
    /// </summary>
    public class StartupArgsModel
    {
        /// <summary>
        /// 应用启动参数名称
        /// </summary>
        public string ArgumentName { get; set; }

        /// <summary>
        /// 具体的应用启动参数
        /// </summary>
        public string Argument { get; set; }

        /// <summary>
        /// 应用启动参数是否必需要输入
        /// </summary>
        public string IsRequired { get; set; }

        /// <summary>
        /// 应用启动参数具体内容
        /// </summary>
        public string ArgumentContent { get; set; }
    }
}
