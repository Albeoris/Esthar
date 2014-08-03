namespace Esthar.Data.Transform
{
    public sealed class AsmMessageCommand : AsmCommand
    {
        private const int StackSize = 2;

        public AsmValueSource MessageChanel;
        public AsmValueSource MessageId;

        public AsmMessageCommand(AsmEvent script, int offset)
            : base(JsmCommand.MES, script, offset)
        {
            IAsmReadableCommandStack stack = GetReadableCommandStack(StackSize);
            MessageChanel = stack.Pop();
            MessageId = stack.Pop();
        }

        public override void CommitChanges()
        {
            IAsmWriteableCommandStack stack = GetWriteableCommandStack(StackSize);
            stack.Push(MessageChanel);
            stack.Push(MessageId);
        }
    }
}