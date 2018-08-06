using System;
using System.Threading;

namespace MCAutoVote.Utilities.Multithreading
{
    public static class MultithreadingUtils
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

        public static void WaitWhile(Func<bool> result) => WaitWhile(result, long.MaxValue, 400);
        public static void WaitWhile(Func<bool> result, long timeout, int lockPeriod)
        {
            while (result())
            {
                Thread.Sleep(lockPeriod);

                timeout -= lockPeriod;
                if (timeout < 0)
                    throw new TimeoutException();
            }
        }
    }
}
