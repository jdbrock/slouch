﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slouch.Core
{
    public static class DisposableExtensionMethods
    {
        public static void TryDispose(this IDisposable inDisposable)
        {
            if (inDisposable != null)
                try { inDisposable.Dispose(); }
                catch { }
        }
    }
}
