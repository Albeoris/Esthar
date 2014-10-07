using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class XmlLocationReader : ILocationReader
    {
        private readonly string _xmlPath;
        private XmlElement _root;

        public XmlLocationReader(string xmlPath)
        {
            _xmlPath = Exceptions.CheckArgumentNullOrEmprty(xmlPath, "xmlPath");
        }

        public bool BeginRead(Location location)
        {
            if (!File.Exists(_xmlPath))
                return false;

            XmlDocument doc = new XmlDocument();
            doc.Load(_xmlPath);
            _root = doc.GetDocumentElement();
            if (_root.Name != "Location")
                throw new Exception(String.Format("Incorrect root node name: '{0}'", _root.Name));

            if (IsExists("Tags")) location.Importable |= LocationProperty.Tags;
            if (IsExists("Title")) location.Importable |= LocationProperty.Title;
            if (IsExists("PvP")) location.Importable |= LocationProperty.PvP;
            if (IsExists("Information")) location.Importable |= LocationProperty.Information;
            if (IsExists("Monologues")) location.Importable |= LocationProperty.Monologues;
            if (IsExists("ExtraFonts")) location.Importable |= LocationProperty.ExtraFonts;
            if (IsExists("Background")) location.Importable |= LocationProperty.Background;
            if (IsExists("Walkmesh")) location.Importable |= LocationProperty.Walkmesh;
            if (IsExists("FieldCamera")) location.Importable |= LocationProperty.FieldCamera;
            if (IsExists("MoviesCameras")) location.Importable |= LocationProperty.MoviesCameras;
            if (IsExists("Placeables")) location.Importable |= LocationProperty.Placeables;
            if (IsExists("Ambient")) location.Importable |= LocationProperty.Ambient;
            if (IsExists("Encounters")) location.Importable |= LocationProperty.Encounters;
            if (IsExists("Scripts")) location.Importable |= LocationProperty.Scripts;
            if (IsExists("Models")) location.Importable |= LocationProperty.Models;
            if (IsExists("Particles")) location.Importable |= LocationProperty.Particles;

            return true;
        }

        public bool EndRead()
        {
            _root = null;
            return true;
        }

        public bool ReadTags(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Tags"];
            if (node == null || !node.GetBoolean("IsExists"))
                return false;

            string[] tagNames = (node.FindString("Value") ?? string.Empty).Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            location.Tags = new UserTagCollection(tagNames.Select(name => Options.UserTags.GetOrCreate(name)).Order());

            location.SaveRequest &= ~LocationProperty.Tags;
            location.Importable |= LocationProperty.Tags;
            return true;
        }

        public bool ReadTitle(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Title"];
            if (node == null)
                return false;

            if (node.GetBoolean("IsExists"))
            {
                string result = node.GetString("Value");
                location.Title = result;
            }

            location.SaveRequest &= ~LocationProperty.Title;
            location.Importable |= LocationProperty.Title;
            return true;
        }

        public bool ReadPvP(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["PVP"];
            if (node == null)
                return false;

            if (node.GetBoolean("IsExists"))
            {
                uint result = node.GetUInt32("Value");
                location.PVP = result;
            }

            location.SaveRequest &= ~LocationProperty.PvP;
            location.Importable |= LocationProperty.PvP;
            return true;
        }

        public bool ReadInformation(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["FieldInfo"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            string fdiPath = Path.ChangeExtension(_xmlPath, ".fdi.xml");
            node = XmlHelper.LoadDocument(fdiPath);

            string name = node.GetString("Name");
            byte direction = node.GetByte("Direction");
            ushort focusHeight = node.GetUInt16("FocusHeight");
            RectInt16[] cameraRanges = ReadRects(node["CameraRanges"]);
            RectInt16[] screenRanges = ReadRects(node["ScreenRanges"]);
            FieldGateway[] gateways = ReadGateways(node["Gateways"]);
            FieldTrigger[] triggers = ReadTriggers(node["Triggers"]);
            uint? pvp = node.FindUInt32("PvP");
            uint unknown = node.GetUInt32("Unknown");
            FieldInfo result = new FieldInfo(name, direction, focusHeight, cameraRanges, screenRanges, gateways, triggers, pvp, unknown);
            location.Info = result;

            location.SaveRequest &= ~LocationProperty.Information;
            location.Importable |= LocationProperty.Information;
            return true;
        }

        public bool ReadMonologues(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Monologues"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            string dlgPath = Path.ChangeExtension(_xmlPath, ".dlg.xml");
            node = XmlHelper.LoadDocument(dlgPath);

            location.Monologues = LocalizableString.FromXml(node);
            location.SaveRequest &= ~LocationProperty.Monologues;
            location.Importable |= LocationProperty.Monologues;

            return true;
        }

        public bool ReadExtraFonts(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["ExtraFont"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            string exfDirPath = Path.ChangeExtension(_xmlPath, ".exf");
            
            GameFont result = GameFontReader.FromDirectory(exfDirPath);
            location.ExtraFont = result;

            location.SaveRequest &= ~LocationProperty.ExtraFonts;
            location.Importable |= LocationProperty.ExtraFonts;
            return true;
        }

        public bool GetBackgroundReader(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Background"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            //GameImage result = GameImageReader.FromDirectory(Path.ChangeExtension(_xmlPath, ".bgr"));
            //location.Background = result.Layers[0];

            //return true;

            throw new NotImplementedException();
        }


        public bool ReadWalkmesh(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Walkmesh"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            string wlkPath = Path.ChangeExtension(_xmlPath, ".wlk.xml");
            XmlDocument wlkDoc = new XmlDocument();
            wlkDoc.Load(wlkPath);

            node = wlkDoc.GetDocumentElement();

            XmlElement trianglesNode = node.GetChildElement("Triangles");
            WalkmeshTriangle[] triangles = new WalkmeshTriangle[trianglesNode.ChildNodes.Count];
            for (int i = 0; i < triangles.Length; i++)
                triangles[i] = ReadWalkmeshTriangle((XmlElement)trianglesNode.ChildNodes[i]);

            XmlElement passabilityNode = node.GetChildElement("Passability");
            WalkmeshPassability[] passability = new WalkmeshPassability[passabilityNode.ChildNodes.Count];
            for (int i = 0; i < passability.Length; i++)
                passability[i] = ReadWalkmeshPassability((XmlElement)passabilityNode.ChildNodes[i]);

            Walkmesh result = new Walkmesh(triangles, passability);
            location.Walkmesh = result;

            location.SaveRequest &= ~LocationProperty.Walkmesh;
            location.Importable |= LocationProperty.Walkmesh;
            return true;
        }

        public bool ReadCamera(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["FieldCameras"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            FieldCameras fieldCameras = new FieldCameras(node.ChildNodes.Count);
            foreach (XmlElement child in node.ChildNodes)
                fieldCameras.Add(ReadFieldCamera(child));
            location.FieldCameras = fieldCameras;

            location.SaveRequest &= ~LocationProperty.FieldCamera;
            location.Importable |= LocationProperty.FieldCamera;
            return true;
        }

        public bool ReadMoviesCameras(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["MovieCameras"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            MovieCameras movieCameras = new MovieCameras(node.ChildNodes.Count);
            foreach (XmlElement child in node.ChildNodes)
                movieCameras.Add(ReadMovieCamera(child));
            location.MovieCameras = movieCameras;

            location.SaveRequest &= ~LocationProperty.MoviesCameras;
            location.Importable |= LocationProperty.MoviesCameras;
            return true;
        }

        public bool ReadPlaceables(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Placeables"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            Placeables placeables = new Placeables(node.ChildNodes.Count);
            foreach (XmlElement child in node.ChildNodes)
            {
                ulong value = child.GetUInt64("Value");
                placeables.Add(new Placeable(BitConverter.GetBytes(value)));
            }
            location.Placeables = placeables;

            location.SaveRequest &= ~LocationProperty.Placeables;
            location.Importable |= LocationProperty.Placeables;
            return true;
        }

        public bool ReadAmbient(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Ambient"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            uint[] soundsIds = new uint[node.ChildNodes.Count];
            for (int i = 0; i < soundsIds.Length; i++)
                soundsIds[i] = ((XmlElement)node.ChildNodes[i]).GetUInt32("Id");

            Ambient result = new Ambient(soundsIds);
            location.Ambient = result;

            location.SaveRequest &= ~LocationProperty.Ambient;
            location.Importable |= LocationProperty.Ambient;
            return true;
        }

        public bool ReadEncounters(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Encounters"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            ushort[] enemiesID = new ushort[node.ChildNodes.Count];
            for (int i = 0; i < enemiesID.Length; i++)
                enemiesID[i] = ((XmlElement)node.ChildNodes[i]).GetUInt16("Id");
            byte frequency = node.GetByte("Frequency");
            
            Encounters result = new Encounters(enemiesID, frequency);
            location.Encounters = result;

            location.SaveRequest &= ~LocationProperty.Encounters;
            location.Importable |= LocationProperty.Encounters;
            return true;
        }

        public bool ReadScripts(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Scripts"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            string scriptsFile = Path.ChangeExtension(_xmlPath, ".sct");
            if (!File.Exists(scriptsFile))
                return false;

            using (Stream input = new FileStream(scriptsFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (BinaryReader br = new BinaryReader(input, Encoding.UTF8))
            {
                string magicNumber = br.ReadString();
                if (magicNumber != "FF8S")
                    throw Exceptions.CreateException("Неверное магическое число: '{0}'", magicNumber);

                int groupsCount = br.ReadInt32();
                AsmModule previousModule = null;
                AsmCollection groups = new AsmCollection(groupsCount);
                for (ushort g = 0; g < groupsCount; g++)
                {
                    ushort executionOrder = br.ReadUInt16();
                    ushort label = br.ReadUInt16();
                    string title = br.ReadString();
                    Data.JsmModuleType type = (Data.JsmModuleType)br.ReadInt32();
                    int scriptsCount = br.ReadInt32();

                    AsmModule module = AsmModuleFactory.Create(type);
                    module.Index = g;
                    module.ExecutionOrder = executionOrder;
                    module.Label = label;
                    module.Title = title;
                    module.PreviousModule = previousModule;
                    if (previousModule != null) previousModule.NextModule = module;
                    previousModule = module;
                    groups.Add(label, module);
                    for (int s = 0; s < scriptsCount; s++)
                    {
                        string scriptLabel = br.ReadString();
                        int operationsCount = br.ReadInt32();

                        AsmEvent script = new AsmEvent(module, operationsCount) { Label = (ushort)(label + s + 1), Title = scriptLabel };
                        module.SetEventByIndex(s, script);
                        for (int o = 0; o < operationsCount; o++)
                        {
                            JsmOperation operation = new JsmOperation(br.ReadUInt32());
                            script.Add(operation);
                        }
                    }
                }
                location.Scripts = groups;
            }

            location.SaveRequest &= ~LocationProperty.Scripts;
            location.Importable |= LocationProperty.Scripts;
            return true;
        }

        public bool ReadModels(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Models"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            string oneFile = PathEx.ChangeName(_xmlPath, Path.ChangeExtension(_xmlPath, ".one"));
            using (Stream input = new FileStream(oneFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                SafeHGlobalHandle oneContent = new SafeHGlobalHandle((int)input.Length);
                try
                {
                    using (Stream output = oneContent.OpenStream(FileAccess.Write))
                        input.CopyTo(output);
                    location.Models = oneContent;
                }
                catch
                {
                    oneContent.SafeDispose();
                    throw;
                }
            }

            location.SaveRequest &= ~LocationProperty.Models;
            location.Importable |= LocationProperty.Models;
            return true;
        }

        public bool ReadParticles(Location location)
        {
            if (_root == null)
                return false;

            XmlElement node = _root["Particles"];
            if (node == null)
                return false;

            if (!node.GetBoolean("IsExists"))
                return true;

            SafeHGlobalHandle pmdContent, pmpContent;
            string pmdFile = PathEx.ChangeName(_xmlPath, Path.ChangeExtension(_xmlPath, ".pmd"));
            string pmpFile = PathEx.ChangeName(_xmlPath, Path.ChangeExtension(_xmlPath, ".pmp"));

            using (Stream input = new FileStream(pmdFile, FileMode.Open, FileAccess.Read, FileShare.Read))
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

            using (Stream input = new FileStream(pmpFile, FileMode.Open, FileAccess.Read, FileShare.Read))
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
            location.Importable |= LocationProperty.Particles;
            return true;
        }

        private FieldGateway[] ReadGateways(XmlElement node)
        {
            if (node == null)
                return null;

            var result = new List<FieldGateway>(node.ChildNodes.Count);
            foreach (XmlElement child in node.ChildNodes)
            {
                Line3 boundary = ReadLine3(child["Boundary"]);
                ushort destinationId = child.GetUInt16("DestinationId");
                Vector3 destinationPoint = ReadVector3(child["DestinationPoint"]);
                int? unknown1 = child.FindInt32("Unknown1");
                int? unknown2 = child.FindInt32("Unknown2");
                int unknown3 = child.GetInt32("Unknown3");
                FieldGateway gateway = new FieldGateway(boundary, destinationId, destinationPoint, unknown1, unknown2, unknown3);
                result.Add(gateway);
            }

            return result.ToArray();
        }

        private FieldTrigger[] ReadTriggers(XmlElement node)
        {
            if (node == null)
                return null;

            var result = new List<FieldTrigger>(node.ChildNodes.Count);
            foreach (XmlElement child in node.ChildNodes)
            {
                byte doorId = child.GetByte("DoorID");
                Line3 boundary = ReadLine3(child["Boundary"]);
                FieldTrigger trigger = new FieldTrigger(doorId, boundary);
                result.Add(trigger);
            }
            return result.ToArray();
        }

        private WalkmeshTriangle ReadWalkmeshTriangle(XmlElement node)
        {
            Vector3[] coords = new Vector3[node.ChildNodes.Count];
            for (int i = 0; i < coords.Length; i++)
                coords[i] = ReadVector3((XmlElement)node.ChildNodes[i]);
            return new WalkmeshTriangle(coords);
        }

        private WalkmeshPassability ReadWalkmeshPassability(XmlElement node)
        {
            ushort[] edges = new ushort[node.ChildNodes.Count];
            for (int i = 0; i < edges.Length; i++)
                edges[i] = ((XmlElement)node.ChildNodes[i]).GetUInt16("Value");
            return new WalkmeshPassability(edges);
        }

        private FieldCamera ReadFieldCamera(XmlElement node)
        {
            return new FieldCamera
            {
                XAxis = ReadVector3(node["XAxis"]),
                YAxis = ReadVector3(node["YAxis"]),
                ZAxis = ReadVector3(node["ZAxis"]),
                Position = ReadVector3Int32(node["Position"]),
                Zoom = node.GetInt16("Zoom")
            };
        }

        private MovieCamera ReadMovieCamera(XmlElement node)
        {
            return new MovieCamera
            {
                TopLeft = ReadVector3(node["TopLeft"]),
                TopRight = ReadVector3(node["TopRight"]),
                BottomRight = ReadVector3(node["BottomRight"]),
                BottomLeft = ReadVector3(node["BottomLeft"])
            };
        }

        private Vector3 ReadVector3(XmlElement node)
        {
            return new Vector3
            {
                X = node.GetInt16("X"),
                Y = node.GetInt16("Y"),
                Z = node.GetInt16("Z")
            };
        }

        private Vector3Int32 ReadVector3Int32(XmlElement node)
        {
            return new Vector3Int32
            {
                X = node.GetInt32("X"),
                Y = node.GetInt32("Y"),
                Z = node.GetInt32("Z")
            };
        }

        private Line3 ReadLine3(XmlElement node)
        {
            return new Line3
            {
                Begin = ReadVector3(node["Begin"]),
                End = ReadVector3(node["End"])
            };
        }

        private RectInt16[] ReadRects(XmlElement node)
        {
            if (node == null)
                return null;

            var result = new List<RectInt16>(node.ChildNodes.Count);
            foreach (XmlElement child in node)
            {
                RectInt16 rectInt16 = new RectInt16
                {
                    Top = child.GetInt16("Top"),
                    Bottom = child.GetInt16("Bottom"),
                    Right = child.GetInt16("Right"),
                    Left = child.GetInt16("Left")
                };
                result.Add(rectInt16);
            }
            return result.ToArray();
        }

        private bool IsExists(string nodeName)
        {
            XmlElement node = _root[nodeName];
            if (node == null)
                return false;

            return node.GetBoolean("IsExists");
        }
    }
}
