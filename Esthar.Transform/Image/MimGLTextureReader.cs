using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using Esthar.Core;
using Esthar.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace Esthar.Data.Transform
{
    public sealed class MimGLTextureReader : GLTextureReader
    {
        private readonly MimFileReader _mim;
        private readonly MapFileReader _map;
        private readonly bool _accessorsOwner;
        private readonly byte[] _buff3 = new byte[3];

        public MimGLTextureReader(MimFileReader mim, MapFileReader map, bool accessorsOwner)
        {
            try
            {
                _mim = Exceptions.CheckArgumentNull(mim, "mim");
                _map = Exceptions.CheckArgumentNull(map, "map");
                _accessorsOwner = accessorsOwner;

                if (_map.LayersCount != 1)
                    throw new ArgumentException("map");
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_accessorsOwner)
            {
                _mim.SafeDispose();
                _map.SafeDispose();
            }
        }

        public async override Task<GLTexture> ReadTextureAsync(CancellationToken cancelationToken)
        {
            if (cancelationToken.IsCancellationRequested)
                return RaiseTextureReaded(null);

            using (SafeHGlobalHandle pixels = new SafeHGlobalHandle(_map.Width * _map.Height * 3))
            {
                using (Stream stream = pixels.OpenStream(FileAccess.ReadWrite))
                {
                    if (cancelationToken.IsCancellationRequested)
                        return RaiseTextureReaded(null);

                    CreateBackground(stream);

                    foreach (MimTile tile in _map.LayredTiles[0])
                    {
                        if (cancelationToken.IsCancellationRequested)
                            return RaiseTextureReaded(null);

                        //if (tile.Layered.Z < _map.LayredTiles[0][0].Layered.Z)
                        //    break;

                        int position = tile.Layered.GetPositionInImage(_map, _map.Width * 3);
                        stream.Seek(position, SeekOrigin.Begin);

                        Color[] colors = ReadTileColors(tile);
                        for (int i = 0; i < 16; i++)
                        {
                            for (int k = 0; k < 16; k++)
                                WriteColor(colors[i * 16 + k], tile.Layered.BlendType, stream);

                            if (i < 15)
                                stream.Seek((_map.Width - 16) * 3, SeekOrigin.Current);
                        }
                    }
                }

                if (cancelationToken.IsCancellationRequested)
                    return RaiseTextureReaded(null);

                PixelFormatDescriptor pixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;

                int textureId;
                using (GLService.AcquireContext())
                {
                    GL.GenTextures(1, out textureId);
                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    GL.TexImage2D(TextureTarget.Texture2D, 0, pixelFormat, _map.Width, _map.Height, 0, pixelFormat, pixelFormat, pixels.DangerousGetHandle());

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                }

                if (cancelationToken.IsCancellationRequested)
                    return RaiseTextureReaded(null);
                
                return new GLTexture(textureId, _map.Width, _map.Height, pixelFormat);
            }
        }

        private void CreateBackground(Stream stream)
        {
            _buff3[0] = _buff3[1] = _buff3[2] = 0;
            for (long k = 0; k < stream.Length / 3; k++)
                stream.Write(_buff3, 0, 3);
        }

        private Color[] ReadTileColors(MimTile tile)
        {
            return _mim.Textures.ReadIndices(tile).Select(i => _mim.Palettes[tile.Layered.PaletteId].Colors[i]).ToArray();
        }

        private void WriteColor(Color color, MimTileBlendType blendType, Stream ioStream)
        {
            if (color.R == 0x78 && color.G == 0x78 && color.B == 0x78)
                Console.WriteLine();

            if (ColorsHelper.IsBlack(color))
            {
                ioStream.Seek(3, SeekOrigin.Current);
                return;
            }

            if ((int)blendType < 4)
            {
                Color oldColor = ColorsHelper.ReadColor(ioStream, _buff3);
                color = BlendColor(color, oldColor, blendType);
                ioStream.Seek(-3, SeekOrigin.Current);
            }

            ColorsHelper.WriteBgr(ioStream, color);
        }

        private static Color BlendColor(Color newColor, Color oldColor, MimTileBlendType blendType)
        {
            byte nr = newColor.R;
            byte ng = newColor.G;
            byte nb = newColor.B;

            byte or = oldColor.R;
            byte og = oldColor.G;
            byte ob = oldColor.B;

            switch (blendType)
            {
                case MimTileBlendType.Average:
                    return Color.FromArgb(255,
                        (byte)Math.Min(255, ((nr + or) / 2)),
                        (byte)Math.Min(255, ((ng + og) / 2)),
                        (byte)Math.Min(255, ((nb + ob) / 2)));

                case MimTileBlendType.Add:
                    return Color.FromArgb(255,
                        (byte)Math.Min(255, (nr + or)),
                        (byte)Math.Min(255, (ng + og)),
                        (byte)Math.Min(255, (nb + ob)));

                case MimTileBlendType.Sub:
                    return Color.FromArgb(255,
                        (byte)Math.Min(255, (nr - or)),
                        (byte)Math.Min(255, (ng - og)),
                        (byte)Math.Min(255, (nb - ob)));

                case MimTileBlendType.Mul25:
                    return Color.FromArgb(255,
                        (byte)Math.Min(255, (0.25 * nr + or)),
                        (byte)Math.Min(255, (0.25 * ng + og)),
                        (byte)Math.Min(255, (0.25 * nb + ob)));

                default:
                    throw new NotSupportedException();
            }
        }
    }
}