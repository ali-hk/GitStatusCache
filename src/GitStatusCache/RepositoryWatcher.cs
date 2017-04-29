using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitStatusCache
{
    public class RepositoryWatcher : IRepositoryWatcher
    {
        private readonly FileSystemWatcher _watcher;

        public RepositoryWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path);
            _watcher.IncludeSubdirectories = true;
        }
    }
}
