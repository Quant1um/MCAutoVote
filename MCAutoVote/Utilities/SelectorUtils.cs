using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MCAutoVote.Utilities
{
    public static class SelectorUtils
    {
        public static IEnumerable<HtmlElement> GetElementsByClass(this IEnumerable<HtmlElement> doc, string clazz)
        {
            return doc.Where(CreateClassPredicate(clazz));
        }

        public static IEnumerable<HtmlElement> GetElementsByAttribute(this IEnumerable<HtmlElement> doc, string attr, string val)
        {
            return doc.Where(elem => elem.GetAttribute(attr) == val);
        }

        public static IEnumerable<HtmlElement> GetElementsByName(this IEnumerable<HtmlElement> doc, string name)
        {
            return GetElementsByAttribute(doc, "name", name);
        }

        public static IEnumerable<HtmlElement> GetElementsById(this IEnumerable<HtmlElement> doc, string id)
        {
            return GetElementsByAttribute(doc, "id", id);
        }

        public static IEnumerable<HtmlElement> GetElementsByAttribute(this HtmlElementCollection doc, string attr, string val)
        {
            return GetElementsByAttribute(doc.NonBlockCopy(), attr, val);
        }

        public static IEnumerable<HtmlElement> GetElementsByClass(this HtmlElementCollection doc, string clazz)
        {
            return GetElementsByClass(doc.NonBlockCopy(), clazz);
        }

        public static IEnumerable<HtmlElement> GetElementsByName(this HtmlElementCollection doc, string name)
        {
            return GetElementsByName(doc.NonBlockCopy(), name);
        }

        public static IEnumerable<HtmlElement> GetElementsById(this HtmlElementCollection doc, string id)
        {
            return GetElementsById(doc.NonBlockCopy(), id);
        }

        private static IEnumerable<HtmlElement> NonBlockCopy(this HtmlElementCollection collection)
        {
            HashSet<HtmlElement> elems = new HashSet<HtmlElement>();
            try
            {
                for (int i = 0; i < collection.Count; i++)
                    elems.Add(collection[i]);
            }
            catch (ArgumentOutOfRangeException) { } //sometimes, when loop reaches end, something can remove element => it can crash application. so we catch exception and do not care about remove element
            return elems;
        }

        public static Func<HtmlElement, bool> CreateClassPredicate(string clazz)
        {
            return (elem) => elem.GetAttribute("className").Split(' ').Contains(clazz);
        }
    }
}
