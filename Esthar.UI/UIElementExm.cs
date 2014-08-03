using System;
using System.Windows;
using System.Windows.Threading;

namespace Esthar.UI
{
    public static class UIElementExm
    {
        private static readonly Action EmptyDelegate = delegate { };

        public static void Refresh(this UIElement element)
        {
            element.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }
}