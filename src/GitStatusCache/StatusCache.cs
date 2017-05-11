using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace GitStatusCache
{
    public class StatusCache
    {
        private const int BACKGROUND_UPDATE_DELAY_MS = 500;
        private readonly ReaderWriterLockSlim _watcherLock = new ReaderWriterLockSlim();
        private readonly Dictionary<string, RepositoryWatcher> _watcherMap = new Dictionary<string, RepositoryWatcher>();
        private readonly ConcurrentDictionary<string, CachedStatus> _statusMap = new ConcurrentDictionary<string, CachedStatus>();
        private readonly ConcurrentDictionary<string, ReaderWriterLockSlim> _statusLockMap = new ConcurrentDictionary<string, ReaderWriterLockSlim>();

        public bool TryGetStatus(string path, out RepositoryStatus status)
        {
            if (_statusMap.TryGetValue(path, out var cachedStatus) && cachedStatus.IsValid)
            {
                Debug.WriteLine("Cache hit");
                status = cachedStatus.Status;
                return true;
            }
            else
            {
                Debug.WriteLine("Cache miss");
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
            else
            {
                var addStatus = new CachedStatus(true, status);
                _statusMap.TryAdd(path, addStatus);
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
            if (changedPath != null && (changedPath.EndsWith("index.lock", StringComparison.OrdinalIgnoreCase) || changedPath.EndsWith(".git", StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }

            var watcher = watcherObj as RepositoryWatcher;
            var repoPath = watcher.RepositoryPath;
            if (_statusMap.TryGetValue(repoPath, out var status))
            {
                var newStatus = status.Invalidate();
                if (!_statusMap.TryUpdate(repoPath, newStatus, status))
                {
                    Debug.WriteLine("Cached status wasn't the same as retrieved status, didn't update validity.");
                }

                Task.Delay(BACKGROUND_UPDATE_DELAY_MS).ContinueWith(t =>
                {
                    ReaderWriterLockSlim statusLock = null;
                    if (!_statusLockMap.TryGetValue(repoPath, out statusLock))
                    {
                        statusLock = new ReaderWriterLockSlim();
                        _statusLockMap.TryAdd(repoPath, statusLock);
                    }

                    using (new ScopedReaderWriterLock(statusLock, ReaderWriterLockType.Writing))
                    {
                        if (!_statusMap.TryGetValue(repoPath, out var cachedStatus) || !cachedStatus.IsValid)
                        {
                            Debug.WriteLine("Calling git status");
                            var repoWorkingPath = GitHelper.GetWorkingPath(repoPath);
                            var process = Process.Start(new ProcessStartInfo { FileName = "git", Arguments = "-c core.quotepath=false -c color.status=false -c status.relativePaths=false status --no-lock-index --short --branch", CreateNoWindow = true, UseShellExecute = false, WorkingDirectory = repoWorkingPath, RedirectStandardOutput = true });
                            var output = process.StandardOutput.ReadToEnd();
                            process.WaitForExit();

                            Debug.WriteLine("git status completed");
                            var updatedStatus = GitHelper.ParseStatusOutput(output);
                            _statusMap.TryUpdate(repoPath, cachedStatus.Update(updatedStatus), cachedStatus);
                        }
                    }
                });
            }
        }
    }
}
