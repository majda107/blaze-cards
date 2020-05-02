using BlazeCardsCore.Models;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public static void FireAndForget(Task task)
        {
            // Only care about tasks that may fault (not completed) or are faulted,
            // so fast-path for SuccessfullyCompleted and Canceled tasks.
            if (!task.IsCompleted || task.IsFaulted)
            {
                // use "_" (Discard operation) to remove the warning IDE0058: Because this call is not awaited, execution of the current method continues before the call is completed
                // https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
                _ = ForgetAwaited(task);
            }

        }
        // Allocate the async/await state machine only when needed for performance reason.
        // More info about the state machine: https://blogs.msdn.microsoft.com/seteplia/2017/11/30/dissecting-the-async-methods-in-c/
        static async Task ForgetAwaited(Task task)
        {
            try
            {
                // No need to resume on the original task scheduler, so use ConfigureAwait(false)
                await task.ConfigureAwait(false);
            }
            catch
            {
                // Nothing to do here
            }
        }


        public void Flush(IJSRuntime js)
        {
            //Console.WriteLine($"Flushing {this.Changes.Count} changes...");

            //new Thread(() =>
            //{
            //    js.InvokeVoidAsync("changeFlush", this.Changes);
            //}).Start();

            //Console.WriteLine("Movin!");
            FireAndForget(js.InvokeVoidAsync("changeFlush", this.Changes).AsTask());

            this.Changes.Clear();
        }
    }
}
