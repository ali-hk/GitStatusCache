using System;

namespace GitStatusCache
{
    public interface IRepositoryWatcher
    {
        event EventHandler<string> RepositoryChanged;
    }
}