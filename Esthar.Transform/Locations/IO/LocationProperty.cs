using System;

namespace Esthar.Data.Transform
{
    [Flags]
    public enum LocationProperty
    {
        None = 0,
        Tags = 1,
        Title = 2,
        PvP = 4,
        Information = 8,
        Monologues = 16,
        ExtraFonts = 32,
        Background = 64,
        Walkmesh = 128,
        FieldCamera = 256,
        MoviesCameras = 512,
        Placeables = 1024,
        Ambient = 2048,
        Encounters = 4096,
        Scripts = 8192,
        Models = 16384,
        Particles = 32768
    }
}