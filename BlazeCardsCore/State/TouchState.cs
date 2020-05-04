using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.State
{
    public class TouchState
    {
        public CardState State { get; private set; }
        public double LastPinchHypot { get; set; }
        public TouchState(CardState state)
        {
            this.State = state;
        }
    }
}
