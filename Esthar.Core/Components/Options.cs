using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Esthar.Core
{
    public sealed class Options
    {
        private const string ConfigFile = "Esthar.cfg";
        private const string RootTag = "Options";

        #region Initialization

        static Options()
        {
            try
            {
                if (File.Exists(ConfigFile))
                    Load();
                else
                    Create();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Ошибка при инициализации настроек приложения.");
                Environment.Exit(1);
            }
        }

        #endregion

        private static void Create()
        {
            GameDirectory = string.Empty;
            WorkingDirectory = string.Empty;
            CVSDirectory = string.Empty;
            AbsoluteReserve = 2048;
            RelativeReserve = 10;
            UserTags = new UserTagCollection(); 
            Codepage = FF8TextEncodingCodepage.Create();
            Encoding = new FF8TextEncoding(Codepage);
        }

        private static void Load()
        {
            XmlElement config = XmlHelper.LoadDocument(ConfigFile);
            
            XmlElement dirs = config["Directories"];
            if (dirs != null)
            {
                GameDirectory = dirs.FindString("Game");
                WorkingDirectory = dirs.FindString("Working");
                CVSDirectory = dirs.FindString("CVS");
            }

            XmlElement reserves = config["Reserves"];
            if (reserves != null)
            {
                AbsoluteReserve = reserves.FindUInt32("Absolute") ?? 0;
                RelativeReserve = reserves.FindUInt32("Relative") ?? 0;
            }

            UserTags = UserTagCollection.SafeLoad(config["UserTags"]);
        
            Codepage = FF8TextEncodingCodepage.Unserialize(config) ?? FF8TextEncodingCodepage.Create();
            Encoding = new FF8TextEncoding(Codepage);
        }

        public static void Save()
        {
            XmlElement config = XmlHelper.CreateDocument(RootTag);

            XmlElement dirs = config.EnsureChildElement("Directories");
            {
                dirs.SetString("Game", GameDirectory);
                dirs.SetString("Working", WorkingDirectory);
                dirs.SetString("CVS", CVSDirectory);
            }

            XmlElement reserves = config.EnsureChildElement("Reserves");
            {
                reserves.SetUInt32("Absolute", AbsoluteReserve);
                reserves.SetUInt32("Relative", RelativeReserve);
            }

            UserTags.Save(config.EnsureChildElement("UserTags"));
            Codepage.Serial(config);

            config.GetOwnerDocument().Save(ConfigFile);
        }

        public const string GameExecutableMask = "ff8*.exe";
        public const string GameExecutablesExpression = @"(?i)ff8(|_[a-z]{1,2})\.exe\Z";
        public const string GameDataDirectoryName = "Data";
        public const string GameDataDirectorySubName = "lang-en";

        public static readonly string[] GameArchivesNames = { "battle", "field", "magic", "main", "menu", "world" };

        public static string GameDirectory { get; set; }
        public static string WorkingDirectory { get; set; }
        public static string CVSDirectory { get; set; }

        public static uint AbsoluteReserve { get; set; }
        public static uint RelativeReserve { get; set; }

        public static UserTagCollection UserTags { get; set; }

        public static FF8TextEncodingCodepage Codepage { get; set; }
        public static FF8TextEncoding Encoding { get; set; }

        public static string GameDataDirectoryPath
        {
            get
            {
                string path = Path.Combine(GameDirectory, GameDataDirectoryName);
                string altPath = Path.Combine(path, GameDataDirectorySubName);
                return Directory.Exists(altPath) ? altPath : path;
            }
        }

        public static string FindGameExecutablePath()
        {
            string[] executables = Directory.GetFiles(GameDirectory, GameExecutableMask);
            return executables.FirstOrDefault(file => Regex.IsMatch(file, GameExecutablesExpression));
        }

        public static IEnumerable<string> GetGameArchivesPaths()
        {
            return GameArchivesNames.Select(name => Path.Combine(GameDataDirectoryPath, name + ".fs"));
        }
    }
}