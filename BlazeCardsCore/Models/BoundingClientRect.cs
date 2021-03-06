﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazeCardsCore.Models
{
    public struct BoundingClientRect
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }
        public double Left { get; set; }

        public double CenterX { get => this.Width / 2; }
        public double CenterY { get => this.Height / 2; }
        public Vector2f Center { get => new Vector2f((float)this.CenterX, (float)this.CenterY); }

        public Vector2f Size { get => new Vector2f((float)this.Width, (float)this.Height); }

        public Vector2f ToCenterCoords(Vector2f vector)
        {
            return this.Center - vector;
        }
    }
}
