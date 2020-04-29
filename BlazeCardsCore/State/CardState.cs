using BlazeCardsCore.Components;
using BlazeCardsCore.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.State
{
    public class CardState
    {
        // because of broken event propagation
        public bool ComponentClicked { get; set; }


        public MouseState Mouse { get; private set; }


        // EXCHANGE FOR CARD!
        //public CardComponent Selected { get; set; }
        //public Card Selected { get; set; }
        public IList<Card> Selected { get; set; }


        public RectCard Highlighter { get; set; }
        public RectCard Selector { get; set; }

        public CardState()
        {
            this.Selected = new List<Card>();
            this.Mouse = new MouseState(this);
        }

        public void Deselect()
        {
            this.Highlighter = null;
            this.Selected.Clear();
        }
    }
}
