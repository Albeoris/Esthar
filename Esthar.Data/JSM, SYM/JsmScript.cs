namespace Esthar.Data
{
    public sealed class JsmScript
    {
        public readonly ushort Position;
        public readonly bool Flag;
        public int OperationsCount;

        public JsmScript(ushort position, bool flag)
        {
            Position = position;
            Flag = flag;
        }
    }
}