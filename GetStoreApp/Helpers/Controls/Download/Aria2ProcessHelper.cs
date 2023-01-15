﻿using GetStoreApp.WindowsAPI.PInvoke.Kernel32;
using GetStoreApp.WindowsAPI.PInvoke.NTdll;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace GetStoreApp.Helpers.Controls.Download
{
    /// <summary>
    /// Aria2进程服务辅助类
    /// </summary>
    public static class Aria2ProcessHelper
    {
        // Aria2下载进程的ID号
        private static int Aria2ProcessID = default;

        /// <summary>
        /// 获取当前进程的PID信息
        /// </summary>
        public static int GetProcessID()
        {
            return Aria2ProcessID;
        }

        /// <summary>
        /// 让Aria2以RPC方式启动，并让其在后台运行
        /// </summary>
        public static async Task RunAria2Async(string fileName, string arguments)
        {
            //设置启动程序的信息
            ProcessStartInfo Aria2Info = new ProcessStartInfo
            {
                //设置外部程序名
                FileName = fileName,

                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,

                //最小化方式启动
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = arguments
            };

            // 启动Aria2下载进程，并设置进程ID号
            Aria2ProcessID = Process.Start(Aria2Info).Id;
            await Task.CompletedTask;
        }

        /// <summary>
        /// 应用程序关闭时，需要终止相应的Aria2进程树
        /// </summary>
        public static void KillProcessAndChildren(int processID)
        {
            GetChildProcessIds(processID).ForEach(childProcessId =>
            {
                using (Process child = Process.GetProcessById(childProcessId))
                {
                    child.Kill();
                }
            });

            using (Process thisProcess = Process.GetProcessById(processID))
            {
                thisProcess.Kill();
            }
        }

        /// <summary>
        /// 获取所有的子进程ID
        /// </summary>
        public static List<int> GetChildProcessIds(int parentProcessId)
        {
            List<int> myChildrenProcessList = new List<int>();
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc is null)
                {
                    return new List<int>();
                }

                int currentProcessId = proc.Id;
                proc.Dispose();
                if (parentProcessId == GetParentProcessId(currentProcessId))
                {
                    // 添加自身
                    myChildrenProcessList.Add(currentProcessId);
                    // 添加和其自身有关的所有子进程
                    myChildrenProcessList.AddRange(GetChildProcessIds(currentProcessId));
                }
            }

            return myChildrenProcessList;
        }

        /// <summary>
        /// 获取父进程ID
        /// </summary>
        public static int GetParentProcessId(int processId)
        {
            int ParentID = 0;
            int hProcess = Kernel32Library.OpenProcess(EDesiredAccess.PROCESS_QUERY_INFORMATION, false, processId);
            if (hProcess is not 0)
            {
                try
                {
                    PROCESS_BASIC_INFORMATION pbi = new PROCESS_BASIC_INFORMATION();
                    int pSize = 0;
                    if (NTdllLibrary.NtQueryInformationProcess(hProcess, PROCESSINFOCLASS.ProcessBasicInformation, ref pbi, pbi.Size, ref pSize) is not -1)
                    {
                        ParentID = pbi.InheritedFromUniqueProcessId;
                    }
                }
                finally
                {
                    Kernel32Library.CloseHandle(hProcess);
                }
            }

            return (ParentID);
        }

        /// <summary>
        /// 判断Aria2进程是否存活
        /// </summary>
        public static bool IsAria2ProcessExisted()
        {
            try
            {
                if (GetProcessID() != default)
                {
                    Process.GetProcessById(GetProcessID());
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
