using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public sealed class Location : IUserTagsHandler, IDisposable
    {
        private readonly GameLocationReader _gameReader;
        private readonly GameLocationWriter _gameWriter;
        private readonly XmlLocationReader _xmlReader;
        private readonly XmlLocationWriter _xmlWriter;
        private readonly LocationReaders _readers;

        public Location(string name, GameLocationReader gameReader, GameLocationWriter gameWriter, XmlLocationReader xmlReader, XmlLocationWriter xmlWriter)
        {
            Name = Exceptions.CheckArgumentNullOrEmprty(name, "name");
            _gameReader = Exceptions.CheckArgumentNull(gameReader, "gameReader");
            _gameWriter = Exceptions.CheckArgumentNull(gameWriter, "gameWriter");
            _xmlReader = Exceptions.CheckArgumentNull(xmlReader, "xmlReader");
            _xmlWriter = Exceptions.CheckArgumentNull(xmlWriter, "xmlWriter");
            _readers = new LocationReaders(this, _xmlReader, _gameReader);
        }

        public void Dispose()
        {
            ExtraFont.SafeDispose();
            Background.SafeDispose();
            Models.SafeDispose();
            Particles.SafeDispose();
        }

        #region Properties

        public string Name { get; set; }
        public string Title { get; set; }
        public uint? PVP { get; set; }
        public FieldInfo Info { get; set; }
        public LocalizableStrings Monologues { get; set; }
        public GameFont ExtraFont { get; set; }
        public GLTexture Background { get; set; }
        public GLTextureReader BackgroundReader { get; set; }
        public Walkmesh Walkmesh { get; set; }
        public FieldCameras FieldCameras { get; set; }
        public MovieCameras MovieCameras { get; set; }
        public Placeables Placeables { get; set; }
        public Ambient Ambient { get; set; }
        public Encounters Encounters { get; set; }
        public AsmCollection Scripts { get; set; }
        public SafeHGlobalHandle Models { get; set; }
        public Particles Particles { get; set; }

        public int TitleId { get; private set; }
        public UserTagCollection Tags { get; set; }

        #endregion

        public event Action<LocationProperty> SaveRequested;
        public LocationProperty SaveRequest;
        public LocationProperty Importable;

        public List<MessageWindow> MessageWindows = new List<MessageWindow>(16);
        public List<AsmCommand> ScriptCommands = new List<AsmCommand>(16);

        public void LoadBaseInformation()
        {
            using (_readers.BeginRead())
            {
                _readers.ReadTags();
                _readers.ReadScripts();
                _readers.ReadTitle();
                ParseScripts();
            }
        }

        public GLTextureReader GetBackgroundReader()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _readers.GetBackgroundReader();

            if (BackgroundReader != null)
                BackgroundReader.TextureReaded += b => Background = b;
            sw.Stop();
            return BackgroundReader;
        }

        public void LoadText()
        {
            if ((SaveRequest & LocationProperty.Monologues) == LocationProperty.Monologues)
                return;

            using (_readers.BeginRead())
            {
                _readers.ReadMonologues();

                foreach (MessageWindow window in MessageWindows)
                    window.Message = Monologues[window.MessageId];
            }
        }

        public void UnloadText()
        {
            if ((SaveRequest & LocationProperty.Monologues) == LocationProperty.Monologues)
                return;

            Monologues = null;
        }

        public void SaveRequestedData()
        {
            if (SaveRequest == LocationProperty.None)
                return;

            SaveRequested.NullSafeInvoke(SaveRequest);

            using (new DisposableBeginEndActions(() => _xmlWriter.BeginWrite(), () => _xmlWriter.EndWrite()))
            {
                if ((SaveRequest & LocationProperty.Tags) == LocationProperty.Tags)
                {
                    _xmlWriter.WriteTags(Tags);
                    SaveRequest &= ~LocationProperty.Tags;
                    Importable |= LocationProperty.Tags;
                }

                if ((SaveRequest & LocationProperty.Scripts) == LocationProperty.Scripts)
                {
                    foreach (AsmCommand command in ScriptCommands)
                        command.CommitChanges();

                    _xmlWriter.WriteScripts(Scripts);
                    SaveRequest &= ~LocationProperty.Scripts;
                    Importable |= LocationProperty.Scripts;
                }

                if ((SaveRequest & LocationProperty.Monologues) == LocationProperty.Monologues)
                {
                    _xmlWriter.WriteMonologues(Monologues);
                    SaveRequest &= ~LocationProperty.Monologues;
                    Importable |= LocationProperty.Monologues;
                }

                if (SaveRequest != LocationProperty.None)
                    throw new NotImplementedException();
            }
        }

        public void ImportRequestedData()
        {
            if (Importable == LocationProperty.None)
                return;

            using (new DisposableBeginEndActions(() => _xmlReader.BeginRead(this), () => _xmlReader.EndRead()))
            using (new DisposableBeginEndActions(() => _gameWriter.BeginWrite(), () => _gameWriter.EndWrite()))
            {
                //if ((Importable & LocationProperty.Scripts) == LocationProperty.Scripts)
                //{
                //    if (Scripts == null) _xmlReader.ReadScripts(this);
                //    _gameWriter.WriteScripts(Scripts);
                //}

                if ((Importable & LocationProperty.Monologues) == LocationProperty.Monologues)
                {
                    if (Monologues == null) _xmlReader.ReadMonologues(this);
                    _gameWriter.WriteMonologues(Monologues);
                }
            }
        }

        public void ParseScripts()
        {
            MessageWindows.Clear();
            ScriptCommands.Clear();

            if (Scripts == null)
                return;

            foreach (AsmModule child in Scripts.GetOrderedModules())
            {
                foreach (AsmEvent evt in child.GetOrderedEvents())
                {
                    AsmCommandFactory factory = new AsmCommandFactory(evt);
                    factory.RegisterCreateHandler<AsmSetPlaceCommand>(OnAsmSetPlace);
                    factory.RegisterCreateHandler<AsmMessageCommand>(OnMessageWindowCommandCreated);
                    factory.RegisterCreateHandler<AsmAppearMessageCommand>(OnMessageWindowCommandCreated);
                    factory.RegisterCreateHandler<AsmAppearMessageAndWaitCommand>(OnMessageWindowCommandCreated);
                    factory.RegisterCreateHandler<AsmResumeScriptAppearMessageAndWaitCommand>(OnMessageWindowCommandCreated);
                    factory.RegisterCreateHandler<AsmAskCommand>(OnMessageWindowCommandCreated);
                    factory.RegisterCreateHandler<AsmAppearAskCommand>(OnMessageWindowCommandCreated);
                    ScriptCommands.AddRange(factory.FindAll(JsmCommand.SETPLACE, JsmCommand.MES, JsmCommand.AMES, JsmCommand.AMESW, JsmCommand.RAMESW, JsmCommand.ASK, JsmCommand.AASK));
                }
            }
        }

        public void OnAsmSetPlace(AsmSetPlaceCommand command)
        {
            TitleId = command.TextId.ResolveValue();
            Title = Title ?? Glossary.AreaNames[TitleId].Current;
        }

        private void OnMessageWindowCommandCreated(AsmCommand command)
        {
            MessageWindow window = MessageWindow.FromCommand(command);
            MessageWindows.Add(window);
        }

        public bool MatchNameFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;

            if (Name.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                return true;

            return false;
        }

        public bool MatchTitleFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return false;

            return Title != null && Title.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0;
        }
    }
}