using System.Collections.Generic;

namespace Esthar.Data.Transform
{
    public sealed class MovieCameras : List<MovieCamera>
    {
        public MovieCameras()
        {
        }

        public MovieCameras(int capacity)
            : base(capacity)
        {
        }

        public MovieCameras(IEnumerable<MovieCamera> collection)
            : base(collection)
        {
        }
    }
}