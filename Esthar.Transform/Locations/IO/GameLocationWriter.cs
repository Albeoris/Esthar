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

            JsmHeader header = new JsmHeader();
            List<JsmGroup> groups = new List<JsmGroup>(8);
            List<JsmScript> scripts = new List<JsmScript>(32);
            List<JsmOperation> operations = new List<JsmOperation>(512);

            foreach (AsmModule module in asmCollection.GetOrderedModules())
            {
                JsmGroup group = new JsmGroup(module.ExecutionOrder, module.Label, (byte)module.Count, (JsmModuleType)module.Type);
                header.IncrementCount(group.Type);
                groups.Add(group);
            }

            foreach (AsmModule module in asmCollection.GetOrderedModulesByIndex())
            {
                foreach (AsmEvent evt in module.GetOrderedEvents())
                {
                    JsmScript script = new JsmScript((ushort)operations.Count, evt.Flag);
                    scripts.Add(script);

                    foreach (JsmOperation command in evt)
                        operations.Add(command);
                }
            }

            scripts.Add(new JsmScript((ushort)operations.Count, false));

            header.ScriptsOffset = (ushort)(8 + groups.Count * 2);
            header.OperationsOffset = (ushort)(header.ScriptsOffset + scripts.Count * 2);

            using (JsmFileWriter jsmWriter = new JsmFileWriter(jsmEntry.OpenWritableCapacityStream()))
                jsmWriter.WriteScripts(header, groups, scripts, operations);

            //ArchiveFileEntry symEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".sym");
            //if (symEntry == null)
            //    return;

            //using (SymFileWriter symWriter = new SymFileWriter(symEntry.OpenWritableCapacityStream()))
            //{
            //}
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
