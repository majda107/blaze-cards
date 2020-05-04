using Mono.WebAssembly.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeCardsCore.State
{
    class MonoInteropState
    {
        public MonoWebAssemblyJSRuntime JSRuntime { get; private set; }

        public MonoInteropState()
        {
            this.JSRuntime = new MonoWebAssemblyJSRuntime();
        }

        public void TestInvoke<TRes>()
        { 
            var res = this.JSRuntime.InvokeUnmarshalled<TRes>("alert('nigga');");
        }
    }
}
