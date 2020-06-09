using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Event
{
    public class KeyEventArgs: EventArgs
    {
        public string Key { get; private set; }
        public KeyEventArgs(string key)
        {
            this.Key = key;
        }
    }
}
