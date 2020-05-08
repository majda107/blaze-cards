using BlazeCardsCore.Components;
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


        public RectCard CaretDescriptor { get; private set; }
        public RectCard SelectorDescriptor { get; private set; }


        public float LastCalculatedSnap { get; private set; }
        public bool Selecting { get; set; }


        public int BaseOffset { get; set; }
        public int ExtentOffset { get; set; }

        public int CorrectedBase { get => this.BaseOffset > this.ExtentOffset ? this.ExtentOffset : this.BaseOffset; }
        public int CorrectedExtent { get => this.ExtentOffset > this.BaseOffset? this.ExtentOffset : this.BaseOffset; }


        public SelectionBehavior(TextCard card)
        {
            this.Card = card;
        }


        public async Task EndCaret(CanvasComponent canvas)
        {
            var str = this.Card.TextBehavior.Value;
            float caretOffset = (float)(await canvas.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", str)).Width;
            this.CaretDescriptor.PositionBehavior.Position = this.Card.TextBehavior.Padding + new Vector2f(caretOffset, 1);
            this.BaseOffset = this.ExtentOffset = str.Length - 1;

            canvas.State.InteropQueue.Flush(canvas.JSRuntime);
        }
        public async Task Init(CanvasComponent canvas)
        {
            this.CaretDescriptor = new RectCard();
            this.CaretDescriptor.Highlightable = this.CaretDescriptor.Draggable = false;
            this.CaretDescriptor.Classes.Add("blaze-text-caret");
            this.CaretDescriptor.SizeBehavior.Size = new Vector2f(2, 24);

            await this.EndCaret(canvas);

            this.SelectorDescriptor = new RectCard();
            this.SelectorDescriptor.Highlightable = this.SelectorDescriptor.Draggable = false;
            this.SelectorDescriptor.Classes.Add("blaze-text-selector");
            this.SelectorDescriptor.SizeBehavior.Size = new Vector2f(0, 24);
            this.SelectorDescriptor.SizeBehavior.HookNegativeSize(this.SelectorDescriptor.PositionBehavior);
            this.SelectorDescriptor.PositionBehavior.Position = this.Card.TextBehavior.Padding + new Vector2f(0, 1); // lol dat 0 : 1 boi

            canvas.State.InteropQueue.Flush(canvas.JSRuntime);
        }

        public async Task OnDown(Vector2f pos, CanvasComponent canvas)
        {
            Console.WriteLine("SNAPPING LETTER!");
            float offsetX = await this.Card.SelectionBehavior.SnapLetter(pos.X, canvas.JSRuntime, true);
            Console.WriteLine(this.BaseOffset);

            var paddingX = this.Card.TextBehavior.Padding.X;
            this.CaretDescriptor.PositionBehavior.Position = new Vector2f(offsetX + paddingX, this.CaretDescriptor.PositionBehavior.Position.Y);

            this.SelectorDescriptor.PositionBehavior.Correction = Vector2f.Zero;
            this.SelectorDescriptor.PositionBehavior.Position = new Vector2f(offsetX + paddingX, this.SelectorDescriptor.PositionBehavior.Position.Y);
            this.SelectorDescriptor.SizeBehavior.Size = new Vector2f(pos.X - offsetX, 24);

            canvas.State.InteropQueue.Flush(canvas.JSRuntime);
        }

        public async Task OnUp(CanvasComponent canvas)
        {
            float last = this.Card.SelectionBehavior.LastCalculatedSnap;
            float offsetX = await this.Card.SelectionBehavior.SnapLetter(last + this.SelectorDescriptor.SizeBehavior.Size.X, canvas.JSRuntime);

            var paddingX = this.Card.TextBehavior.Padding.X;
            this.CaretDescriptor.PositionBehavior.Position = new Vector2f(offsetX + paddingX, this.CaretDescriptor.PositionBehavior.Position.Y);
            this.SelectorDescriptor.SizeBehavior.Size = new Vector2f(offsetX - last, 24);

            canvas.State.InteropQueue.Flush(canvas.JSRuntime);
        }



        public void MoveLeft()
        {
            this.BaseOffset -= 1;
            this.ExtentOffset = this.BaseOffset;
        }
        
        public void MoveRight()
        {
            this.ExtentOffset += 1;
            this.BaseOffset = this.ExtentOffset;
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
                    this.SetOffset(i, setBase);

                    break;
                }

                lastWidth = actualWidth;
            }

            return this.LastCalculatedSnap;
        }

        public async Task ExtentCaret(CanvasComponent canvas)
        {
            var str = this.Card.TextBehavior.Value.Substring(0, this.ExtentOffset);
            var offsetX = (float)(await canvas.JSRuntime.InvokeAsync<BoundingClientRect>("calculateTextRect", str)).Width;

            var paddingX = this.Card.TextBehavior.Padding.X;
            this.CaretDescriptor.PositionBehavior.Position = new Vector2f(offsetX + paddingX, this.CaretDescriptor.PositionBehavior.Position.Y);
            this.SelectorDescriptor.SizeBehavior.Size = new Vector2f(0, 24);

            canvas.State.InteropQueue.Flush(canvas.JSRuntime);
        }
    }
}
