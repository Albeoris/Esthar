namespace Esthar.Data.Transform
{
    public sealed class AsmAppearMessageCommand : AsmCommand
    {
        private const int StackSize = 4;

        public AsmValueSource MessageChanel;
        public AsmValueSource MessageId;

        public AsmValueSource X;
        public AsmValueSource Y;

        public AsmAppearMessageCommand(AsmEvent script, int offset)
            : base(JsmCommand.AMES, script, offset)
        {
            IAsmReadableCommandStack stack = GetReadableCommandStack(StackSize);
            MessageChanel = stack.Pop();
            MessageId = stack.Pop();
            X = stack.Pop();
            Y = stack.Pop();
        }

        public override void CommitChanges()
        {
            IAsmWriteableCommandStack stack = GetWriteableCommandStack(StackSize);
            stack.Push(MessageChanel);
            stack.Push(MessageId);
            stack.Push(X);
            stack.Push(Y);
        }
    }
}