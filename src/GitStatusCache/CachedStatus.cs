namespace GitStatusCache
{
    internal class CachedStatus
    {
        private readonly bool _isValid;
        private readonly RepositoryStatus _status;

        public CachedStatus(bool isValid, RepositoryStatus status)
        {
            _isValid = isValid;
            _status = status;
        }

        public bool IsValid => _isValid;

        public RepositoryStatus Status => _status;

        public CachedStatus Invalidate()
        {
            return new CachedStatus(false, _status);
        }
        public CachedStatus Update(RepositoryStatus updatedStatus)
        {
            return new CachedStatus(true, updatedStatus);
        }
    }
}