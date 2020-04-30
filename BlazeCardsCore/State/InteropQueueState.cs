using BlazeCardsCore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.State
{
    public class InteropQueueState
    {
        public CardState State { get; private set; }
        public List<PositionChange> Changes { get; private set; }

        public InteropQueueState(CardState state)
        {
            this.State = state;
            this.Changes = new List<PositionChange>();
        }

        public void QueueChange(PositionChange change)
        {
            this.Changes.Add(change);
        }

        public void Flush(IJSRuntime js)
        {
            //Console.WriteLine($"Flushing {this.Changes.Count} changes...");

            js.InvokeVoidAsync("changeFlush", this.Changes);
            this.Changes.Clear();
        }
    }
}
