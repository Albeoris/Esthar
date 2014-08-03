using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class Ambient
    {
        public readonly uint[] SoundsIds;

        public Ambient(uint[] soundsIds)
        {
            SoundsIds = Exceptions.CheckArgumentNull(soundsIds, "soundsIds");
        }
    }
}