using System;
using System.Collections.Generic;
using System.Threading;

namespace MCAutoVote
{
    public class Loop
    {
        private List<Action> actions = new List<Action>();

        public Loop Add(Action action)
        {
            actions.Add(action);
            return this;
        }

        public void Run()
        {
            while(true)
            {
                foreach(Action action in actions)
                {
                    action();
                    Thread.Sleep(5);
                }
            }
        }
    }
}
