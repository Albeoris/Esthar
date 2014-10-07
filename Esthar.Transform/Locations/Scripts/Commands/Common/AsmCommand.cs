namespace Esthar.Data.Transform
{
    public abstract class AsmCommand
    {
        public readonly AsmEvent Script;
        public readonly int Offset;

        public readonly JsmOperation NativeOperation;

        public abstract void CommitChanges();

        protected AsmCommand(JsmCommand command, AsmEvent script, int offset)
        {
            Script = script;
            Offset = offset;
            NativeOperation = Script.GetOperation(command, offset);
        }

        protected IAsmReadableCommandStack GetReadableCommandStack(int size)
        {
            return Script.GetReadableCommandStack(Offset, size);
        }

        protected IAsmWriteableCommandStack GetWriteableCommandStack(int size)
        {
            return Script.GetWriteableCommandStack(Offset, size);
        }
    }
}