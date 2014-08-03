using System;
using System.Linq;
using Esthar.Core;
using Esthar.OpenGL;

namespace Esthar.Data.Transform
{
    public sealed class LocationReaders
    {
        private readonly Location _location;
        private readonly ILocationReader[] _readers;

        public LocationReaders(Location location, params ILocationReader[] readers)
        {
            _location = Exceptions.CheckArgumentNull(location, "location");
            _readers = Exceptions.CheckArgumentNull(readers, "readers");
        }

        public LocationReadersState BeginRead()
        {
            bool result = false;
            foreach (ILocationReader reader in _readers)
            {
                if (reader.BeginRead(_location))
                    result = true;
            }

            if (!result)
                throw new Exception("Failed to begin read.");

            return new LocationReadersState(this);
        }

        public bool EndRead()
        {
            return _readers.All(r => r.EndRead());
        }

        public bool ReadTags()
        {
            return _readers.Any(r => r.ReadTags(_location));
        }

        public bool ReadTitle()
        {
            return _readers.Any(r => r.ReadTitle(_location));
        }

        public bool ReadPvP()
        {
            return _readers.Any(r => r.ReadPvP(_location));
        }

        public bool ReadInformation()
        {
            return _readers.Any(r => r.ReadInformation(_location));
        }

        public bool ReadMonologues()
        {
            return _readers.Any(r => r.ReadMonologues(_location));
        }

        public bool ReadExtraFonts()
        {
            return _readers.Any(r => r.ReadExtraFonts(_location));
        }

        public bool GetBackgroundReader()
        {
            return _readers.Any(r => r.GetBackgroundReader(_location));
        }

        public bool ReadWalkmesh()
        {
            return _readers.Any(r => r.ReadWalkmesh(_location));
        }

        public bool ReadCamera()
        {
            return _readers.Any(r => r.ReadCamera(_location));
        }

        public bool ReadMoviesCameras()
        {
            return _readers.Any(r => r.ReadMoviesCameras(_location));
        }

        public bool ReadPlaceables()
        {
            return _readers.Any(r => r.ReadPlaceables(_location));
        }

        public bool ReadAmbient()
        {
            return _readers.Any(r => r.ReadAmbient(_location));
        }

        public bool ReadEncounters()
        {
            return _readers.Any(r => r.ReadEncounters(_location));
        }

        public bool ReadScripts()
        {
            return _readers.Any(r => r.ReadScripts(_location));
        }

        public bool ReadModels()
        {
            return _readers.Any(r => r.ReadModels(_location));
        }

        public bool ReadParticles()
        {
            return _readers.Any(r => r.ReadParticles(_location));
        }
    }
}