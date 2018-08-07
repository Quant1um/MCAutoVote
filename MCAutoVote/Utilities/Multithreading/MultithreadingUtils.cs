using System;
using System.Threading;

namespace MCAutoVote.Utilities.Multithreading
{
    public static class MultithreadingUtils
    {
        public static void WaitWhile(Func<bool> result) => WaitWhile(result, long.MaxValue, 400);
        public static void WaitWhile(Func<bool> result, long timeout, int lockPeriod)
        {
            long time = timeout;
            while (result())
            {
                Thread.Sleep(lockPeriod);

                time -= lockPeriod;
                if (time < 0)
                    throw new TimeoutException($"WaitWhile() timeout of {timeout}ms exceeded!");
            }
        }
    }
}
