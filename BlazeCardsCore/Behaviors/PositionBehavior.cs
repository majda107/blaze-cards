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
                //this.Move(this.Position);

                if (this.Card == null) return;
                this.Card.Canvas.State.InteropQueue.QueueChange(new PositionChange(this.Card.Graphics, this.Position));
            }
        }

        public void Update()
        {
            //if (!this.dirty) return;

            this.dirty = false;

            Console.WriteLine("Updatin...");
            if (this.Card == null) return;

            //Console.WriteLine("Flushing from position behavior!");
            this.Card.Canvas.State.InteropQueue.QueueChange(new PositionChange(this.Card.Graphics, this.Position));
            this.Card.Canvas.State.InteropQueue.Flush(this.Card.Canvas.JSRuntime);


            //this.Move(this.Position);
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
