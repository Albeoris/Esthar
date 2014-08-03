namespace Esthar.Data.Transform
{
    public abstract class AsmCommand
    {
        public readonly AsmEvent Script;
        public readonly int Offset;

        public readonly AsmOperation NativeOperation;

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

        protected bool TrySetAbsoluteValue(ref AsmValueSource source, int value)
        {
            if (source == null || source.Type != AsmValueSourceType.Static)
                return false;

            source.SetAbsoluteValue(value);
            return true;
        }

        protected void SetAbsoluteValue(ref AsmValueSource source, int value)
        {
            if (source != null && source.Type == AsmValueSourceType.Static)
                source.SetAbsoluteValue(value);

            source = new AsmStaticValueSource(new AsmOperation { Command = JsmCommand.PSHN_L, Argument = value });
        }
    }
}