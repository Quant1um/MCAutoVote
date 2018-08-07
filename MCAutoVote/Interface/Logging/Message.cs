using System;

namespace MCAutoVote.Interface.Logging
{
    public class Message
    {
        public string Text { get; private set; } = "";
        public object Object { get; private set; } = null;
        public Level LoggingLevel { get; private set; } = Logging.Level.Echo;
        public ConsoleColor TextColor { get; private set; } = Console.ForegroundColor;
        public DateTime Time { get; } = DateTime.Now;

        public Message Append(string text)
        {
            Text += text;
            return this;
        }

        public Message Format(string text, params object[] param) =>
            Append(string.Format(text, param));

        public Message Level(Level level)
        {
            LoggingLevel = level;
            return this;
        }

        public Message Color(ConsoleColor color)
        {
            TextColor = color;
            return this;
        }

        public Message Data(object obj)
        {
            Object = obj;
            return this;
        }
    }
}
