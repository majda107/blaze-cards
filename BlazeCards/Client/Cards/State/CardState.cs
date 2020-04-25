using BlazeCards.Client.Cards.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.State
{
    public class CardState
    {
        public MouseState Mouse { get; private set; }

        public CardComponent Selected { get; set; }

        public CardState()
        {
            this.Mouse = new MouseState(this);
        }
    }
}
