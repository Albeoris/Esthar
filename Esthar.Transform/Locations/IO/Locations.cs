using System;
using System.Collections.Generic;
using System.IO;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public static class Locations
    {
        private const string MaplistFileName = "maplist";
        private const string TargetSubPath = @"Locations";
        private const string SourcePath = @"c:\ff8\data\eng\field\mapdata";

        public static Location[] GetLocationList(Action<long> totalCallback, Action<long> progressCallback)
        {
            List<Location> result = new List<Location>(850);

            try
            {
                string mapFilePath = Path.Combine(SourcePath, MaplistFileName);
                ArchiveFileEntry mapFile = Archives.GetEntry<ArchiveFileEntry>(mapFilePath);

                totalCallback.Invoke(result.Capacity);

                using (FileSegment stream = mapFile.OpenReadableContentStream())
                using (StreamReader sr = stream.GetStreamReader())
                    while (!sr.EndOfStream)
                    {
                        string name = sr.ReadLine();
                        if (name == "testno")
                        {
                            progressCallback.Invoke(1);
                            continue;
                        }

                        Location location = FindLocation(name);
                        if (location == null)
                        {
                            progressCallback.Invoke(1);
                            continue;
                        }

                        result.Add(location);
                        location.LoadBaseInformation();
                        progressCallback.Invoke(1);
                    }

                totalCallback.Invoke(result.Count);
            }
            catch
            {
                result.SafeDispose();
                throw;
            }

            return result.ToArray();
        }

        public static Location FindLocation(string name)
        {
            Exceptions.CheckArgumentNullOrEmprty(name, "name");
            if (name.Length < 2)
                throw new ArgumentException("name");

            string transformedPath = GetTransformedPath(name);

            ArchiveDirectoryEntry locationDirectory = FindLocationDirectory(name);
            if (locationDirectory == null)
                return null;

            GameLocationReader gameReader = new GameLocationReader(locationDirectory, name);
            GameLocationWriter gameWriter = new GameLocationWriter(locationDirectory, name);
            XmlLocationReader xmlReader = new XmlLocationReader(transformedPath);
            XmlLocationWriter xmlWriter = new XmlLocationWriter(transformedPath);

            return new Location(name, gameReader, gameWriter, xmlReader, xmlWriter);
        }

        private static string GetTransformedPath(string name)
        {
            return Path.Combine(Options.WorkingDirectory, TargetSubPath, Path.ChangeExtension(name, null), Path.ChangeExtension(name, ".xml"));
        }

        private static ArchiveDirectoryEntry FindLocationDirectory(string name)
        {
            string directoryName = name.Substring(0, 2);
            string sourcePath = Path.Combine(SourcePath, directoryName, name);

            return Archives.FindEntry<ArchiveDirectoryEntry>(sourcePath);
        }
    }
}
