using Esthar.Core;

namespace Esthar.Data.Transform
{
    public sealed class Encounters
    {
        public ushort[] EnemiesID;
        public byte Frequency;

        public Encounters(ushort[] enemiesID, byte frequency)
        {
            EnemiesID = Exceptions.CheckArgumentNull(enemiesID, "enemiesID");
            Frequency = frequency;
        }
    }
}