using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Esthar.Core;
using Esthar.Data;

namespace Esthar.UI
{
    /// <summary>
    /// Логика взаимодействия для OptionsPathsControl.xaml
    /// </summary>
    public partial class OptionsPathsWindow
    {
        public OptionsPathsWindow()
        {
            InitializeComponent();
            Subscribe();

            LoadOptions();
        }

        private void Subscribe()
        {
            ContinueButton.Click += OnContinueButtonClick;
            SelectDirectoryControl.PathProperty.Subscribe(GameDirectoryControl, (s, e) => ValidateGameDirectory());
            SelectDirectoryControl.PathProperty.Subscribe(WorkingDirectoryControl, (s, e) => ValidateWorkingDirectory());
            SelectDirectoryControl.PathProperty.Subscribe(CVSDirectoryControl, (s, e) => ValidateCVSDirectory());
        }

        private void LoadOptions()
        {
            try
            {
                GameDirectoryControl.Path = Options.GameDirectory;
                WorkingDirectoryControl.Path = Options.WorkingDirectory;
                CVSDirectoryControl.Path = Options.CVSDirectory;
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        private void SaveOptions()
        {
            try
            {
                Options.GameDirectory = GameDirectoryControl.Path;
                Options.WorkingDirectory = WorkingDirectoryControl.Path;
                Options.CVSDirectory = CVSDirectoryControl.Path;
                Options.Save();
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        public bool Check()
        {
            if (!DirectoryMustExistsRule.Validate(GameDirectoryControl, GameDirectoryControl.Path, false).IsValid)
                return false;
            if (!DirectoryMustExistsRule.Validate(WorkingDirectoryControl, WorkingDirectoryControl.Path, false).IsValid)
                return false;
            if (!DirectoryMustExistsRule.Validate(CVSDirectoryControl, CVSDirectoryControl.Path, true).IsValid)
                return false;

            return true;
        }

        private void OnContinueButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Check())
                {
                    SaveOptions();
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                UIHelper.ShowError(ex);
            }
        }

        #region Validation

        private ValidationResult ValidateGameDirectory()
        {
            try
            {
                string gameDirectoryPath = GameDirectoryControl.Path;

                ValidationResult result = DirectoryMustExistsRule.Validate(GameDirectoryControl, gameDirectoryPath, false);
                if (!result.IsValid)
                    return result;

                string[] executables = Directory.GetFiles(gameDirectoryPath, Options.GameExecutableMask);
                if (executables.All(file=>!Regex.IsMatch(file, Options.GameExecutablesExpression)))
                {
                    result = new ValidationResult(false, String.Format(Lang.ErrorFileNotFoundFormat, Options.GameExecutableMask));
                    ValidationHelper.SetInvalid(GameDirectoryControl, result);
                    return result;
                }

                string dataDirectoryPath = Path.Combine(gameDirectoryPath, Options.GameDataDirectoryName);

                result = DirectoryMustExistsRule.Validate(GameDirectoryControl, dataDirectoryPath, false);
                if (!result.IsValid)
                    return result;

                FileMustExistsRule fileMustExistsRule = new FileMustExistsRule(false);

                foreach (string archiveName in Options.GameArchivesNames)
                {
                    string archivePath = Path.Combine(dataDirectoryPath, archiveName);

                    result = fileMustExistsRule.Validate(Path.ChangeExtension(archivePath, ".fs"), CultureInfo.CurrentCulture);
                    if (!result.IsValid) break;

                    result = fileMustExistsRule.Validate(Path.ChangeExtension(archivePath, ".fi"), CultureInfo.CurrentCulture);
                    if (!result.IsValid) break;

                    result = fileMustExistsRule.Validate(Path.ChangeExtension(archivePath, ".fl"), CultureInfo.CurrentCulture);
                    if (!result.IsValid) break;
                }

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ValidationResult result = new ValidationResult(false, ex.GetBaseException().Message);
                ValidationHelper.SetInvalid(GameDirectoryControl, result);
                return result;
            }
        }

        private ValidationResult ValidateWorkingDirectory()
        {
            return DirectoryMustExistsRule.Validate(WorkingDirectoryControl, WorkingDirectoryControl.Path, false);
        }

        private ValidationResult ValidateCVSDirectory()
        {
            return DirectoryMustExistsRule.Validate(CVSDirectoryControl, CVSDirectoryControl.Path, true);
        }

        #endregion
    }
}