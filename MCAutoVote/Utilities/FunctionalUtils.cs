using System;
using System.Threading;

namespace MCAutoVote.Utilities
{
    public static class FunctionalUtils
    {
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
