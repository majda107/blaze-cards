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

        private Vector2f size;
        public Vector2f Size
        {
            get => this.size;
            set
            {
                this.size = value;
                this.Card.InvokeChange();
            }
        }

        public float Width { get => this.size.X; }
        public float Height { get => this.size.Y; }

        public SizeBehavior(CardComponent card, float width = 0, float height = 0)
        {
            this.Card = card;
            this.size = new Vector2f(width, height);
        }
    }
}
