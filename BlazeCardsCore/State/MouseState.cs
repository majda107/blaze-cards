﻿using BlazeCardsCore.Descriptors;
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
        public CardState State { get; private set; }
        public bool Down { get; private set; }



        private Vector2f scroll;
        public Vector2f Scroll
        {
            get => this.scroll;
            private set
            {
                this.scroll = this.Scrollable ? value : Vector2f.Zero;
            }
        }


        private float zoom;
        public float Zoom
        {
            get => this.zoom;
            set
            {
                this.zoom = this.Zoomable ? value : 1.0f;
            }
        }

        public bool Zoomable { get; set; }
        public bool Scrollable { get; set; }




        private Vector2f lastPosition;

        public bool Moved { get => this.LastDownMoved != Vector2f.Zero; }
        public Vector2f LastDownMoved { get; private set; }

        public MouseState(CardState state)
        {
            this.State = state;
            this.Down = false;

            this.lastPosition = Vector2f.Zero;
            this.Scroll = Vector2f.Zero;
            this.Zoom = 1.0f;

            this.Zoomable = this.Scrollable = true;
        }

        public void ScrollToZoom(bool negate)
        {
            //Console.WriteLine(this.lastPosition.ToString());

            //if (negate)
            //    this.Scroll -= this.lastPosition * (1 / this.Zoom);
            //else
            //    this.Scroll += this.lastPosition * (1 / this.Zoom);
        }

        public void CheckDrop()
        {
            if (this.State.Highlighter == null) return;
            foreach (var selectedCard in this.State.Selected)
                if (selectedCard.GetType() == typeof(DropAreaCard) || selectedCard.HasDescendant(c => c.GetType() == typeof(DropAreaCard))) return;

            // TEST DROP
            var box = BoundingRect.FromPositionSize(this.State.Highlighter.GetGlobalPosition(), this.State.Highlighter.GetSize());
            var droppables = new List<Card>();
            foreach (var card in this.State.Storage.Cards) // exchange for cardstate.cards
                card.TraverseCard((c) => c.GetType() == typeof(DropAreaCard), droppables);

            Console.WriteLine($"Found {droppables.Count} droppables");

            var dropped = new List<Card>();
            foreach (var droppable in droppables)
            {
                dropped.Clear();
                droppable.TraverseTouches(box, dropped);

                if (dropped.Count > 0)
                    (droppable as DropAreaCard).FireDrop(this.State.Selected);
            }
        }

        public void CheckSelector()
        {
            if (this.State.Selector == null || !this.State.Selector.Visible) return;

            var selectorBox = BoundingRect.FromPositionSize(this.State.Selector.GetGlobalPosition(), this.State.Selector.GetSize());
            var traversed = new List<Card>();
            foreach (var card in this.State.Storage.Cards)
                card.TraverseOverlap(selectorBox, traversed);

            if (traversed.Count > 0)
            {
                //Console.WriteLine($"Selecting {traversed.Count} items...");

                foreach (var traversedCard in traversed)
                    this.State.Selected.Add(traversedCard);

                this.State.Highlighter = RectFactory.CreateHighlighter(traversed);
            }

            this.State.Selector.Visible = false;
            this.State.Selector.Component.InvokeChange();
        }

        public void OnDown(Vector2f position)
        {
            this.Down = true;
            this.LastDownMoved = Vector2f.Zero;

            this.lastPosition = position;
        }

        public void OnFakeMove(Vector2f position)
        {
            if (!this.Down) return;

            this.lastPosition = position;
        }

        public void OnMove(Vector2f position)
        {
            var dev = this.lastPosition - position;
            dev /= this.Zoom;

            this.lastPosition = position;

            if (!this.Down) return;

            this.LastDownMoved += dev;

            if (this.State.Selected.Count > 0)
            {
                bool moveHighlighter = true; // HIGHLIGHTER DRAGGING WORKAROUND, FIX LATER!!!!
                foreach (var card in this.State.Selected)
                {
                    card.FireMove(dev);
                    if (card.Draggable)
                        card.PositionBehavior.Position += dev;
                    else
                        moveHighlighter = false;
                }


                if (this.State.Highlighter != null && moveHighlighter)
                    this.State.Highlighter.PositionBehavior.Position += dev;

                this.State.InteropQueue.Flush(this.State.Canvas.JSRuntime);
            }




            if (this.State.Selector.Visible)
                this.State.Selector.SizeBehavior.Size += dev;

            if (!this.State.Selector.Visible && this.State.Selected.Count <= 0)
            {
                this.Scroll += dev;
                this.State.Canvas.Translate();
            }
        }

        public void OnUp(Vector2f position)
        {
            if (this.State.Selected.Count > 0)
            {
                foreach (var card in this.State.Selected)
                    card.Snap();

                this.State.InteropQueue.Flush(this.State.Canvas.JSRuntime);

                this.State.Highlighter = RectFactory.CreateHighlighter(this.State.Selected);

            }

            this.Down = false;
        }
    }
}
