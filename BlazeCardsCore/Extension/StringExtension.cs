using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Extension
{
    public static class StringExtension
    {
        public static string RemoveLast(this string str)
        {
            if (str.Length <= 0) return String.Empty;
            return str.Substring(0, str.Length - 1);
        }
    }
}
