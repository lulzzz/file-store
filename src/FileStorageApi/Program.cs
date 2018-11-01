﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.ServiceFabric.Services.Runtime;

namespace FileStorageApi
{
    internal static class Program
    {
        /// <summary>
        /// 这是服务主机进程的入口点。
        /// </summary>
        private static void Main()
        {
#if DEBUG
            DebugHost();
            return;
#endif

            try
            {
                // ServiceManifest.XML 文件定义一个或多个服务类型名称。
                // 注册服务会将服务类型名称映射到 .NET 类型。
                // 在 Service Fabric 创建此服务类型的实例时，
                // 会在此主机进程中创建类的实例。

                ServiceRuntime.RegisterServiceAsync("FileStorageApiType",
                    context => new FileStorageApi(context)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(FileStorageApi).Name);

                // 防止此主机进程终止，以使服务保持运行。 
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void DebugHost()
        {
            WebHost.CreateDefaultBuilder()
                .UseStartup<Startup>()
                .Build()
                .Run();
        }
    }
}