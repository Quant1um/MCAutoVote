/*
The MIT License (MIT)

Copyright (c) 2017 Toni Solarin-Sodara

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Internal.ReadLine;
using Internal.ReadLine.Abstractions;
using MCAutoVote.CLI.Command;
using System.Collections.Generic;

namespace System
{
    public static class ReadLine
    {
        private static List<string> _history;

        static ReadLine()
        {
            _history = new List<string>();
        }

        public static void AddHistory(params string[] text) => _history.AddRange(text);
        public static List<string> GetHistory() => _history;
        public static void ClearHistory() => _history = new List<string>();

        public static bool HistoryEnabled { get; set; } = true;
        public static IAutoCompleteHandler AutoCompletionHandler { private get; set; } = CommandRegistry.Autocompletion;

        public static KeyHandler CreateHandler()
        {
            return new KeyHandler(new Console2(), _history, AutoCompletionHandler);
        }

    }
}
