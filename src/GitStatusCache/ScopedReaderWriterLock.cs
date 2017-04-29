using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GitStatusCache
{
    public enum ReaderWriterLockType
    {
        Reading,
        Writing,
        ReadingUpgradeable
    }

    public class ScopedReaderWriterLock : IDisposable
    {
        private readonly ReaderWriterLockType _type;
        private ReaderWriterLockSlim _lock;

        public ScopedReaderWriterLock(
            ReaderWriterLockSlim rwLock,
            ReaderWriterLockType type
            )
        {
            _lock = rwLock;
            _type = type;

            if (_type == ReaderWriterLockType.Reading)
            {
                _lock.EnterReadLock();
            }
            else if (_type == ReaderWriterLockType.ReadingUpgradeable)
            {
                _lock.EnterUpgradeableReadLock();
            }
            else
            {
                _lock.EnterWriteLock();
            }
        }

        public void UpgradeToWriteLock()
        {
            if (_type == ReaderWriterLockType.ReadingUpgradeable)
            {
                _lock.EnterWriteLock();
            }
            else
            {
                throw new InvalidOperationException("Writer lock is not upgradeable");
            }
        }

        ~ScopedReaderWriterLock()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_lock == null)
            {
                return;
            }

            if (_type == ReaderWriterLockType.Reading)
            {
                _lock.ExitReadLock();
            }
            else if (_type == ReaderWriterLockType.ReadingUpgradeable)
            {
                if (_lock.IsWriteLockHeld)
                {
                    _lock.ExitWriteLock();
                }
                _lock.ExitUpgradeableReadLock();
            }
            else
            {
                _lock.ExitWriteLock();
            }

            _lock = null;
        }
    }
}
