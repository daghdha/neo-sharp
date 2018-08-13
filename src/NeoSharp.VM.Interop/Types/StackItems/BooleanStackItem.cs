﻿using System;
using NeoSharp.VM.Interop.Extensions;
using NeoSharp.VM.Interop.Native;
using Newtonsoft.Json;

namespace NeoSharp.VM.Interop.Types.StackItems
{
    public class BooleanStackItem : IBooleanStackItem, INativeStackItem
    {
        static readonly byte[] FALSE_0 = { 0 };

        /// <summary>
        /// Native Handle
        /// </summary>
        IntPtr _handle;
        /// <summary>
        /// Native Handle
        /// </summary>
        [JsonIgnore]
        public IntPtr Handle => _handle;
        /// <summary>
        /// Is Disposed
        /// </summary>
        [JsonIgnore]
        public override bool IsDisposed => _handle == IntPtr.Zero;
        /// <summary>
        /// Type
        /// </summary>
        public new EStackItemType Type => base.Type;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="data">Data</param>
        internal BooleanStackItem(ExecutionEngine engine, bool data) : base(engine, data)
        {
            _handle = this.CreateNativeItem();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="engine">Engine</param>
        /// <param name="handle">Handle</param>
        /// <param name="value">Raw value</param>
        internal BooleanStackItem(ExecutionEngine engine, IntPtr handle, byte[] value) : base(engine, value[0] == NeoVM.TRUE)
        {
            _handle = handle;
        }

        public byte[] GetNativeByteArray()
        {
            return Value ? TRUE : FALSE_0;
        }

        #region IDisposable Support

        protected override void Dispose(bool disposing)
        {
            lock (this)
            {
                if (_handle == IntPtr.Zero) return;

                NeoVM.StackItem_Free(ref _handle);
            }
        }

        #endregion
    }
}