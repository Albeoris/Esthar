namespace Esthar.Data.Transform
{
    public sealed class AsmAppearAskCommand : AsmCommand
    {
        private const int StackSize = 8;

        public AsmValueSource MessageChanel;
        public AsmValueSource MessageId;

        public AsmValueSource X;
        public AsmValueSource Y;

        public AsmValueSource FirstAnswerLine;
        public AsmValueSource LastAnswerLine;
        public AsmValueSource DefaultAnswerLine;
        public AsmValueSource CancelAnswerLine;

        public AsmAppearAskCommand(AsmEvent script, int offset)
            : base(JsmCommand.AASK, script, offset)
        {
            IAsmReadableCommandStack stack = GetReadableCommandStack(StackSize);
            MessageChanel = stack.Pop();
            MessageId = stack.Pop();
            FirstAnswerLine = stack.Pop();
            LastAnswerLine = stack.Pop();
            DefaultAnswerLine = stack.Pop();
            CancelAnswerLine = stack.Pop();
            X = stack.Pop();
            Y = stack.Pop();
        }

        public override void CommitChanges()
        {
            IAsmWriteableCommandStack stack = GetWriteableCommandStack(StackSize);
            stack.Push(MessageChanel);
            stack.Push(MessageId);
            stack.Push(FirstAnswerLine);
            stack.Push(LastAnswerLine);
            stack.Push(DefaultAnswerLine);
            stack.Push(CancelAnswerLine);
            stack.Push(X);
            stack.Push(Y);
        }
    }
}