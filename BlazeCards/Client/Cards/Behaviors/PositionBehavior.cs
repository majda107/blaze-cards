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
        public Vector2f Position {
            get => this.position; 
            set
            {
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
            this.Move(this.position);
        }

        public PositionBehavior(CardComponent card)
        {
            this.Card = card;
        }
    }
}
