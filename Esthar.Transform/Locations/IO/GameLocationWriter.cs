using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class GameLocationWriter : ILocationWriter
    {
        private readonly ArchiveDirectoryEntry _locationDirectory;
        private readonly string _name;

        public GameLocationWriter(ArchiveDirectoryEntry locationDirectory, string name)
        {
            _locationDirectory = Exceptions.CheckArgumentNull(locationDirectory, "locationDirectory");
            _name = Exceptions.CheckArgumentNullOrEmprty(name, "name");
        }

        public bool BeginWrite()
        {
            return true;
        }

        public bool EndWrite()
        {
            return true;
        }

        public void WriteTitle(string tags)
        {
        }

        public void WritePvP(uint? pvp)
        {
            throw new NotImplementedException();
        }

        public void WriteInformation(FieldInfo info)
        {
            throw new NotImplementedException();
        }

        public void WriteMonologues(LocalizableStrings monologues)
        {
            ArchiveFileEntry msdEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".msd");
            if (msdEntry == null)
            {
                if (!monologues.IsNullOrEmpty())
                    throw new NotImplementedException();
                
                return;
            }

            using (Stream output = msdEntry.OpenWritableCapacityStream())
            using (MsdFileWriter msdWriter = new MsdFileWriter(output))
            {
                msdWriter.WriteAllMonologues(monologues.Select(m=>m.Current));
                msdEntry.UpdateMetrics((int)output.Position, msdEntry.ContentOffset, Compression.None);
            }
        }

        public void WriteExtraFonts(GameFont extraFont)
        {
            throw new NotImplementedException();
        }

        public void WriteBackground(GameImage background)
        {
            throw new NotImplementedException();
        }

        public void WriteWalkmesh(Walkmesh walkmesh)
        {
            throw new NotImplementedException();
        }

        public void WriteCamera(FieldCameras fieldCameras)
        {
            throw new NotImplementedException();
        }

        public void WriteMoviesCameras(MovieCameras movieCameras)
        {
            throw new NotImplementedException();
        }

        public void WritePlaceables(Placeables placeables)
        {
            throw new NotImplementedException();
        }

        public void WriteAmbient(Ambient ambient)
        {
            throw new NotImplementedException();
        }

        public void WriteEncounters(Encounters encounters)
        {
            throw new NotImplementedException();
        }

        public void WriteScripts(AsmCollection asmCollection)
        {
            ArchiveFileEntry jsmEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".jsm");
            if (jsmEntry == null)
                return;

            throw new NotImplementedException();

            JsmHeader header = new JsmHeader();
            JsmGroup[] groups = null;
            JsmScript[] scripts = null;
            JsmOperation[] opertations = null;

            using (JsmFileWriter jsmWriter = new JsmFileWriter(jsmEntry.OpenWritableCapacityStream()))
            {
                jsmWriter.WriteScripts(header, groups, scripts, opertations);
            }

            ArchiveFileEntry symEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".sym");
            if (symEntry == null)
                return;

            //using (SymFileWriter symWriter = new SymFileWriter(symEntry.OpenWritableCapacityStream()))
            //{
            //}

            return;
        }

        public void WriteModels(SafeHGlobalHandle oneContent)
        {
            throw new NotImplementedException();
        }

        public void WriteParticles(Particles particles)
        {
            throw new NotImplementedException();
        }
    }
}
