namespace Esthar.Data.Transform
{
    public interface ILocationReader
    {
        bool BeginRead(Location location);
        bool EndRead();
        bool ReadTags(Location location);
        bool ReadTitle(Location location);
        bool ReadPvP(Location location);
        bool ReadInformation(Location location);
        bool ReadMonologues(Location location);
        bool ReadExtraFonts(Location location);
        bool GetBackgroundReader(Location location);
        bool ReadWalkmesh(Location location);
        bool ReadCamera(Location location);
        bool ReadMoviesCameras(Location location);
        bool ReadPlaceables(Location location);
        bool ReadAmbient(Location location);
        bool ReadEncounters(Location location);
        bool ReadScripts(Location location);
        bool ReadModels(Location location);
        bool ReadParticles(Location location);
    }
}