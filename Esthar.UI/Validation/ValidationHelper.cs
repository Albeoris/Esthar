using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Esthar.UI
{
    public static class ValidationHelper
    {
        private static readonly DropShadowEffect InvalidEffect = new DropShadowEffect { Color = Colors.Red, BlurRadius = 5, ShadowDepth = 1 };
        
        public static void SetInvalid(FrameworkElement element, ValidationResult result)
        {
            if (result.IsValid)
            {
                element.Effect = null;
                element.ToolTip = null;
            }
            else
            {
                element.Effect = InvalidEffect;
                element.ToolTip = new ToolTip { Content = result.ErrorContent };
            }
        }
    }
}