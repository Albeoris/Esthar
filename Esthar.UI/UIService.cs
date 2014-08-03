using System;
using Esthar.Core;

namespace Esthar.UI
{
    public static class UiService
    {
        public static event Action TagsChanged;

        public static void InvokeTagsChanged()
        {
            TagsChanged.NullSafeInvoke();
        }
    }
}