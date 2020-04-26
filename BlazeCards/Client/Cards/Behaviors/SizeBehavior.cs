using BlazeCards.Client.Cards.Components;
using BlazeCards.Client.Cards.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCards.Client.Cards.Behaviors
{
    public class SizeBehavior
    {
        public CardComponent Card;

        private Vector2f _size;
        public Vector2f Size
        {
            get => this._size;
            set
            {
                this._size = value;

                if (this.Card == null) return;
                this.Card.InvokeChange();
            }
        }

        public float Width { get => this._size.X; }
        public float Height { get => this._size.Y; }

        public SizeBehavior(float width = 0, float height = 0)
        {
            this._size = new Vector2f(width, height);
        }

        public void AssignComponent(CardComponent card)
        {
            this.Card = card;
        }
    }
}
