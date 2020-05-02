using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.State
{
    public class EventPrevention
    {
        private static int mutex = 0;

        public static bool EventsEnabled { get => mutex == 0; }
        [JSInvokable]
        public static void DisableEvents()
        {
            //Console.WriteLine("DISABLING EVENTS IN C#!!!!!!!!!");
            mutex += 1;
        }

        [JSInvokable]
        public static void EnableEvents()
        {
            mutex -= 1;
        }
    }
}
