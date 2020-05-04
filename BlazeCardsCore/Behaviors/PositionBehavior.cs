using BlazeCardsCore.Components;
using BlazeCardsCore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Behaviors
{
    public class PositionBehavior
    {
        private Vector2f _correction = Vector2f.Zero;
        public Vector2f Correction
        {
            get => this._correction;
            set
            {
                this._correction = value;
            }
        }

        public CardComponent Card;
        private Vector2f _position;
        
        public Vector2f Position
        {
            get
            {
                return this._position + this.Correction;
            }
            set
            {
                this._position = value;

                if (this.Card == null) return;
                this.Card.Canvas.State.InteropQueue.QueueChange(new PositionChange(this.Card.GetUniquieID(), this.Position));
            }
        }

        public void Update(bool checkDirty = false)
        {
            if (this.Card == null) return;

            //Console.WriteLine("Flushing from position behavior!");
            this.Card.Canvas.State.InteropQueue.QueueChange(new PositionChange(this.Card.GetUniquieID(), this.Position));
            this.Card.Canvas.State.InteropQueue.Flush(this.Card.Canvas.JSRuntime);


            //this.Move(this.Position);
        }


        public void AssignComponent(CardComponent component)
        {
            this.Card = component;
        }
    }
}
