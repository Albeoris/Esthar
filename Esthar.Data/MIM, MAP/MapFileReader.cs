using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using Esthar.Core;

namespace Esthar.Data
{
    public sealed class MapFileReader : GameFileReader
    {
        public int MinX;
        public int MinY;
        public int MaxX;
        public int MaxY;
        public int MaxTextureId;
        public int ColorsPerPalette;
        public MimTile[][] LayredTiles;

        public int Width
        {
            get { return MaxX - MinX + 16; }
        }

        public int Height
        {
            get { return MaxY - MinY + 16; }
        }

        public int LayersCount
        {
            get { return LayredTiles.Length; }
        }

        public int TexturesCount
        {
            get { return MaxTextureId + 1; }
        }

        public MapFileReader(Stream input)
            : base(input)
        {
        }

        public override void Close()
        {
            IOStream.Seek(0, SeekOrigin.Begin);

            MinX = Int32.MaxValue;
            MaxX = Int32.MinValue;
            MinY = Int32.MaxValue;
            MaxY = Int32.MinValue;
            MaxTextureId = Int32.MinValue;

            LayredTiles = null;
        }

        public override void Open()
        {
            Close();

            List<MimTile>[] layers = new List<MimTile>[8];

            while (!IOStream.IsEndOfStream())
            {
                MimTile tile = IOStream.ReadStruct<MimTile>();
                if (tile.IsEndOfFile())
                    break;

                List<MimTile> layer = layers.GetOrCreate(tile.Layered.LayerId, i => new List<MimTile>(4096));
                AddTile(tile, layer);
            }

            LayredTiles = layers.Where(l => l != null).Select(l => l.OrderByDescending(t => t.Layered.Z).ToArray()).ToArray();
        }

        private void AddTile(MimTile tile, List<MimTile> layer)
        {
            short x = tile.Layered.X;
            short y = tile.Layered.Y;
            short textureId = tile.Layered.TextureID;
            if (x < MinX)
                MinX = x;
            if (x > MaxX)
                MaxX = x;
            if (y < MinY)
                MinY = y;
            if (y > MaxY)
                MaxY = y;
            if (textureId > MaxTextureId)
                MaxTextureId = textureId;

            layer.Add(tile);
        }
    }
}