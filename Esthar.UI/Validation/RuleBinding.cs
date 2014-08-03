using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using Esthar.UI.MarkupExtensions;

namespace Esthar.UI
{
    public class RuleBinding : BindingDecoratorBase
    {
        [ConstructorArgument("rule")]
        public CustomRule Rule { get; set; }

        public RuleBinding(string path)
            : base(path)
        {
            ValidatesOnDataErrors = true;
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
        }
        public RuleBinding(string path, CustomRule rule)
            :this(path)
        {
            Rule = rule;
        }

        /// <summary>
        /// This method is being invoked during initialization.
        /// </summary>
        /// <param name="provider">Provides access to the bound items.</param>
        /// <returns>The binding expression that is created by
        /// the base class.</returns>
        public override object ProvideValue(IServiceProvider provider)
        {
            
            ValidationRule rule;
            switch (Rule)
            {
                case CustomRule.DirectoryMustExists:
                    rule = new DirectoryMustExistsRule(false);
                    break;
                case CustomRule.EmptyOrDirectoryMustExists:
                    rule = new DirectoryMustExistsRule(true);
                    break;
                default:
                    rule = new ExceptionValidationRule();
                    break;
            }

            Binding.ValidationRules.Add(rule);
            return base.ProvideValue(provider);
        }
    }
}