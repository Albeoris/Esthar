using System;
using System.Windows.Input;
using Esthar.Core;
using Esthar.UI;

namespace Esthar
{
    public sealed class SelectFontForUserTagCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            UserTagView tag = Exceptions.CheckArgumentNull((UserTagView)parameter, "parameter");

            FontChooser chooser = new FontChooser { FontDescriptor = (FontDescriptor)tag.Font };
            if (chooser.ShowDialog() == true)
                tag.Font = (FontDescriptorView)chooser.FontDescriptor;
        }
    }

    public sealed class RemoveFontForUserTagCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            UserTagView tag = Exceptions.CheckArgumentNull((UserTagView)parameter, "parameter");
            tag.Font = (FontDescriptorView)FontDescriptor.Default;
        }
    }
}