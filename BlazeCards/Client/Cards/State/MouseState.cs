using BlazeCards.Client.Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.State
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

            //Console.WriteLine("trying to move");

            //Console.WriteLine($"Mouse move: {position.X}");
            var dev = position - this.lastPosition;

            if (this.CardState.Selected != null)
                this.CardState.Selected.Descriptor.PositionBehavior.Position += dev;

            this.lastPosition = position;
        }

        public void OnUp(Vector2f position)
        {
            this.CardState.Selected?.Descriptor?.Snap();

            this.Down = false;
        }
    }
}
