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

        public CanvasComponent Canvas { get; private set; }
        public MouseState Mouse { get; private set; }
        public TouchState Touch { get; private set; }
        public KeyboardState Keyboard { get; private set; }
        public InteropQueueState InteropQueue { get; private set; }
        public StorageState Storage { get; private set; }
        public CharacterState Character { get; private set; }



        public IList<Card> Selected { get; set; }


        public RectCard Highlighter { get; set; }
        public RectCard Selector { get; set; }

        public CardState(CanvasComponent canvas)
        {
            this.Canvas = canvas;

            this.Selected = new List<Card>();
            this.Mouse = new MouseState(this);
            this.Touch = new TouchState(this);
            this.Keyboard = new KeyboardState(this);
            this.InteropQueue = new InteropQueueState(this);
            this.Storage = new StorageState(this);
            this.Character = new CharacterState(this);

            this.Selector = RectFactory.CreateSelector(Vector2f.Zero);
            this.Selector.Visible = false;
        }

        public void Deselect()
        {
            this.Highlighter = null;

            foreach (var card in this.Selected)
                card.Deselect();

            this.Selected.Clear();

            this.Canvas.InvokeChange();
        }
    }
}
