using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.State
{
    public class KeyboardState
    {
        public CardState State { get; private set; }
        public List<string> KeysDown { get; private set; }

        public KeyboardState(CardState state)
        {
            this.State = state;
            this.KeysDown = new List<string>();
        }


        [JSInvokable]
        public void KeyDown(string key)
        {
            if (this.KeysDown.Contains(key)) return;
            this.KeysDown.Add(key);

            //Console.WriteLine($"Key down... {key}");
        }


        [JSInvokable]
        public void KeyUp(string key)
        {
            if (!this.KeysDown.Contains(key)) return;
            this.KeysDown.Remove(key);

            //Console.WriteLine($"Key up... {key}");
        }

        public bool IsDown(string key) => this.KeysDown.Contains(key);
    }
}
