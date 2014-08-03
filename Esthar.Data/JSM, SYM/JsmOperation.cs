namespace Esthar.Data
{
    public sealed class JsmOperation
    {
        public readonly uint Operation;

        public JsmOperation(uint operation)
        {
            Operation = operation;
        }

        public bool HasArgument
        {
            get { return (Operation & 0xFF000000) != 0; }
        }

        public JsmCommand Command
        {
            get { return (JsmCommand)(HasArgument ? Operation >> 24 : Operation); }
        }

        public int Argument
        {
            get { return (int)((Operation & 0x00800000) == 0 ? (Operation & 0x00FFFFFF) : (Operation | 0xFF000000)); }
        }
    }
}