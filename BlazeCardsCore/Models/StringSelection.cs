using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Net.Security;
using System.Text;

namespace BlazeCardsCore.Models
{
    public struct StringSelection
    {
        public static readonly StringSelection Empty = new StringSelection(-1, -1);
        public int Start { get; set; }
        public int Length { get; set; }

        public StringSelection(int start, int length)
        {
            this.Start = start;
            this.Length = length;
        }

        public string Cut(string input) => input.Substring(this.Start, this.Length);
        public string RemoveFrom(string baseString) => baseString.Remove(this.Start, this.Length);



        public override bool Equals(object obj)
        {
            var sel = (StringSelection)obj;
            return this.Start == sel.Start && this.Length == this.Length;
        }

        public override int GetHashCode()
        {
            return this.Start * this.Length + this.Length;
        }

        public static bool operator==(StringSelection s1, StringSelection s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator !=(StringSelection s1, StringSelection s2)
        {
            return !s1.Equals(s2);
        }
    }
}
