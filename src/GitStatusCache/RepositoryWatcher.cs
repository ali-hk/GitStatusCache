using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace GitStatusCache
{
    public class RepositoryWatcher : IRepositoryWatcher
    {
        private readonly string _path;
        private readonly FileSystemWatcher _watcher;

        public RepositoryWatcher(string path)
        {
            _path = path;
            _watcher = new FileSystemWatcher(path);
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Size;
            _watcher.Changed += OnChanged;
            _watcher.Created += OnCreated;
            _watcher.Deleted += OnDeleted;
            _watcher.Error += OnError;
            _watcher.Renamed += OnRenamed;
            _watcher.EnableRaisingEvents = true;
        }

        public event EventHandler<string> RepositoryChanged;

        public string RepositoryPath => _path;

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            RepositoryChanged?.Invoke(this, e.FullPath);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            RepositoryChanged?.Invoke(this, e.FullPath);
        }

        private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            RepositoryChanged?.Invoke(this, e.FullPath);
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            RepositoryChanged?.Invoke(this, null);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            RepositoryChanged?.Invoke(this, e.FullPath);
        }
    }
}
