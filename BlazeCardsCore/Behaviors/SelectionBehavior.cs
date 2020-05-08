using BlazeCardsCore.Descriptors;
using BlazeCardsCore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazeCardsCore.Behaviors
{
    public class SelectionBehavior
    {
        public TextCard Card { get; private set; }

        public float LastCalculatedSnap { get; private set; }
        public SelectionBehavior(TextCard card)
        {
            this.Card = card;
        }

        public async Task<float> SnapLetter(float inputX, IJSRuntime JSRuntime)
        {
            var tb = this.Card.TextBehavior;

            float lastWidth = 0;
            for (int i = 1; i <= tb.Value.Length; i++)
            {
                var selectorStr = tb.Value.Substring(0, i);
                var actualWidth = (float)(await JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", selectorStr)).Width;
                if (lastWidth + ((actualWidth - lastWidth) / 2) > inputX)
                {
                    this.LastCalculatedSnap = lastWidth;
                    break;
                }

                if (i == tb.Value.Length)
                {
                    this.LastCalculatedSnap = actualWidth;
                    break;
                }

                lastWidth = actualWidth;
            }

            return this.LastCalculatedSnap;
        }
    }
}
