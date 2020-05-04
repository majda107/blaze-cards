using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Extension
{
    public static class FloatExtension
    {
        public static string ToJSStr(this float value)
        {
            return value.ToString("0.0").Replace(',', '.');
        }
    }
}
