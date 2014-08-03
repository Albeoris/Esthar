using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Esthar.Core
{
    public sealed partial class Lang
    {
        #region Lazy

        private static readonly Lazy<Lang> Instance = new Lazy<Lang>(Initialize, true);

        private static Lang Initialize()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo("Languages");
                if (!dir.Exists)
                    dir.Create();

                FileInfo defaultLang = null;
                FileInfo currentLang = null;

                FileInfo[] files = dir.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                {
                    if (file.Name.StartsWith(CultureInfo.CurrentCulture.Name, StringComparison.InvariantCultureIgnoreCase))
                        currentLang = file;
                    else if (file.Name.StartsWith("ru-RU", StringComparison.InvariantCultureIgnoreCase))
                        defaultLang = file;
                }

                if (defaultLang != null)
                    defaultLang.Delete();

                string pathCombine = Path.Combine(dir.FullName, "ru-RU.xml");

                using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream("Esthar.Core.Languages.ru-RU.xml"))
                using (Stream output = File.Create(pathCombine))
                    input.CopyTo(output);

                defaultLang = new FileInfo(pathCombine);

                if (currentLang == null)
                    currentLang = defaultLang;

                XmlElement cur = XmlHelper.LoadDocument(currentLang.FullName);
                XmlElement def = currentLang == defaultLang ? cur : XmlHelper.LoadDocument(defaultLang.FullName);

                return new Lang(def, cur);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to initialize Lang Data.");
                Environment.Exit(1);
                return null;
            }
        }

        #endregion

        #region Instance

        private readonly XmlElement _default;
        private readonly XmlElement _current;

        private Lang(XmlElement def, XmlElement cur)
        {
            _default = Exceptions.CheckArgumentNull(def, "def");
            _current = Exceptions.CheckArgumentNull(cur, "cur");
        }

        public string GetString(string arg, params string[] path)
        {
            Exceptions.CheckArgumentNullOrEmprty(arg, "arg");

            try
            {
                string str = FindString(_current, arg, path);
                if (str != null)
                    return str;
                Log.Warning("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }

            try
            {
                string str = FindString(_default, arg, path);
                if (str != null)
                    return str;
                Log.Error("String is not found in current dictionary: {0}.{1}", string.Join(".", path), arg);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "String is not found in default dictionary: {0}.{1}", string.Join(".", path), arg);
            }

            return "#StringNotFound#";
        }

        private string FindString(XmlElement root, string arg, params string[] path)
        {
            XmlElement result = root;
            foreach (string s in path)
            {
                result = result[s];
                if (result == null)
                    break;
            }

            return result == null ? null : result.FindString(arg);
        }

        #endregion
    }

    public sealed partial class Lang
    {
        private static readonly string MeasurementsElapsedInternal = GetMeasurements("Elapsed");
        private static readonly string MeasurementsRemainingInternal = GetMeasurements("Remaining");

        private static readonly string MeasurementsSecondAbbrInternal = GetMeasurements("SecondAbbr");

        private static readonly string MeasurementsByteAbbrInternal = GetMeasurements("ByteAbbr");
        private static readonly string MeasurementsKByteAbbrInternal = GetMeasurements("KByteAbbr");
        private static readonly string MeasurementsMByteAbbrInternal = GetMeasurements("MByteAbbr");
        private static readonly string MeasurementsGByteAbbrInternal = GetMeasurements("GByteAbbr");
        private static readonly string MeasurementsTByteAbbrInternal = GetMeasurements("TByteAbbr");
        private static readonly string MeasurementsPByteAbbrInternal = GetMeasurements("PByteAbbr");
        private static readonly string MeasurementsEByteAbbrInternal = GetMeasurements("EByteAbbr");

        private static string GetMeasurements(string name)
        {
            return Instance.Value.GetString(name, "Measurements");
        }

        public static string MeasurementsElapsed
        {
            get { return MeasurementsElapsedInternal; }
        }

        public static string MeasurementsRemaining
        {
            get { return MeasurementsRemainingInternal; }
        }

        public static string MeasurementsSecondAbbr
        {
            get { return MeasurementsSecondAbbrInternal; }
        }

        public static string MeasurementsByteAbbr
        {
            get { return MeasurementsByteAbbrInternal; }
        }

        public static string MeasurementsKByteAbbr
        {
            get { return MeasurementsKByteAbbrInternal; }
        }

        public static string MeasurementsMByteAbbr
        {
            get { return MeasurementsMByteAbbrInternal; }
        }

        public static string MeasurementsGByteAbbr
        {
            get { return MeasurementsGByteAbbrInternal; }
        }

        public static string MeasurementsTByteAbbr
        {
            get { return MeasurementsTByteAbbrInternal; }
        }

        public static string MeasurementsPByteAbbr
        {
            get { return MeasurementsPByteAbbrInternal; }
        }

        public static string MeasurementsEByteAbbr
        {
            get { return MeasurementsEByteAbbrInternal; }
        }
    }

    public sealed partial class Lang
    {
        private static string GetError(string name)
        {
            return Instance.Value.GetString(name, "Errors");
        }

        public static string ErrorFileMustBeSpecified
        {
            get { return GetError("FileMustBeSpecified"); }
        }

        public static string ErrorDirectoryMustBeSpecified
        {
            get { return GetError("DirectoryMustBeSpecified"); }
        }

        public static string ErrorFileNotFoundFormat
        {
            get { return GetError("FileMustNotFoundFormat"); }
        }

        public static string ErrorDirectoryNotFoundFormat
        {
            get { return GetError("DirectoryNotFoundFormat"); }
        }
    }

    public sealed partial class Lang
    {
        private static string GetButtons(string name)
        {
            return Instance.Value.GetString(name, "Buttons");
        }

        public static string ButtonsOK
        {
            get { return GetButtons("OK"); }
        }

        public static string ButtonsCancel
        {
            get { return GetButtons("Cancel"); }
        }

        public static string ButtonsContinue
        {
            get { return GetButtons("Continue"); }
        }
    }

    public sealed partial class Lang
    {
        private static string GetWizard(string name)
        {
            return Instance.Value.GetString(name, "Wizard");
        }

        public static string WizardHeader
        {
            get { return GetWizard("Header"); }
        }

        public static string WizardHint
        {
            get { return GetWizard("Hint"); }
        }
    }

    public sealed partial class Lang
    {
        private static string GetWizardDirectories(string name)
        {
            return Instance.Value.GetString(name, "Wizard", "Directories");
        }

        public static string WizardDirectoriesHeader
        {
            get { return GetWizardDirectories("Header"); }
        }

        public static string WizardDirectoriesGameDir
        {
            get { return GetWizardDirectories("GameDir"); }
        }

        public static string WizardDirectoriesWorkingDir
        {
            get { return GetWizardDirectories("WorkingDir"); }
        }

        public static string WizardDirectoriesCVSDir
        {
            get { return GetWizardDirectories("CVSDir"); }
        }
    }

    public sealed partial class Lang
    {
        private static string GetWizardArchives(string name)
        {
            return Instance.Value.GetString(name, "Wizard", "Archives");
        }

        public static string WizardArchivesHeader
        {
            get { return GetWizardArchives("Header"); }
        }

        public static string WizardArchivesArchiveList
        {
            get { return GetWizardArchives("ArchiveList"); }
        }

        public static string WizardArchivesArchiveName
        {
            get { return GetWizardArchives("ArchiveName"); }
        }

        public static string WizardArchivesArchiveState
        {
            get { return GetWizardArchives("ArchiveState"); }
        }
        
        public static string WizardArchivesExtension
        {
            get { return GetWizardArchives("Extension"); }
        }

        public static string WizardArchivesReserve
        {
            get { return GetWizardArchives("Reserve"); }
        }

        public static string WizardArchivesAbsoluteReserve
        {
            get { return GetWizardArchives("AbsoluteReserve"); }
        }

        public static string WizardArchivesRelativeReserve
        {
            get { return GetWizardArchives("RelativeReserve"); }
        }

        public static string WizardArchivesOptimize
        {
            get { return GetWizardArchives("Optimize"); }
        }
    }
}