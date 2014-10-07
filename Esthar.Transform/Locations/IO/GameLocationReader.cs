using System.IO;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class GameLocationReader : ILocationReader
    {
        private readonly ArchiveDirectoryEntry _locationDirectory;
        private readonly string _name;

        public GameLocationReader(ArchiveDirectoryEntry locationDirectory, string name)
        {
            _locationDirectory = Exceptions.CheckArgumentNull(locationDirectory, "locationDirectory");
            _name = Exceptions.CheckArgumentNullOrEmprty(name, "name");
        }

        public bool BeginRead(Location location)
        {
            return true;
        }

        public bool EndRead()
        {
            return true;
        }

        public bool ReadTags(Location location)
        {
            location.Tags = new UserTagCollection();
            return true;
        }

        public bool ReadTitle(Location location)
        {
            location.Title = null;
            location.SaveRequest &= ~LocationProperty.Title;
            location.Importable &= ~LocationProperty.Title;
            return true;
        }

        public bool ReadPvP(Location location)
        {
            ArchiveFileEntry pvpEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".pvp");
            if (pvpEntry == null)
                return true;

            using (PvpFileReader pvpReader = new PvpFileReader(pvpEntry.OpenReadableContentStream()))
                location.PVP = pvpReader.Value;

            location.SaveRequest &= ~LocationProperty.PvP;
            location.Importable &= ~LocationProperty.PvP;
            return true;
        }

        public bool ReadInformation(Location location)
        {
            ArchiveFileEntry infEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".inf");
            if (infEntry == null)
                return true;

            using (InfFileReader infReader = new InfFileReader(infEntry.OpenReadableContentStream()))
            {
                InfEntry entry = infReader.Entry;
                FieldInfo result = new FieldInfo(
                    entry.Name,
                    entry.Direction,
                    entry.HeightCameraFocus,
                    entry.CameraRanges,
                    entry.ScreenRanges,
                    entry.Gateways.Select(g => new FieldGateway(g.SourceLine, g.TargetFieldID, g.TargetPoint, g.Unknown1, g.Unknown2, g.Unknown3)).ToArray(),
                    entry.Triggers.Select(t => new FieldTrigger(t.DoorID, t.SourceLine)).ToArray(),
                    entry.PvP,
                    entry.Unknown);

                location.Info = result;
            }

            location.SaveRequest &= ~LocationProperty.Information;
            location.Importable &= ~LocationProperty.Information;
            return true;
        }

        public bool ReadMonologues(Location location)
        {
            ArchiveFileEntry msdEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".msd");
            if (msdEntry == null)
                return true;

            using (MsdFileReader msdReader = new MsdFileReader(msdEntry.OpenReadableContentStream()))
            {
                string[] strings = msdReader.ReadAllMonologues();
                LocalizableStrings monologues = new LocalizableStrings(strings.Length);
                foreach (string str in strings)
                {
                    LocalizableString localizableString = new LocalizableString(str, str);
                    monologues.Add(localizableString);
                }
                location.Monologues = monologues;
            }

            location.SaveRequest &= ~LocationProperty.Monologues;
            location.Importable &= ~LocationProperty.Monologues;
            return true;
        }

        public bool ReadExtraFonts(Location location)
        {
            GameFont font = GameFontReader.FromGameData(_locationDirectory, _name);
            location.ExtraFont = font;

            location.SaveRequest &= ~LocationProperty.ExtraFonts;
            location.Importable &= ~LocationProperty.ExtraFonts;
            return true;
        }

        public bool GetBackgroundReader(Location location)
        {
            ArchiveFileEntry mapEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".map");
            ArchiveFileEntry mimEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".mim");
            if (mapEntry == null || mimEntry == null)
                return true;

            MimFileReader mimReader = new MimFileReader(mimEntry.OpenReadableContentStream());
            MapFileReader mapReader = new MapFileReader(mapEntry.OpenReadableContentStream());
            location.BackgroundReader = new MimGLTextureReader(mimReader, mapReader, true);

            location.SaveRequest &= ~LocationProperty.Background;
            location.Importable &= ~LocationProperty.Background;
            return true;
        }

        public bool ReadWalkmesh(Location location)
        {
            ArchiveFileEntry idEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".id");
            if (idEntry == null)
                return true;

            using (IdFileReader idReader = new IdFileReader(idEntry.OpenReadableContentStream()))
            {
                WalkmeshTriangle[] triangles = idReader.Triangles.Select(t => new WalkmeshTriangle(t.Vertices.Select(v => v.Point).ToArray())).ToArray();
                WalkmeshPassability[] passability = idReader.Accesses.Select(a => new WalkmeshPassability(a.Accesses)).ToArray();
                Walkmesh result = new Walkmesh(triangles, passability);
                location.Walkmesh = result;
            }

            location.SaveRequest &= ~LocationProperty.Walkmesh;
            location.Importable &= ~LocationProperty.Walkmesh;
            return true;
        }

        public bool ReadCamera(Location location)
        {
            ArchiveFileEntry caEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".ca");
            if (caEntry == null)
                return true;

            using (CaFileReader caReader = new CaFileReader(caEntry.OpenReadableContentStream()))
            {
                FieldCameras cameras = new FieldCameras(caReader.Rects.Length);
                foreach (CaCamera rect in caReader.Rects)
                {
                    FieldCamera camera = new FieldCamera
                    {
                        XAxis = rect.XAxis,
                        YAxis = rect.YAxis,
                        ZAxis = rect.ZAxis,
                        Position = rect.Position,
                        Zoom = rect.Zoom
                    };
                    cameras.Add(camera);
                }
                location.FieldCameras = cameras;
            }

            location.SaveRequest &= ~LocationProperty.FieldCamera;
            location.Importable &= ~LocationProperty.FieldCamera;
            return true;
        }

        public bool ReadMoviesCameras(Location location)
        {
            ArchiveFileEntry mskEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".msk");
            if (mskEntry == null)
                return true;

            using (MskFileReader mskReader = new MskFileReader(mskEntry.OpenReadableContentStream()))
            {
                MovieCameras cameras = new MovieCameras(mskReader.Rects.Length);
                foreach (MskRect rect in mskReader.Rects)
                {
                    MovieCamera camera = new MovieCamera
                    {
                        TopLeft = rect.Top,
                        TopRight = rect.Bottom,
                        BottomRight = rect.Right,
                        BottomLeft = rect.Left
                    };
                    cameras.Add(camera);
                }
                location.MovieCameras = cameras;
            }

            location.SaveRequest &= ~LocationProperty.MoviesCameras;
            location.Importable &= ~LocationProperty.MoviesCameras;
            return true;
        }

        public bool ReadPlaceables(Location location)
        {
            ArchiveFileEntry pcbEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".pcb");
            if (pcbEntry == null)
                return true;

            using (PcbFileReader pcbReader = new PcbFileReader(pcbEntry.OpenReadableContentStream()))
            {
                Placeables placeables = new Placeables(pcbReader.Entries.Length);
                for (int i = 0; i < placeables.Capacity; i++)
                    placeables.Add(new Placeable(pcbReader.Entries[i].Unknown));
                location.Placeables = placeables;
            }

            location.SaveRequest &= ~LocationProperty.Placeables;
            location.Importable &= ~LocationProperty.Placeables;
            return true;
        }

        public bool ReadAmbient(Location location)
        {
            ArchiveFileEntry sfxEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".sfx");
            if (sfxEntry == null)
                return true;

            using (SfxFileReader sfxReader = new SfxFileReader(sfxEntry.OpenReadableContentStream()))
            {
                Ambient result = new Ambient(sfxReader.SoundIds);
                location.Ambient = result;
            }

            location.SaveRequest &= ~LocationProperty.Ambient;
            location.Importable &= ~LocationProperty.Ambient;
            return true;
        }

        public bool ReadEncounters(Location location)
        {
            ArchiveFileEntry mrtEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".mrt");
            ArchiveFileEntry ratEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".rat");
            if (mrtEntry == null || ratEntry == null)
                return true;

            using (MrtFileReader mrtReader = new MrtFileReader(mrtEntry.OpenReadableContentStream()))
            using (RatFileReader ratReader = new RatFileReader(ratEntry.OpenReadableContentStream()))
            {
                Encounters result = new Encounters(mrtReader.Troops, (byte)(ratReader.Rates & 0xFF));
                location.Encounters = result;
            }

            location.SaveRequest &= ~LocationProperty.Encounters;
            location.Importable &= ~LocationProperty.Encounters;
            return true;
        }

        public bool ReadScripts(Location location)
        {
            ArchiveFileEntry jsmEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".jsm");
            if (jsmEntry == null)
                return true;

            AsmCollection result;
            using (JsmFileReader jsmReader = new JsmFileReader(jsmEntry.OpenReadableContentStream()))
            {
                AsmModule previousModule = null;
                result = new AsmCollection(jsmReader.Groups.Length);
                for (ushort g = 0, s = 0, o = 0; g < jsmReader.Groups.Length; g++)
                {
                    JsmGroup jsmGroup = jsmReader.Groups[g];
                    AsmModule asmGroup = AsmModuleFactory.Create(jsmGroup.Type);
                    asmGroup.Index = g;
                    asmGroup.ExecutionOrder = jsmGroup.ExecutionOrder;
                    asmGroup.Label = jsmGroup.Label;
                    asmGroup.Title = jsmGroup.Label.ToString("D3");
                    asmGroup.PreviousModule = previousModule;
                    if (previousModule != null) previousModule.NextModule = asmGroup;
                    previousModule = asmGroup;
                    result.Add(asmGroup.Label, asmGroup);

                    for (int i = 0; i < jsmGroup.ScriptsCount; i++)
                    {
                        JsmScript jsmScript = jsmReader.Scripts[s++];
                        AsmEvent asmScript = new AsmEvent(asmGroup, jsmScript.OperationsCount)
                        {
                            Flag = jsmScript.Flag,
                            Label = (ushort)(asmGroup.Label + i)
                        };
                        asmGroup.SetEventByIndex(i, asmScript);

                        for (int k = 0; k < jsmScript.OperationsCount; k++)
                        {
                            JsmOperation jsmOperation = jsmReader.Opertations[o++];
                            asmScript.Add(jsmOperation);
                        }
                    }
                }
            }

            ArchiveFileEntry symEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".sym");
            if (symEntry == null)
            {
                location.Scripts = result;
                return true;
            }

            using (SymFileReader symReader = new SymFileReader(symEntry.OpenReadableContentStream()))
            {
                foreach (AsmModule module in result.GetOrderedModules())
                {
                    module.Title = symReader.Labels[module.Label];

                    foreach (AsmEvent evt in module.GetOrderedEvents())
                        evt.Title = symReader.Labels[evt.Label];
                }
            }

            location.Scripts = result;
            
            location.SaveRequest &= ~LocationProperty.Scripts;
            location.Importable &= ~LocationProperty.Scripts;
            return true;
        }

        public bool ReadModels(Location location)
        {
            ArchiveFileEntry oneEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue("chara.one");
            if (oneEntry == null)
                return true;

            using (OneFileReader oneReader = new OneFileReader(oneEntry.OpenReadableContentStream()))
            {
                SafeHGlobalHandle result = oneReader.ReadData();
                location.Models = result;
            }

            location.SaveRequest &= ~LocationProperty.Models;
            location.Importable &= ~LocationProperty.Models;
            return true;
        }

        public bool ReadParticles(Location location)
        {
            ArchiveFileEntry pmdEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".pmd");
            ArchiveFileEntry pmpEntry = (ArchiveFileEntry)_locationDirectory.Childs.TryGetValue(_name + ".pmp");
            if (pmdEntry == null || pmpEntry == null)
                return true;
            
            SafeHGlobalHandle pmdContent, pmpContent;

            using (Stream input = pmdEntry.OpenReadableContentStream())
            {
                pmdContent = new SafeHGlobalHandle((int)input.Length);
                try
                {
                    using (Stream output = pmdContent.OpenStream(FileAccess.Write))
                        input.CopyTo(output);
                }
                catch
                {
                    pmdContent.SafeDispose();
                    throw;
                }
            }

            using (Stream input = pmpEntry.OpenReadableContentStream())
            {
                pmpContent = new SafeHGlobalHandle((int)input.Length);
                try
                {
                    using (Stream output = pmpContent.OpenStream(FileAccess.Write))
                        input.CopyTo(output);
                }
                catch
                {
                    pmpContent.SafeDispose();
                    throw;
                }
            }

            Particles result = new Particles(pmdContent, pmpContent);
            location.Particles = result;

            location.SaveRequest &= ~LocationProperty.Particles;
            location.Importable &= ~LocationProperty.Particles;
            return true;
        }
    }
}