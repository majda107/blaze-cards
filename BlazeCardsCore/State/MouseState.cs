using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Factories;
using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.State
{
    public class MouseState
    {
        public CardState CardState { get; private set; }
        public bool Down { get; private set; }

        public Vector2f Scroll { get; private set; }
        public float Zoom { get; set; }


        private Vector2f lastPosition;

        public MouseState(CardState state)
        {
            this.CardState = state;
            this.Down = false;

            this.lastPosition = Vector2f.Zero;
            this.Scroll = Vector2f.Zero;
            this.Zoom = 1.0f;
        }

        public void CheckDrop()
        {
            if (this.CardState.Highlighter == null) return;

            // TEST DROP
            var box = BoundingRect.FromPositionSize(this.CardState.Highlighter.GetGlobalPosition(), this.CardState.Highlighter.GetSize());
            var droppables = new List<Card>();
            foreach (var card in this.CardState.Canvas.Cards) // exchange for cardstate.cards
                card.TraverseCard((c) => c.GetType() == typeof(DropAreaCard), droppables);

            Console.WriteLine($"Found {droppables.Count} droppables");

            var dropped = new List<Card>();
            foreach (var droppable in droppables)
            {
                dropped.Clear();
                droppable.TraverseTouches(box, dropped);

                if (dropped.Count > 0)
                    (droppable as DropAreaCard).FireDrop(this.CardState.Selected);
            }
        }

        public void CheckSelector()
        {
            if (this.CardState.Selector == null || !this.CardState.Selector.Visible) return;

            var selectorBox = BoundingRect.FromPositionSize(this.CardState.Selector.GetGlobalPosition(), this.CardState.Selector.GetSize());
            var traversed = new List<Card>();
            foreach (var card in this.CardState.Canvas.Cards)
                card.TraverseOverlap(selectorBox, traversed);

            if (traversed.Count > 0)
            {
                //Console.WriteLine($"Selecting {traversed.Count} items...");

                foreach (var traversedCard in traversed)
                    this.CardState.Selected.Add(traversedCard);

                this.CardState.Highlighter = RectFactory.CreateHighlighter(traversed);
            }

            this.CardState.Selector.Visible = false;
            this.CardState.Selector.Component.InvokeChange();
        }

        public void OnDown(Vector2f position)
        {
            //position -= this.Scroll;

            this.Down = true;
            //Console.WriteLine($"Mouse down: {position.X}");

            this.lastPosition = position;

            //if (this.CardState.Selected != null)
            //    this.CardState.Selected.Position.Position = position;
        }

        public void OnMove(Vector2f position)
        {
            if (!this.Down) return;

            var dev = position - this.lastPosition;
            dev /= this.Zoom;

            //Console.WriteLine($"Mouse move: {position.X}");


            if (this.CardState.Selected.Count > 0)
            {

                //var minPos = new Vector2f(float.MaxValue, float.MaxValue);
                foreach (var card in this.CardState.Selected)
                {
                    card.PositionBehavior.Position += dev;
                    //this.CardState.InteropQueue.QueueChange(new PositionChange(card.Component.Graphics, card.GetPosition()));


                    //var pos = card.GetGlobalPosition();
                    //if (pos.X < minPos.X) minPos.X = pos.X;
                    //if (pos.Y < minPos.Y) minPos.Y = pos.Y;
                }

                if (this.CardState.Highlighter != null)
                    this.CardState.Highlighter.PositionBehavior.Position += dev;

                this.CardState.InteropQueue.Flush(this.CardState.Canvas.JSRuntime);
            }




            if (this.CardState.Selector.Visible)
                this.CardState.Selector.SizeBehavior.Size += dev;

            if (!this.CardState.Selector.Visible && this.CardState.Selected.Count <= 0)
            {
                this.Scroll += dev;
                this.CardState.Canvas.Translate();
            }


            this.lastPosition = position;

        }

        public void OnUp(Vector2f position)
        {
            //position -= this.Scroll;
            if (this.CardState.Selected.Count > 0)
            {
                foreach (var card in this.CardState.Selected)
                    card.Snap();

                this.CardState.InteropQueue.Flush(this.CardState.Canvas.JSRuntime);

                this.CardState.Highlighter = RectFactory.CreateHighlighter(this.CardState.Selected);

            }


            this.Down = false;
        }
    }
}
