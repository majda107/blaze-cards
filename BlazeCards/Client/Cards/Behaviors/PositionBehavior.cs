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
        private Vector2f _correction = Vector2f.Zero;
        public Vector2f Correction
        {
            get => this._correction; 
            set
            {
                this.dirty = true;
                this._correction = value;
            }
        }

        public CardComponent Card;
        private Vector2f _position;
        private bool dirty;
        public Vector2f Position
        {
            get
            {
                // add this lol 
                //if (this.Correction == Vector2f.Zero) return this.position;
                return this._position + this.Correction;
            }
            set
            {
                this.dirty = true;
                this._position = value;
                this.Move(this.Position);
            }
        }

        private void Move(Vector2f position)
        {
            if (this.Card == null) return;
            //Console.WriteLine($"Translatin.. {position.X} | {position.Y}");
            this.Card.JSRuntime.InvokeVoidAsync("translateGraphics", this.Card.Graphics, position.X, position.Y);
        }

        public void Update()
        {
            if (!this.dirty) return;

            this.dirty = false;
            this.Move(this.Position);
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
