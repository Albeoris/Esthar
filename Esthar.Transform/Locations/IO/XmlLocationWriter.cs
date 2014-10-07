using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class XmlLocationWriter : ILocationWriter
    {
        private readonly string _xmlPath;
        private XmlElement _root;

        public XmlLocationWriter(string xmlPath)
        {
            _xmlPath = Exceptions.CheckArgumentNullOrEmprty(xmlPath, "xmlPath");
        }

        public bool BeginWrite()
        {
            FileCommander.EnsureDirectoryExists(_xmlPath);
            _root = File.Exists(_xmlPath) ? XmlHelper.LoadDocument(_xmlPath) : XmlHelper.CreateDocument("Location");
            return true;
        }

        public bool EndWrite()
        {
            FileCommander.EnsureDirectoryExists(_xmlPath);
            _root.GetOwnerDocument().Save(_xmlPath);
            return true;
        }

        public void WriteTags(UserTagCollection tags)
        {
            XmlElement node = _root.EnsureChildElement("Tags");
            node.RemoveAll();

            bool isExists = tags != null && tags.Count > 0;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            node.SetString("Value", String.Join(";", tags.Select(tag => tag.Name)));
        }

        public void WriteTitle(string tags)
        {
            XmlElement node = _root.EnsureChildElement("Title");
            node.RemoveAll();

            bool isExists = !string.IsNullOrEmpty(tags);
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            node.SetString("Value", tags);
        }

        public void WritePvP(uint? pvp)
        {
            XmlElement node = _root.EnsureChildElement("PVP");
            node.RemoveAll();
            
            bool isExists = pvp != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            node.SetUInt32("Value", pvp.Value);
        }

        public void WriteInformation(FieldInfo info)
        {
            XmlElement node = _root.EnsureChildElement("FieldInfo");
            node.RemoveAll();
            
            bool isExists = info != null;
            node.SetBoolean("IsExists", isExists);
            
            if (!isExists)
                return;

            node.SetString("Name", info.Name);
            node.SetByte("Direction", info.Direction);
            node.SetUInt16("FocusHeight", info.FocusHeight);

            XmlElement cameraRanges = node.EnsureChildElement("CameraRanges");
            foreach (RectInt16 range in info.CameraRanges)
                Write(range, cameraRanges.CreateChildElement("Range"));

            XmlElement screenRanges = node.EnsureChildElement("ScreenRanges");
            foreach (RectInt16 range in info.ScreenRanges)
                Write(range, screenRanges.CreateChildElement("Range"));

            XmlElement gateways = node.EnsureChildElement("Gateways");
            foreach (FieldGateway gateway in info.Gateways)
                Write(gateway, gateways.CreateChildElement("Gateway"));

            XmlElement triggers = node.EnsureChildElement("Triggers");
            foreach (FieldTrigger trigger in info.Triggers)
                Write(trigger, triggers.CreateChildElement("Trigger"));

            if (info.PvP != null)
                node.SetUInt32("PvP", info.PvP.Value);
            
            node.SetUInt32("Unknown", info.Unknown);
        }

        public void WriteMonologues(LocalizableStrings monologues)
        {
            XmlElement node = _root.EnsureChildElement("Monologues");
            node.RemoveAll();

            bool isExists = monologues != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            string dlgPath = Path.ChangeExtension(_xmlPath, ".dlg.xml");
            node = XmlHelper.CreateDocument("Monologues");

            foreach (LocalizableString monologue in monologues)
            {
                XmlElement child = node.CreateChildElement("Monologue");
                child.SetInt32("Order", monologue.Order);
                child.SetBoolean("IsIndent", monologue.IsIndent);
                child.SetString("Current", monologue.Current); 
                child.SetString("Original", monologue.Original);
            }

            node.GetOwnerDocument().Save(dlgPath);
        }

        public void WriteExtraFonts(GameFont extraFont)
        {
            XmlElement node = _root.EnsureChildElement("ExtraFont");
            node.RemoveAll();

            bool isExists = extraFont != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            string exfDirPath = Path.ChangeExtension(_xmlPath, ".exf");
            GameFontWriter.ToDirectory(extraFont, exfDirPath);
        }

        public void WriteBackground(GameImage background)
        {
            XmlElement node = _root.EnsureChildElement("Background");
            node.RemoveAll();

            bool isExists = background != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            GameImageWriter.ToDirectory(background, Path.ChangeExtension(_xmlPath, ".bgr"));
        }

        public void WriteWalkmesh(Walkmesh walkmesh)
        {
            XmlElement node = _root.EnsureChildElement("Walkmesh");
            node.RemoveAll();

            bool isExists = walkmesh != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            string wlkPath = Path.ChangeExtension(_xmlPath, ".wlk.xml");
            node = XmlHelper.CreateDocument("Walkmesh");

            XmlElement triangles = node.EnsureChildElement("Triangles");
            foreach (WalkmeshTriangle triangle in walkmesh.Triangles)
                Write(triangle, triangles.CreateChildElement("Triangle"));

            XmlElement passability = node.EnsureChildElement("Passability");
            foreach (WalkmeshPassability pass in walkmesh.Passability)
                Write(pass, passability.CreateChildElement("Pass"));

            node.GetOwnerDocument().Save(wlkPath);
        }

        public void WriteCamera(FieldCameras fieldCameras)
        {
            XmlElement node = _root.EnsureChildElement("FieldCameras");
            node.RemoveAll();

            bool isExists = fieldCameras != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            foreach (FieldCamera camera in fieldCameras)
                Write(camera, node.CreateChildElement("FieldCamera"));
        }

        public void WriteMoviesCameras(MovieCameras movieCameras)
        {
            XmlElement node = _root.EnsureChildElement("MovieCameras");
            node.RemoveAll();

            bool isExists = movieCameras != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            foreach (MovieCamera camera in movieCameras)
                Write(camera, node.CreateChildElement("MovieCamera"));
        }

        public void WritePlaceables(Placeables placeables)
        {
            XmlElement node = _root.EnsureChildElement("Placeables");
            node.RemoveAll();

            bool isExists = placeables != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            foreach (Placeable placeable in placeables)
                node.CreateChildElement("Placeable").SetUInt64("Value", BitConverter.ToUInt64(placeable.Unknown, 0));
        }

        public void WriteAmbient(Ambient ambient)
        {
            XmlElement node = _root.EnsureChildElement("Ambient");
            node.RemoveAll();

            bool isExists = ambient != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            foreach (uint soundId in ambient.SoundsIds)
                node.CreateChildElement("Sound").SetUInt32("Id", soundId);
        }

        public void WriteEncounters(Encounters encounters)
        {
            XmlElement node = _root.EnsureChildElement("Encounters");
            node.RemoveAll();

            bool isExists = encounters != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            node.SetByte("Frequency", encounters.Frequency);

            foreach (ushort enemyId in encounters.EnemiesID)
                node.CreateChildElement("Enemy").SetUInt16("Id", enemyId);
        }

        public void WriteScripts(AsmCollection scripts)
        {
            XmlElement node = _root.EnsureChildElement("Scripts");
            node.RemoveAll();

            bool isExists = scripts != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            string scriptsFile = Path.ChangeExtension(_xmlPath, ".sct");
            using (Stream output = new FileStream(scriptsFile, FileMode.Create, FileAccess.Write, FileShare.None))
            using (BinaryWriter bw = new BinaryWriter(output, Encoding.UTF8))
            {
                bw.Write("FF8S");
                bw.Write(scripts.Count);

                foreach (AsmModule module in scripts.GetOrderedModulesByIndex())
                {
                    bw.Write(module.ExecutionOrder);
                    bw.Write(module.Label);
                    bw.Write(module.Title);
                    bw.Write((int)module.Type);
                    bw.Write(module.Count);

                    foreach (AsmEvent script in module.GetOrderedEvents())
                    {
                        bw.Write(script.Title);
                        bw.Write(script.Count);
                        
                        foreach (JsmOperation operation in script)
                            bw.Write(operation.Operation);
                    }
                }
            }
        }

        public void WriteModels(SafeHGlobalHandle oneContent)
        {
            XmlElement node = _root.EnsureChildElement("Models");
            node.RemoveAll();

            bool isExists = oneContent != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            string oneFile = PathEx.ChangeName(_xmlPath, Path.ChangeExtension(_xmlPath, ".one"));
            using (Stream input = oneContent.OpenStream(FileAccess.Read))
            using (Stream output = new FileStream(oneFile, FileMode.Create, FileAccess.Write, FileShare.None))
                input.CopyTo(output);
        }

        public void WriteParticles(Particles particles)
        {
            XmlElement node = _root.EnsureChildElement("Particles");
            node.RemoveAll();

            bool isExists = particles != null;
            node.SetBoolean("IsExists", isExists);

            if (!isExists)
                return;

            string pmdFile = PathEx.ChangeName(_xmlPath, Path.ChangeExtension(_xmlPath, ".pmd"));
            string pmpFile = PathEx.ChangeName(_xmlPath, Path.ChangeExtension(_xmlPath, ".pmp"));

            using (Stream input = particles.PMDContent.OpenStream(FileAccess.Read))
            using (Stream output = new FileStream(pmdFile, FileMode.Create, FileAccess.Write, FileShare.None))
                input.CopyTo(output);

            using (Stream input = particles.PMPContent.OpenStream(FileAccess.Read))
            using (Stream output = new FileStream(pmpFile, FileMode.Create, FileAccess.Write, FileShare.None))
                input.CopyTo(output);
        }

        private void Write(FieldGateway gateway, XmlElement node)
        {
            node.SetUInt16("DestinationId", gateway.DestinationId);
            if (gateway.Unknown1 != null)
                node.SetInt32("Unknown1", gateway.Unknown1.Value);
            if (gateway.Unknown2 != null)
                node.SetInt32("Unknown2", gateway.Unknown2.Value);
            node.SetInt32("Unknown3", gateway.Unknown3);

            Write(gateway.Boundary, node.CreateChildElement("Boundary"));
            Write(gateway.DestinationPoint, node.CreateChildElement("DestinationPoint"));
        }

        private void Write(FieldTrigger trigger, XmlElement node)
        {
            node.SetByte("DoorID", trigger.DoorID);
            Write(trigger.Boundary, node.CreateChildElement("Boundary"));
        }

        private void Write(WalkmeshTriangle triangle, XmlElement node)
        {
            foreach (Vector3 coord in triangle.Coords)
                Write(coord, node.CreateChildElement("Coord"));
        }

        private void Write(WalkmeshPassability pass, XmlElement node)
        {
            foreach (ushort edge in pass.Edges)
                node.CreateChildElement("Edge").SetUInt16("Value", edge);
        }

        private void Write(FieldCamera camera, XmlElement node)
        {
            Write(camera.XAxis, node.CreateChildElement("XAxis"));
            Write(camera.YAxis, node.CreateChildElement("YAxis"));
            Write(camera.ZAxis, node.CreateChildElement("ZAxis"));
            Write(camera.Position, node.CreateChildElement("Position"));
            node.SetInt16("Zoom", camera.Zoom);
        }

        private void Write(MovieCamera camera, XmlElement node)
        {
            Write(camera.TopLeft, node.CreateChildElement("TopLeft"));
            Write(camera.TopRight, node.CreateChildElement("TopRight"));
            Write(camera.BottomRight, node.CreateChildElement("BottomRight"));
            Write(camera.BottomLeft, node.CreateChildElement("BottomLeft"));
        }

        private void Write(Vector3 vector, XmlElement node)
        {
            node.SetInt16("X", vector.X);
            node.SetInt16("Y", vector.Y);
            node.SetInt16("Z", vector.Z);
        }

        private void Write(Vector3Int32 vector, XmlElement node)
        {
            node.SetInt32("X", vector.X);
            node.SetInt32("Y", vector.Y);
            node.SetInt32("Z", vector.Z);
        }

        private void Write(Line3 line, XmlElement node)
        {
            Write(line.Begin, node.CreateChildElement("Begin"));
            Write(line.End, node.CreateChildElement("End"));
        }

        private void Write(RectInt16 rectInt16, XmlElement node)
        {
            node.SetInt16("Top", rectInt16.Top);
            node.SetInt16("Bottom", rectInt16.Bottom);
            node.SetInt16("Right", rectInt16.Right);
            node.SetInt16("Left", rectInt16.Left);
        }
    }
}