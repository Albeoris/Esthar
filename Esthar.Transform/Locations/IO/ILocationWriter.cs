using Esthar.Core;

namespace Esthar.Data.Transform
{
    public interface ILocationWriter
    {
        bool BeginWrite();
        bool EndWrite();
        void WriteTitle(string tags);
        void WritePvP(uint? pvp);
        void WriteInformation(FieldInfo info);
        void WriteMonologues(LocalizableStrings monologues);
        void WriteExtraFonts(GameFont extraFont);
        void WriteBackground(GameImage background);
        void WriteWalkmesh(Walkmesh walkmesh);
        void WriteCamera(FieldCameras fieldCameras);
        void WriteMoviesCameras(MovieCameras movieCameras);
        void WritePlaceables(Placeables placeables);
        void WriteAmbient(Ambient ambient);
        void WriteEncounters(Encounters encounters);
        void WriteScripts(AsmCollection scripts);
        void WriteModels(SafeHGlobalHandle oneContent);
        void WriteParticles(Particles particles);
    }
}