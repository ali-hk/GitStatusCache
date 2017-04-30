﻿using GitStatusCache;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitStatusCacheService.Controllers
{
    [Route("api/[controller]")]
    public class StatusController : Controller
    {
        // GET api/status?repoPath=repo-path
        [HttpGet]
        public RepositoryStatus Get([FromQuery]string repoPath)
        {
            //repoPath = @"E:\Git\git-status-cache";
            if (Program._cache.TryGetStatus(repoPath, out var status))
            {
                return status;
            }
            else
            {
                //Program._cache.SetStatus(repoPath, new RepositoryStatus());
                return null;
            }
        }


        // PUT api/status?repoPath=repo-path
        [HttpPut]
        public void Put([FromQuery]string repoPath, [FromBody]RepositoryStatus value)
        {
            Program._cache.SetStatus(repoPath, value);
        }
    }
}
