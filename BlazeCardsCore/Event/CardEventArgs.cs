using BlazeCardsCore.Descriptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Event
{
    public class CardEventArgs : EventArgs
    {
        public IList<Card> Cards { get; private set; }
        public CardEventArgs()
        {
            this.Cards = new List<Card>();
        }

        public CardEventArgs(IList<Card> cards)
        {
            this.Cards = cards;
        }
    }
}
