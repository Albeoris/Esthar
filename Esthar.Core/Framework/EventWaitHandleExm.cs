using System.Threading;

namespace Esthar.Core
{
    public static class EventWaitHandleExm
    {
        public static void NullSafeSet(this EventWaitHandle handle)
        {
            if (handle != null)
                handle.Set();
        }
    }
}