using BlazeCards.Client.Cards.Components;
using BlazeCards.Client.Cards.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Behaviors
{
    public class PositionBehavior
    {
        public CardComponent Card;

        private Vector2f position;
        private bool dirty;
        public Vector2f Position
        {
            get => this.position;
            set
            {
                this.dirty = true;
                this.position = value;
                this.Move(this.position);
            }
        }

        private void Move(Vector2f position)
        {
            //Console.WriteLine($"Translatin.. {position.X} | {position.Y}");
            this.Card.JSRuntime.InvokeVoidAsync("translateGraphics", this.Card.Graphics, position.X, position.Y);
        }

        public void Update()
        {
            if (!this.dirty) return;

            this.dirty = false;
            this.Move(this.position);
        }

        public PositionBehavior()
        {
            this.dirty = true;
        }

        public void AssignComponent(CardComponent component)
        {
            this.Card = component;
        }
    }
}
