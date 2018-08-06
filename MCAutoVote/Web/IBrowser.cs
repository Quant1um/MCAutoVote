using MSHTML;
using System;

namespace MCAutoVote.Web
{
    public interface IBrowser
    {
        Uri Url { get; }
        bool Completed { get; }

        IHTMLElement QuerySelector(string selector);
        IHTMLDOMChildrenCollection QuerySelectorAll(string selector);

        void Navigate(Uri uri);
        void Navigate(string uri);

        void WaitComplete(long timeout = long.MaxValue);
    }
}
