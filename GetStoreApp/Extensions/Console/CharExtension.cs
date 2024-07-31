﻿using GetStoreApp.Services.Root;
using System.Threading.Tasks;

namespace GetStoreApp.Extensions.Console
{
    /// <summary>
    /// 字符类型扩展方法
    /// </summary>
    public static class CharExtension
    {
        private static byte[] lengths;

        public static async Task InitializeAsync()
        {
            lengths = await ResourceService.GetEmbeddedDataAsync("Files/EmbedAssets/Lengths.bin");
        }

        /// <summary>
        /// 判断该字符在控制台显示的实际宽度是否大于1（速度更快版本）
        /// </summary>
        public static bool IsWideDisplayCharEx(char c)
        {
            int index = c;
            return (lengths[index / 8] & (1 << (index % 8))) is not 0;
        }

        /// <summary>
        /// 获取该字符在控制台显示的实际宽度（速度更快版本）
        /// </summary>
        public static int GetCharDisplayLengthEx(char c)
        {
            return IsWideDisplayCharEx(c) ? 2 : 1;
        }

        /// <summary>
        /// 获取字符串在控制台显示的实际宽度（速度更快版本）
        /// </summary>
        public static int GetStringDisplayLengthEx(string str)
        {
            int total = 0;
            foreach (char c in str)
            {
                total += GetCharDisplayLengthEx(c);
            }
            return total;
        }
    }
}
