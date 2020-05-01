using BlazeCardsCore.Components;
using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using BlazeCardsCore.Models;
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


        public CanvasComponent Canvas { get; private set; }
        public MouseState Mouse { get; private set; }
        public KeyboardState Keyboard { get; private set; }
        public InteropQueueState InteropQueue { get; private set; }
        public StorageState Storage { get; private set; }


        // EXCHANGE FOR CARD!
        //public CardComponent Selected { get; set; }
        //public Card Selected { get; set; }
        public IList<Card> Selected { get; set; }


        public RectCard Highlighter { get; set; }
        public RectCard Selector { get; set; }

        public CardState(CanvasComponent canvas)
        {
            this.Canvas = canvas;

            this.Selected = new List<Card>();
            this.Mouse = new MouseState(this);
            this.Keyboard = new KeyboardState(this);
            this.InteropQueue = new InteropQueueState(this);
            this.Storage = new StorageState(this);

            this.Selector = RectFactory.CreateSelector(Vector2f.Zero);
            this.Selector.Visible = false;
        }

        public void Deselect()
        {
            this.Highlighter = null;
            this.Selected.Clear();
            this.Canvas.InvokeChange();
        }
    }
}
