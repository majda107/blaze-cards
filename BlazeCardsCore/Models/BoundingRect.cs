using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Models
{
    public class BoundingRect
    {
        private Vector2f _position;
        public Vector2f Position { get => this._position; set => this._position = value; }

        private Vector2f _size;
        public Vector2f Size { get => this._size; set => this._size = value; }


        public float Width { get => this.Size.X; }
        public float Height { get => this.Size.Y; }


        public float Left { get => this.Position.X; }
        public float Right { get => this.Position.X + this.Size.X; }
        public float Top { get => this.Position.Y; }
        public float Bottom { get => this.Position.Y + this.Size.Y; }


        public static BoundingRect FromPositionSize(Vector2f position, Vector2f size)
        {
            //position -= new Vector2f(Math.Min(size.X, 0), Math.Min(size.Y, 0));
            size = new Vector2f(Math.Abs(size.X), Math.Abs(size.Y));

            var box = new BoundingRect();
            box.Position = position;
            box.Size = size;

            return box;
        }

        public bool Overlap(BoundingRect box)
        {
            if (box.Left < this.Left &&
                box.Right > this.Right &&
                box.Bottom > this.Bottom &&
                box.Top < this.Top) return true;

            //Console.WriteLine($"Selector right - {box.Right}, Item left - {this.Left}");
            //if (box.Right > this.Left) return true;

            return false;
        }
    }
}
