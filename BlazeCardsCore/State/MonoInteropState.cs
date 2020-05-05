using BlazeCardsCore.Extension;
using BlazeCardsCore.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Mono.WebAssembly.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace BlazeCardsCore.State
{
    public class MonoInteropState
    {
        public static readonly MonoWebAssemblyJSRuntime MonoRuntime = new MonoWebAssemblyJSRuntime();

        public static void InvokeChangeQueue(PositionChange[] changes)
        {
            if (changes.Length <= 0) return;

            var packet = String.Empty;
            for (int i = 0; i < changes.Length; i++)
            {
                if (changes[i].UniqueID == String.Empty) continue;

                packet += changes[i].ToString();
                if (i < changes.Length - 1)
                    packet += "|";
            }

            //Console.WriteLine($"Sending packet: {packet}");
            MonoRuntime.InvokeUnmarshalled<string, object>("changeFlushPacket", packet);
        }

        public static void InvokeScale(string elementID, float scale, float centerX, float centerY)
        {
            var packet = $"{elementID};{scale.ToJSStr()};{centerX.ToJSStr()};{centerY.ToJSStr()}";
            //MonoRuntime.InvokeUnmarshalled<string, object>("scaleGraphics", packet);

            MonoRuntime.InvokeUnmarshalled<string, object>("scaleGraphics", packet);
        }
    }
}
