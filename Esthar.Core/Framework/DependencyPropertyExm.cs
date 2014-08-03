using System;
using System.ComponentModel;
using System.Windows;
using Esthar.Core;

namespace Esthar
{
    public static class DependencyPropertyExm
    {
        public static void Subscribe(this DependencyProperty self, object obj, EventHandler callback)
        {
            Exceptions.CheckArgumentNull(self, "self");
            Exceptions.CheckArgumentNull(obj, "obj");
            Exceptions.CheckArgumentNull(callback, "callback");

            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(self, obj.GetType());
            dpd.AddValueChanged(obj, callback);
        }
    }
}