using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using Esthar.Core;

namespace Esthar.UI
{
    public sealed class FileMustExistsRule : ValidationRule
    {
        private readonly bool _isOptional;

        public FileMustExistsRule(bool isOptional)
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

                    return new ValidationResult(false, Lang.ErrorFileMustBeSpecified);
                }

                if (!File.Exists(path))
                    return new ValidationResult(false, String.Format(Lang.ErrorFileNotFoundFormat, path));

                return ValidationResult.ValidResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return new ValidationResult(false, ex);
            }
        }
    }
}