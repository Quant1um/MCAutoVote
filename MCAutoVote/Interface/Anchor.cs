using System;

namespace MCAutoVote.Interface
{
    public class Anchor
    {
        private readonly int left, top;

        public Anchor(int left, int top)
        {
            this.left = left;
            this.top = top;
        }

        public Anchor() : this(Console.CursorLeft, Console.CursorTop) { }

        public Anchor Set()
        {
            int tempL = Console.CursorLeft;
            int tempT = Console.CursorTop;
            Console.CursorLeft = left;
            Console.CursorTop = top;

            return new Anchor(tempL, tempT);
        }
    }
}
