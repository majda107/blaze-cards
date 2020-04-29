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

        private Vector2f lastPosition;

        public MouseState(CardState state)
        {
            this.CardState = state;
            this.Down = false;

            this.lastPosition = Vector2f.Zero;
        }

        public void OnDown(Vector2f position)
        {
            this.Down = true;
            //Console.WriteLine($"Mouse down: {position.X}");

            this.lastPosition = position;

            //if (this.CardState.Selected != null)
            //    this.CardState.Selected.Position.Position = position;
        }

        public void OnMove(Vector2f position)
        {
            if (!this.Down) return;

            //Console.WriteLine($"Mouse move: {position.X}");
            var dev = position - this.lastPosition;

            if (this.CardState.Selected.Count > 0)
            {
                //this.CardState.Selected.PositionBehavior.Position += dev;
                //if (this.CardState.Highlighter != null)
                //    this.CardState.Highlighter.PositionBehavior.Position = this.CardState.Selected.GetGlobalPosition();

                var minPos = new Vector2f(float.MaxValue, float.MaxValue);
                foreach (var card in this.CardState.Selected)
                {
                    card.PositionBehavior.Position += dev;

                    var pos = card.GetGlobalPosition();
                    if (pos.X < minPos.X) minPos.X = pos.X;
                    if (pos.Y < minPos.Y) minPos.Y = pos.Y;
                }

                if (this.CardState.Highlighter != null)
                    this.CardState.Highlighter.PositionBehavior.Position = minPos;
            }




            if (this.CardState.Selector != null)
            {
                this.CardState.Selector.SizeBehavior.Size += dev;
                //if (dev.X < 0 || dev.Y < 0)
                //    this.CardState.Selector.PositionBehavior.Position += new Vector2f(Math.Min(dev.X, 0), Math.Min(dev.Y, 0));
            }



            this.lastPosition = position;
        }

        public void OnUp(Vector2f position)
        {
            if (this.CardState.Selected.Count > 0)
            {
                var minPos = new Vector2f(float.MaxValue, float.MaxValue);
                foreach (var card in this.CardState.Selected)
                {
                    card.Snap();

                    var pos = card.GetGlobalPosition();
                    if (pos.X < minPos.X) minPos.X = pos.X;
                    if (pos.Y < minPos.Y) minPos.Y = pos.Y;
                }


                //this.CardState.Highlighter.PositionBehavior.Position = minPos;
                this.CardState.Highlighter = RectFactory.CreateHighlighter(this.CardState.Selected);
            }


            this.Down = false;
        }
    }
}
