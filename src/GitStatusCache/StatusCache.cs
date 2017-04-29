using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace GitStatusCache
{
    public class StatusCache
    {
        private readonly ReaderWriterLockSlim _watcherLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, RepositoryWatcher> _watcherMap = new Dictionary<string, RepositoryWatcher>();
        private readonly ConcurrentDictionary<string, CachedStatus> _statusMap = new ConcurrentDictionary<string, CachedStatus>();

        public bool TryGetStatus(string path, out RepositoryStatus status)
        {
            if (_statusMap.TryGetValue(path, out var cachedStatus) && cachedStatus.IsValid)
            {
                status = cachedStatus.Status;
                return true;
            }
            else
            {
                WatchRepository(path);
                status = null;
                return false;
            }
        }

        public void SetStatus(string path, RepositoryStatus status)
        {
            if (_statusMap.TryGetValue(path, out var cachedStatus))
            {
                 var newStatus = cachedStatus.Update(status);
                if (!_statusMap.TryUpdate(path, newStatus, cachedStatus))
                {
                    Debug.WriteLine("Cached status wasn't the same as retrieved status, didn't update status.");
                }
            }
        }

        private void WatchRepository(string path)
        {
            RepositoryWatcher watcher = null;
            using (var rwLock = new ScopedReaderWriterLock(_watcherLock, ReaderWriterLockType.ReadingUpgradeable))
            {
                if (!_watcherMap.ContainsKey(path))
                {
                    rwLock.UpgradeToWriteLock();
                    watcher = new RepositoryWatcher(path);
                    _watcherMap.Add(path, watcher);
                }
                else
                {
                    return;
                }
            }

            watcher.RepositoryChanged += OnRepositoryChanged;
        }

        private void OnRepositoryChanged(object watcherObj, string changedPath)
        {
            var watcher = watcherObj as RepositoryWatcher;
            var repoPath = watcher.RepositoryPath;
            if(_statusMap.TryGetValue(repoPath, out var status))
            {
                var newStatus = status.Invalidate();
                if(!_statusMap.TryUpdate(repoPath, newStatus, status))
                {
                    Debug.WriteLine("Cached status wasn't the same as retrieved status, didn't update validity.");
                }
            }
        }
    }
}
