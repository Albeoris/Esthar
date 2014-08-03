using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Esthar.Core;

namespace Esthar.UI
{
    public sealed class DirectoryMustExistsRule : ValidationRule
    {
        private readonly bool _isOptional;

        public DirectoryMustExistsRule(bool isOptional)
        {
            _isOptional = isOptional;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                string path = Convert.ToString(value, cultureInfo);
                if (string.IsNullOrEmpty(path))
                {
                    if (_isOptional)
                        return ValidationResult.ValidResult;

                    return new ValidationResult(false, Lang.ErrorDirectoryMustBeSpecified);
                }

                if (!Directory.Exists(path))
                    return new ValidationResult(false, String.Format(Lang.ErrorDirectoryNotFoundFormat, path));

                return ValidationResult.ValidResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ValidationResult(false, ex.GetBaseException().Message);
            }
        }

        public static ValidationResult Validate(FrameworkElement element, string path, bool isOptional)
        {
            DirectoryMustExistsRule rule = new DirectoryMustExistsRule(isOptional);
            ValidationResult result = rule.Validate(path, CultureInfo.CurrentCulture);
            ValidationHelper.SetInvalid(element, result);
            return result;
        }
    }
}