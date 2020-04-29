using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Behaviors
{
    public class SizeBehavior
    {
        public CardComponent Card;
        public PositionBehavior PositionBehavior { get; private set; }

        private Vector2f _size;
        public Vector2f Size
        {
            get => this._size;
            set
            {
                this._size = value;

                if (this.Card == null) return;
                this.Card.InvokeChange();

                this.CorrectNegativeSize();
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

        public void HookNegativeSize(PositionBehavior position)
        {
            this.PositionBehavior = position;
        }
        private void CorrectNegativeSize()
        {
            if (this.PositionBehavior == null) return;
            if (this.Size.X >= 0 && this.Size.Y >= 0) return;

            this.PositionBehavior.Correction = new Vector2f(Math.Min(this.Size.X, 0), Math.Min(this.Size.Y, 0));

            //this.PositionBehavior.Position += new Vector2f(this.Size.X < 0 ? this.Size.X : 0, this.Size.Y < 0 ? this.Size.Y : 0);
            //this._size = new Vector2f(Math.Abs(this.Size.X), Math.Abs(this.Size.Y));
        }
    }
}
