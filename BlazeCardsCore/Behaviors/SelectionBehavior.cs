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
        public bool Selecting { get; set; }

        private float _baseOffset;
        public float BaseOffset
        {
            get => this._baseOffset;
            set
            {
                if (value > this.ExtentOffset)
                {
                    this._baseOffset = this.ExtentOffset;
                    this.ExtentOffset = value;
                }
                else
                    this._baseOffset = value;
            }
        }

        private float _extentOffset;
        public float ExtentOffset
        {
            get => this._extentOffset;
            set
            {
                if (value < this.BaseOffset)
                {
                    this._extentOffset = this.BaseOffset;
                    this.BaseOffset = value;
                }
                else
                    this._extentOffset = value;
            }
        }

        public SelectionBehavior(TextCard card)
        {
            this.Card = card;
        }

        private void SetOffset(int index, bool setBase)
        {
            if (setBase)
                this.BaseOffset = index;
            else
                this.ExtentOffset = index;
        }
        public async Task<float> SnapLetter(float inputX, IJSRuntime JSRuntime, bool setBase = false)
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
                    this.SetOffset(i - 1, setBase);

                    break;
                }

                if (i == tb.Value.Length)
                {
                    this.LastCalculatedSnap = actualWidth;
                    this.SetOffset(i - 1, setBase);

                    break;
                }

                lastWidth = actualWidth;
            }

            return this.LastCalculatedSnap;
        }
    }
}
