using BlazeCardsCore.Event;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Descriptors
{
    public class DropAreaCard : RectCard
    {
        public delegate void DropEventHandler(DropAreaCard sender, CardEventArgs args);

        public event DropEventHandler OnDrop;
        public DropAreaCard()
        {
            this.Classes.Add("blaze-droparea");
        }

        public void FireDrop(IList<Card> droppedCards)
        {
            this.OnDrop?.Invoke(this, new CardEventArgs(droppedCards));
        }
    }
}
