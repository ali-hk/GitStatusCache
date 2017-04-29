using System;
using System.Collections.Generic;
using System.IO;

namespace GitStatusCache
{
    public class StatusCache
    {
        private readonly Dictionary<string, RepositoryWatcher> _watcherMap = new Dictionary<string, RepositoryWatcher>();

        public RepositoryStatus GetStatus(string path)
        {
            throw new NotImplementedException();
        }
    }
}
