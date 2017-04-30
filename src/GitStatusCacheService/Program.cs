﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using GitStatusCache;

namespace GitStatusCacheService
{
    public class Program
    {
        public static StatusCache _cache;

        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            _cache = new StatusCache();
            //_cache.TryGetStatus(@"E:\Git\git-status-cache", out var status);

            host.Run();
        }
    }
}
