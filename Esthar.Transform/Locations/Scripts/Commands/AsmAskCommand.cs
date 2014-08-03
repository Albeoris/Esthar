namespace Esthar.Data.Transform
{
    public sealed class AsmAskCommand : AsmCommand
    {
        private const int StackSize = 6;

        public AsmValueSource MessageChanel;
        public AsmValueSource MessageId;

        public AsmValueSource FirstAnswerLine;
        public AsmValueSource LastAnswerLine;
        public AsmValueSource DefaultAnswerLine;
        public AsmValueSource CancelAnswerLine;

        public AsmAskCommand(AsmEvent script, int offset)
            : base(JsmCommand.ASK, script, offset)
        {
            IAsmReadableCommandStack stack = GetReadableCommandStack(StackSize);
            MessageChanel = stack.Pop();
            MessageId = stack.Pop();
            FirstAnswerLine = stack.Pop();
            LastAnswerLine = stack.Pop();
            DefaultAnswerLine = stack.Pop();
            CancelAnswerLine = stack.Pop();
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
        }
    }
}