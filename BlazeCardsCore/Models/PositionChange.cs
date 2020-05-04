using BlazeCardsCore.Extension;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Models
{
    public struct PositionChange
    {
        //public ElementReference Element { get; private set; }
        //public Vector2f Change { get; private set; }

        //public PositionChange(ElementReference element, Vector2f change)
        //{
        //    this.Element = element;
        //    this.Change = change;
        //}

        public string UniqueID { get; private set; }
        public Vector2f Change { get; private set; }

        public PositionChange(string uniqueID, Vector2f change)
        {
            this.UniqueID = uniqueID;
            this.Change = change;
        }

        public override string ToString()
        {
            return $"{this.UniqueID};{this.Change.X.ToJSStr()};{this.Change.Y.ToJSStr()}";
        }
    }
}
