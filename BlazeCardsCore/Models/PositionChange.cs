using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.Models
{
    public struct PositionChange
    {
        public ElementReference Element { get; private set; }
        public Vector2f Change { get; private set; }

        public PositionChange(ElementReference element, Vector2f change)
        {
            this.Element = element;
            this.Change = change;
        }
    }
}
