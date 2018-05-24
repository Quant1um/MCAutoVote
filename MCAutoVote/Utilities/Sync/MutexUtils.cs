using System;
using System.Threading;

namespace MCAutoVote.Utilities.Sync
{
    public static class MutexUtils
    {
        private class MutexHandle : IDisposable
        {
            private Mutex mutex;
            public MutexHandle(Mutex mutex)
            {
                this.mutex = mutex;
                mutex.WaitOne();
            }

            void IDisposable.Dispose() => mutex.ReleaseMutex();
        }

        public static IDisposable Use(this Mutex mutex) => new MutexHandle(mutex);
    }
}
