using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Models
{
    public struct Vector2f
    {
        public static Vector2f Zero = new Vector2f(0, 0);

        public float X { get; set; }
        public float Y { get; set; }

        public Vector2f(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }

        public static Vector2f operator -(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X - b.X, a.Y - b.Y);
        }


        public static Vector2f operator +(Vector2f a, Vector2f b)
        {
            return new Vector2f(a.X + b.X, a.Y + b.Y);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector2f)) return false;

            var vec = (Vector2f)obj;
            return (this.X == vec.X && this.Y == vec.Y);
        }

        public void ToLocalFromClient(BoundingClientRect box)
        {
            this.X -= (float)box.Left;
            this.Y -= (float)box.Top;
        }

        public override string ToString()
        {
            return $"{this.X} - {this.Y}";
        }
    }
}
