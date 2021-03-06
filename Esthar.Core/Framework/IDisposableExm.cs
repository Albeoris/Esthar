﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Esthar.Core
{
    public static class IDisposableExm
    {
        [TargetedPatchingOptOut("Performance critical to inline across NGen image boundaries")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void NullSafeDispose(this IDisposable self)
        {
            if (!ReferenceEquals(self, null))
                self.Dispose();
        }

        public static void SafeDispose(this IDisposable self)
        {
            try
            {
                if (!ReferenceEquals(self, null))
                    self.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Debug.Fail(ex.Message, ex.ToString());
            }
        }

        public static void NullSafeDispose(this IEnumerable<IDisposable> self)
        {
            if (ReferenceEquals(self, null))
                return;

            foreach (IDisposable item in self)
                NullSafeDispose(item);
        }

        public static void SafeDispose(this IEnumerable<IDisposable> self)
        {
            if (ReferenceEquals(self, null))
                return;

            foreach (IDisposable item in self)
                SafeDispose(item);
        }
    }
}